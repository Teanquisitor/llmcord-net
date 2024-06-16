using Discord;
using Discord.WebSocket;
using Services;

namespace Commands;

[User]
public class MemeCommand : CommandBase
{
    private readonly RedditService service;

    public MemeCommand()
    {
        Name = "Meme";
        Description = "Get a random meme";
        Syntax = "<command>";
        Aliases = new List<string> { "meme" };

        service = new RedditService();
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var (Title, URL, _) = await service.GetPostAsync("memes");
        var embed = new EmbedBuilder();
        embed.WithAuthor(Title, url: URL);
        embed.WithImageUrl(URL);
        embed.WithFooter($"Requested by {message.Author.Username}");
        embed.WithColor(Color.Blue);
        await message.Channel.SendMessageAsync(embed: embed.Build());
    }
}