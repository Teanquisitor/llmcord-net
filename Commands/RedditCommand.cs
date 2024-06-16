using Discord;
using Discord.WebSocket;
using Services;

namespace Commands;

[User]
public class RedditCommand : CommandBase
{
    private readonly RedditService service;

    public RedditCommand()
    {
        Name = "Reddit";
        Description = "Search Reddit";
        Syntax = "<command> <subreddit>";
        Aliases = new List<string> { "reddit" };

        service = new RedditService();
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var subreddit = message.Content.Split(' ')[1];

        var (Title, URL, Thumbnail) = await service.GetPostAsync(subreddit);
        var embed = new EmbedBuilder();
        embed.WithAuthor(Title, url: URL);
        embed.WithImageUrl(URL);
        embed.WithThumbnailUrl(Thumbnail);
        embed.WithFooter($"Requested by {message.Author.Username}");
        embed.WithColor(Color.Blue);
        await message.Channel.SendMessageAsync(embed: embed.Build());
    }
}