using System.Reflection;
using Discord.WebSocket;

namespace Commands;

[User]
public class HelpCommand : CommandBase
{
    public HelpCommand()
    {
        Name = "Help";
        Description = "List all available commands";
        Syntax = "<command>";
        Aliases = new List<string> { "commands", "cmds" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var commands = CommandHandler.GetCommands();

        string output = "";
        foreach (var command in commands)
        {
            var attributes = command.GetType().GetCustomAttributes<CommandAttributeBase>(true);
            if (!attributes.All(a => a.HasPermission(message)))
                continue;
                
            output += $"```fix\n{string.Join(", ", command.Aliases)} — {command.Description}\nSyntax: {command.Syntax}\n\n```";
        }

        await message.Channel.SendMessageAsync(output);
    }
}