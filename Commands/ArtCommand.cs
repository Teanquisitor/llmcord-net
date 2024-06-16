using Discord;
using Discord.WebSocket;
using Services;

namespace Commands;

[User]
public class ArtCommand : CommandBase
{
    private readonly RedditService service;

    public ArtCommand()
    {
        Name = "Art";
        Description = "Get a random art";
        Syntax = "<command>";
        Aliases = new List<string> { "art" };

        service = new RedditService();
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var (Title, URL, _) = await service.GetPostAsync("AnimeArt");
        var embed = new EmbedBuilder();
        embed.WithAuthor(Title, url: URL);
        embed.WithImageUrl(URL);
        embed.WithFooter($"Requested by {message.Author.Username}");
        embed.WithColor(Color.Blue);
        await message.Channel.SendMessageAsync(embed: embed.Build());
    }
}