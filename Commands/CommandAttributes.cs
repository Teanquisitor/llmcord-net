using Discord.WebSocket;
using Storage;

namespace Commands;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public abstract class CommandAttributeBase : Attribute
{
    public abstract bool HasPermission(SocketMessage message);
}

public class AdminAttribute : CommandAttributeBase
{
    
    public override bool HasPermission(SocketMessage message) => message.Author is SocketGuildUser user && user.GuildPermissions.Administrator;
}

public class WhitelistedAttribute : CommandAttributeBase
{
    public override bool HasPermission(SocketMessage message)
    {
        var path = EnvironmentVariables.Whitelisted;
        var whitelist = File.ReadAllLines(path);
        return File.ReadAllLines(EnvironmentVariables.Whitelisted)
            .Any(line => line.Equals(message.Author.Id.ToString(), StringComparison.Ordinal));
    }
}

public class UserAttribute : CommandAttributeBase
{
    public override bool HasPermission(SocketMessage message) => true;
}