using Discord;
using Discord.WebSocket;

namespace Commands;

[Admin, Whitelisted]
public class DeleteCommand : CommandBase
{
    public DeleteCommand()
    {
        Name = "Delete";
        Description = "Delete the last X messages.";
        Syntax = "<command> <count>";
        Aliases = new List<string> { "delete", "del", "purge" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var channel = message.Channel;

        var count = int.Parse(message.Content.Split(' ')[1]);
        var messages = await channel.GetMessagesAsync(count + 1).FlattenAsync();
        if (channel is SocketTextChannel textChannel)
            await textChannel.DeleteMessagesAsync(messages);
    }
}