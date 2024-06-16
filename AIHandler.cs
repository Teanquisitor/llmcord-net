using System.Text;
using System.Text.Json;
using Discord;
using Discord.WebSocket;
using Storage;

class AIHandler
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<bool> ExecuteAsync(SocketMessage message)
    {
        if (message.MentionedUsers.Any(user => user.Id == Program.Client.CurrentUser.Id))
        {
            var attachment = await GetAttachmentAsync(message);
            if (attachment != null)
                return await HandleVisionModel(message, attachment);

            return await HandleLanguageModel(message);
        }
        
        var context = Context.LoadHistory();
        Context.AddMessage(context, $"{message.Author.Username}: {message.Content}\n");

        return false;
    }

    private static async Task<bool> HandleVisionModel(SocketMessage message, Attachment attachment)
    {
        var content = PrepareContent(message);
        var imageBase64 = await GetImageBase64(attachment);

        var context = Context.LoadHistory();
        var currentMessage = $"{message.Author.Username}: {content}\n";
        Context.AddMessage(context, currentMessage);

        await message.Channel.TriggerTypingAsync();

        var requestBody = JsonSerializer.Serialize(new
        {
            model = EnvironmentVariables.Vision,
            prompt = content,
            images = new[] { imageBase64 }
        });

        var visionResponse = await httpClient.PostAsync(EnvironmentVariables.LLM_URL, new StringContent(requestBody, Encoding.UTF8, "application/json"));

        if (!visionResponse.IsSuccessStatusCode)
        {
            await Program.Log(new LogMessage(LogSeverity.Critical, "AI", "Failed to get response from Vision. Response: " + visionResponse.StatusCode));
            return false;
        }

        var fullResponse = await GetResponseFromVisionJson(visionResponse);
        var assistanceResponse = $"{Program.Client.CurrentUser.Username}: {fullResponse}\n";
        Context.AddMessage(context, assistanceResponse);

        while (fullResponse.Length > 0)
        {
            var part = fullResponse[..Math.Min(1920, fullResponse.Length)];
            await message.Channel.SendMessageAsync(part);
            fullResponse = fullResponse[part.Length..];
        }
        return true;
    }

    private static async Task<bool> HandleLanguageModel(SocketMessage message)
    {
        var content = PrepareContent(message);
        var context = Context.LoadHistory();

        var currentMessage = $"{message.Author.Username}: {content}\n";
        if (message.Reference != null)
        {
            var reference = message.Channel.GetMessageAsync((ulong)message.Reference.MessageId).Result;
            var referenceContent = reference.Content;
            currentMessage += $"Replying to {reference.Author.Username}: {referenceContent}\n";
        }

        var prompt = $"{Context.Get(context)}\nCurrent context:\n{currentMessage}";
        Context.AddMessage(context, currentMessage);

        await message.Channel.TriggerTypingAsync();

        var requestBody = JsonSerializer.Serialize(new
        {
            model = EnvironmentVariables.LLM,
            prompt,
            stream = false
        });

        var response = await httpClient.PostAsync(EnvironmentVariables.LLM_URL, new StringContent(requestBody, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            await Program.Log(new LogMessage(LogSeverity.Critical, "AI", "Failed to get response from LLM. Response: " + response.StatusCode));
            return false;
        }

        var output = await GetResponseFromLLMJson(response);
        var assistanceResponse = $"{Program.Client.CurrentUser.Username}: {output}\n";
        Context.AddMessage(context, assistanceResponse);

        while (output.Length > 0)
        {
            var part = output[..Math.Min(1920, output.Length)];
            await message.Channel.SendMessageAsync(part);
            output = output[part.Length..];
        }
        return true;
    }

    private static string PrepareContent(SocketMessage message)
    {
        var content = message.Content;
        foreach (var user in message.MentionedUsers)
            content = content.Replace($"<@{user.Id}>", $"{user.Username}");
        return content;
    }

    private static async Task<string> GetResponseFromVisionJson(HttpResponseMessage visionResponse)
    {
        var visionResp = await visionResponse.Content.ReadAsStringAsync();
        var jsonObjects = visionResp.Split('\n');

        var jsonResponses = new List<JsonResponse>();
        foreach (var jsonObject in jsonObjects)
            if (!string.IsNullOrWhiteSpace(jsonObject))
                jsonResponses.Add(JsonSerializer.Deserialize<JsonResponse>(jsonObject)!);

        return string.Concat(jsonResponses.Select(r => r.response));
    }

    private static async Task<string> GetResponseFromLLMJson(HttpResponseMessage response)
        => JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync())!["response"].ToString()!;

    private static async Task<string> GetImageBase64(Attachment attachment)
        => Convert.ToBase64String(await httpClient.GetByteArrayAsync(new Uri(attachment.Url)));

    private static async Task<Attachment?> GetAttachmentAsync(SocketMessage message)
    {
        if (message.Attachments.Any())
            return message.Attachments.FirstOrDefault()!;

        if (message.Reference != null)
        {
            var referenceMessage = await message.Channel.GetMessageAsync((ulong)message.Reference.MessageId);
            if (referenceMessage.Attachments.Any())
                return (Attachment)referenceMessage.Attachments.FirstOrDefault()!;
        }

        return null;
    }
    
}

public class JsonResponse
{
    public string response { get; set; }
}