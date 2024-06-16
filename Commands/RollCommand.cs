using Discord;
using Discord.WebSocket;

namespace Commands;

[User]
public class RollCommand : CommandBase
{
    public RollCommand()
    {
        Name = "Roll";
        Description = "Roll a dice";
        Syntax = "<command> <sides> [cubes] [iterations]";
        Aliases = new List<string> { "roll", "r" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var arguments = message.Content.Split(' ');

        int sides;
        int cubes = 1;
        int iterations = 1;

        // Parse the required argument: sides
        if (arguments.Length < 2 || !int.TryParse(arguments[1], out sides))
        {
            await message.Channel.SendMessageAsync("Invalid input: <sides> must be a positive integer.");
            return;
        }

        // Parse the optional argument: cubes
        if (arguments.Length >= 3 && !int.TryParse(arguments[2], out cubes))
        {
            await message.Channel.SendMessageAsync("Invalid input: [cubes] must be a positive integer.");
            return;
        }

        // Parse the optional argument: iterations
        if (arguments.Length == 4 && !int.TryParse(arguments[3], out iterations))
        {
            await message.Channel.SendMessageAsync("Invalid input: [iterations] must be a positive integer.");
            return;
        }

        var random = new Random();
        var allRolls = new List<List<int>>();

        for (int i = 0; i < iterations; i++)
        {
            var iterationRolls = new List<int>();
            for (int j = 0; j < cubes; j++)
                iterationRolls.Add(random.Next(1, sides + 1));

            allRolls.Add(iterationRolls);
        }

        var embed = new EmbedBuilder()
            .WithTitle("Roll")
            .WithDescription($"{message.Author.Mention} rolled {iterations} time{(iterations > 1 ? "s" : "")} {cubes} cube{(cubes > 1 ? "s" : "")} with {sides} side{(sides > 1 ? "s" : "")}");
            for (var i = 0; i < iterations; i++)
            {
                var iterationRolls = allRolls[i];
                var iterationTotal = iterationRolls.Sum();
                embed.AddField($"Iteration {i + 1}", $"Results: {string.Join(", ", iterationRolls)}\nTotal: {iterationTotal} out of {cubes * sides}", true);
            }
            embed.AddField("Total", $"{allRolls.Select(x => x.Sum()).Sum()} out of {iterations * cubes * sides}")
            .WithColor(Color.Blue);

        await message.Channel.SendMessageAsync(embed: embed.Build());
    }
}