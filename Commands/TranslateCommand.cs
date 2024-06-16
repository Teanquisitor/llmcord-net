using Discord.WebSocket;
using Services;

namespace Commands;

[User]
public class TranslateCommand : CommandBase
{
    private readonly TranslationService service;

    public TranslateCommand()
    {
        Name = "Translate";
        Description = "Translate text to another language.";
        Syntax = "<command> <language>";
        Aliases = new List<string> { "translate", "t" };

        service = new TranslationService();
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var language = message.Content.Split(' ')[1];
        var reference = await message.Channel.GetMessageAsync((ulong)message.Reference.MessageId);
        var text = reference.Content;

        var translation = await service.TranslateTextAsync(language, text);
        var response = $"Google Translation({language}):\n```fix\n{translation}```";
        await message.Channel.SendMessageAsync(response);
    }
}