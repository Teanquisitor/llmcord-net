using Discord.WebSocket;

namespace Commands;

[User]
public class PingCommand : CommandBase
{
    public PingCommand()
    {
        Name = "Ping";
        Description = "Ping the bot";
        Syntax = "<command>";
        Aliases = new List<string> { "ping" };
    }

    public override async Task ExecuteAsync(SocketMessage message) => await message.Channel.SendMessageAsync($"{Program.Client.Latency}ms");
}