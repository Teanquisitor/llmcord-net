using System.Text.Json;
using Discord;
using Discord.WebSocket;
using Storage;

namespace Services;

public class ReminderService
{
    private static List<Record> records = new();

    public static async Task StartAsync()
    {
        var file = File.ReadAllText(EnvironmentVariables.ReminderFile);
        records = JsonSerializer.Deserialize<List<Record>>(file) ?? new();

        _ = Task.Run(ReminderLoop);
        await Task.CompletedTask;
    }

    public static void AddReminder(SocketUser user, DateTime time, string content)
    {
        records.Add(new Record(new UserData(user), time, content));
        File.WriteAllText(EnvironmentVariables.ReminderFile, JsonSerializer.Serialize(records));
    }

    private static async Task ReminderLoop()
    {
        while (true)
        {
            var currentTime = DateTime.Now;
            var recordsToRemove = new List<Record>();
            
            foreach (var record in records)
            {
                if (record.Time > currentTime)
                    continue;

                var user = await Program.Client.Rest.GetUserAsync(record.User.Id);
                await user.SendMessageAsync(record.Message);
                recordsToRemove.Add(record);
            }

            foreach (var record in recordsToRemove)
                records.Remove(record);

            File.WriteAllText(EnvironmentVariables.ReminderFile, JsonSerializer.Serialize(records));

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}

public struct UserData
{
    public UserData(SocketUser user)
    {
        Id = user.Id;
        Username = user.Username;
    }

    public ulong Id { get; }
    public string Username { get; }
}

public class Record
{
    public UserData User { get; }
    public DateTime Time { get; }
    public string Message { get; }

    public Record(UserData user, DateTime time, string message)
    {
        User = user;
        Time = time;
        Message = message;
    }
}