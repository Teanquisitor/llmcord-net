using System.Reflection;
using Commands;
using Discord.WebSocket;

class CommandHandler
{
    private static List<CommandBase> commands = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsSubclassOf(typeof(CommandBase)) && !t.IsAbstract)
        .Select(t => (CommandBase)Activator.CreateInstance(t)!)
        .ToList()!;
    public static List<CommandBase> GetCommands() => commands;

    public static bool Execute(SocketMessage message)
    {
        var content = message.Content;

        foreach (var command in commands)
        {
            if (!command.Aliases.Any(alias => alias.Equals(content.Split(' ').FirstOrDefault(), StringComparison.OrdinalIgnoreCase)))
                continue;
            
            var attributes = command.GetType().GetCustomAttributes<CommandAttributeBase>(true);
            if (!attributes.All(a => a.HasPermission(message)))
                continue;

            _ = command.ExecuteAsync(message);
            return true;
        }

        return false;
    }   
}