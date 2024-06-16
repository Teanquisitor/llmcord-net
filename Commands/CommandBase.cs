using Discord.WebSocket;

namespace Commands;

public abstract class CommandBase
{
    public string Name;
    public string Description;
    public string Syntax;
    public List<string> Aliases;

    public abstract Task ExecuteAsync(SocketMessage message);
}