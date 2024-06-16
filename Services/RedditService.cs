using Python;
using Storage;

namespace Services;

public class RedditService
{
    public async Task<(string, string, string)> GetPostAsync(string subreddit)
    {
        Pythonet<Input, Output>.Execute(EnvironmentVariables.PythonReddit, new Input(subreddit), out var results);
        await Task.CompletedTask;
        return (results.Title, results.URL, results.Thumbnail);
    }

    private record Input(string Subreddit);
    private record Output(string Title, string URL, string Thumbnail);
}