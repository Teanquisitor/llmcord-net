using Discord;
using Discord.WebSocket;
using Services;

namespace Commands;

[User]
public class ReminderCommand : CommandBase
{
    public ReminderCommand()
    {
        Name = "Reminder";
        Description = "Set a reminder";
        Syntax = "<command> <time> <message>";
        Aliases = new List<string> { "remind" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var args = message.Content.Split(' ');

        SocketUser user = (SocketUser)await message.Channel.GetUserAsync(message.Author.Id);
        var time = args[1];
        var timeToSend = DateTime.Now.AddHours(double.Parse(time));
        var content = GetContentAsync(message).Result;

        ReminderService.AddReminder(user, timeToSend, content!);

        await message.Channel.SendMessageAsync($"Reminder set for {timeToSend}");
    }

    private static async Task<string?> GetContentAsync(SocketMessage message)
    {
        var args = message.Content.Split(' ');
        var content = string.Join(' ', args.Skip(2));

        if (!string.IsNullOrEmpty(content))
            return content;

        if (message.Reference != null)
            return (await message.Channel.GetMessageAsync((ulong)message.Reference.MessageId)).Content;

        return null;
    }
}