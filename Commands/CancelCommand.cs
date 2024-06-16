using System.Diagnostics;
using Discord.WebSocket;

namespace Commands;

[Whitelisted]
public class CancelCommand : CommandBase
{
    public CancelCommand()
    {
        Name = "Cancel";
        Description = "Cancel the current shutdown.";
        Syntax = "<command>";
        Aliases = new List<string> { "cancel" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        Process.Start("shutdown", "-a");
        await message.Channel.SendMessageAsync($"Отмена выключения компьютера.");
    }
}