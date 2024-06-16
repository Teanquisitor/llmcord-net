using System.Reflection;
using Discord;
using Discord.WebSocket;

namespace Commands;

[User]
public class EmbedHelpCommand : CommandBase
{
    public EmbedHelpCommand()
    {
        Name = "Embed Help";
        Description = "List all available commands";
        Syntax = "<command>";
        Aliases = new List<string> { "help", "h", "??" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var commands = CommandHandler.GetCommands();

        var embed = new EmbedBuilder();
        embed.WithTitle("Available commands");
        embed.WithColor(Color.Blue);
        foreach (var command in commands)
        {
            var attributes = command.GetType().GetCustomAttributes<CommandAttributeBase>(true);
            if (!attributes.All(a => a.HasPermission(message)))
                continue;
                
            embed.AddField($"{string.Join(", ", command.Aliases)} — {command.Description}", $"Syntax: {command.Syntax}", false);
        }

        await message.Channel.SendMessageAsync(embed: embed.Build());
    }
}