using System.Diagnostics;
using Discord.WebSocket;

namespace Commands;

[Whitelisted]
public class ShutdownCommand : CommandBase
{
    public ShutdownCommand()
    {
        Name = "Shutdown";
        Description = "Shut down the computer.";
        Syntax = "<command>";
        Aliases = new List<string> { "shutdown" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        Process.Start("shutdown", "-s -t 360");
        await message.Channel.SendMessageAsync($"Выключение компьютера через 360 секунд.");
    }
}