using System.Diagnostics;
using Discord.WebSocket;

namespace Commands;

[User]
public class DownloadCommand : CommandBase
{
    private static string targetDirectory = "C:\\Users\\White\\Desktop\\llmcord\\llmcord-net\\Storage";

    public DownloadCommand()
    {
        Name = "Download";
        Description = "Download an M4A audio file from YouTube";
        Syntax = "<command> <url>";
        Aliases = new List<string> { "download", "ytd" };
    }

    public override async Task ExecuteAsync(SocketMessage message)
    {
        var url = message.Content.Split(' ')[1];

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return;

        var processInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = $"-f \"bestaudio[ext=m4a]\" -x {url}",
            WorkingDirectory = targetDirectory,
            UseShellExecute = false
        };
        Process.Start(processInfo)?.WaitForExit();

        var videoId = uri.Segments[^1];
        var audioFiles = Directory.GetFiles(targetDirectory, $"*{videoId}*");

        if (audioFiles.Length == 0)
        {
            await message.Channel.SendMessageAsync("No file found.");
            return;
        }

        var audioFile = audioFiles.First();
        await message.Channel.SendFileAsync(audioFile);
        File.Delete(audioFile);
    }
}