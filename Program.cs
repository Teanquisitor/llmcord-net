using System.Security.Cryptography;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Services;
using Storage;

class Program
{
    private static DiscordSocketClient client;
    private static CommandService commands;

    public static DiscordSocketClient Client => client;

    static async Task Main(string[] args)
    {
        var program = new Program();
        await program.RunBotAsync();
    }

    public async Task RunBotAsync()
    {
        var discordClientConfig = new DiscordSocketConfig { GatewayIntents = GatewayIntents.All };
        client = new DiscordSocketClient(discordClientConfig);

        var commandServiceConfig = new CommandServiceConfig { CaseSensitiveCommands = false };
        commands = new CommandService(commandServiceConfig);


        


        client.Log += Log;
        commands.Log += Log;

        client.Ready += OnReady;
        client.MessageReceived += OnMessage;
        client.SlashCommandExecuted += OnSlashCommand;

        await client.LoginAsync(TokenType.Bot, EnvironmentVariables.Token);
        await client.StartAsync();

        await Task.Delay(-1);
    }

    public static Task Log(LogMessage log)
    {
        Console.ForegroundColor = log.Severity switch
        {
            LogSeverity.Critical or LogSeverity.Error => ConsoleColor.Red,
            LogSeverity.Warning => ConsoleColor.Yellow,
            LogSeverity.Info => ConsoleColor.White,
            LogSeverity.Verbose or LogSeverity.Debug => ConsoleColor.DarkGray,
            _ => throw new ArgumentOutOfRangeException(nameof(log.Severity))
        };
        Console.WriteLine($"{log.Severity,-8}> {log.Source}: {log.Message}");
        Console.ResetColor();

        return Task.CompletedTask;
    }

    private async Task<Task> OnReady()
    {
        // var global = new SlashCommandBuilder();
        // global.WithName("help");
        // global.WithDescription("Список команд");

        // await client.CreateGlobalApplicationCommandAsync(global.Build());

        // try
        // {
        //     var guild = new SlashCommandBuilder()
        //         .WithName("feedback")
        //         .WithDescription("Обратная связь")
        //         .AddOption(new SlashCommandOptionBuilder()
        //             .WithName("rating")
        //             .WithDescription("Оценка")
        //             .WithRequired(true)
        //             .AddChoice("1", "1")
        //             .AddChoice("2", "2")
        //             .AddChoice("3", "3")
        //             .AddChoice("4", "4")
        //             .AddChoice("5", "5")
        //             .WithType(ApplicationCommandOptionType.Integer));

        //     await client.CreateGlobalApplicationCommandAsync(guild.Build());
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Ошибка при регистрации команды feedback: {ex.Message}");
        // }

        await client.SetActivityAsync(new Game($"{EnvironmentVariables.LLM}"));

        await ReminderService.StartAsync();
        Console.WriteLine("READY!");

        return Task.CompletedTask;
    }

    private Task OnMessage(SocketMessage message)
    {
        Console.ForegroundColor = message.Author.IsBot ? ConsoleColor.DarkYellow : ConsoleColor.DarkCyan;
        Console.WriteLine($"{message.Channel} > {message.Author.Username}: {message.Content}");
        Console.ResetColor();

        if (message.Author.IsBot)
            return Task.CompletedTask;

        var result = CommandHandler.Execute(message);
        if (result)
            return Task.CompletedTask;

        _ = AIHandler.ExecuteAsync(message);

        return Task.CompletedTask;
    }

    private async Task<Task> OnSlashCommand(SocketSlashCommand command)
    {
        Console.WriteLine($"{command.Channel, -8} > {command.User.Username}: {command.Data.Name}");
        var embed = new EmbedBuilder();
        embed.WithTitle("Список команд");
        embed.WithDescription("Список доступных команд:");
        embed.WithColor(Color.Blue);
        embed.AddField("commands", "список команд");
        embed.AddField("h | help", "показать это сообщение");
        embed.AddField("aud [кол-во]", "вывести [кол-во] последних сообщений из аудит логов");
        embed.AddField("de [кол-во]", "удалить [кол-во] последних сообщений");
        embed.AddField("r | roll [итерации] [кубики] [грани]", "бросить кубики");
        embed.AddField("t | translate", "перевести сообщение");
        embed.AddField("m | meme", "мем");
        embed.AddField("a | art", "арт");
        embed.AddField("inv | invite", "одноразовое приглашение");
        embed.AddField("av | avatar", "аватар");
        embed.AddField("si | serverinfo", "информация о сервере");
        embed.AddField("ui | userinfo", "информация о пользователе");
        embed.AddField("ri | roleinfo", "информация о роле");
        embed.AddField("ping", "пинг");
        embed.AddField("d | disable", "выключить компьютер");
        embed.AddField("c | cancel", "отменить выключение компьютера");
        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        return Task.CompletedTask;
    }
}