using Discord;
using Discord.WebSocket;

namespace Commands;

[User]
public class AvatarCommand : CommandBase
{
    public AvatarCommand()
    {
        Name = "Avatar";
        Description = "Get a user's avatar";
        Syntax = "<command> <user>";
        Aliases = new List<string> { "avatar", "av" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var user = message.MentionedUsers.FirstOrDefault();
        var embed = new EmbedBuilder();
        if (user is null)
        {
            embed.WithAuthor(message.Author.Username);
            embed.WithImageUrl(message.Author.GetAvatarUrl(size: 1024));
            embed.WithFooter($"Requested by {message.Author.Username}");
            embed.WithColor(Color.Blue);
            await message.Channel.SendMessageAsync(embed: embed.Build());

            return;
        }

        embed.WithAuthor(user.Username);
        embed.WithImageUrl(user.GetAvatarUrl(size: 1024));
        embed.WithFooter($"Requested by {message.Author.Username}");
        embed.WithColor(Color.Blue);
        await message.Channel.SendMessageAsync(embed: embed.Build());
    }
}