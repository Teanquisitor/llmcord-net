using System.Text.Json;

namespace Storage;

public static class Context
{
    public static string Get(Queue<string> context) => string.Join(" ", context);

    public static Queue<string> LoadHistory()
    {
        if (!File.Exists(EnvironmentVariables.ContextFile))
            return new Queue<string>();

        var json = File.ReadAllText(EnvironmentVariables.ContextFile);
        return new Queue<string>(JsonSerializer.Deserialize<List<string>>(json)!);
    }

    public static void SaveHistory(Queue<string> context) => File.WriteAllText(EnvironmentVariables.ContextFile, JsonSerializer.Serialize(context.ToList()));

    public static void CleanHistory(Queue<string> context)
    {
        var length = Get(context).Length;
        var maxLength = int.Parse(EnvironmentVariables.MaxTokens);
        while (length > maxLength)
        {
            var oldestMessage = context.Dequeue();
            length -= oldestMessage.Length;
        }
    }

    public static void AddMessage(Queue<string> context, string message)
    {
        context.Enqueue(message);
        CleanHistory(context);
        SaveHistory(context);
    }

}