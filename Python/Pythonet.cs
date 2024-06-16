using System.Diagnostics;
using System.Text.Json;
using Storage;

namespace Python;

public static class Pythonet<Input, Output>
{
    public static void Execute(string method, Input args, out Output results)
    {
        var jsonString = JsonSerializer.Serialize(args);
        File.WriteAllText(EnvironmentVariables.InputFile, jsonString);

        var processInfo = new ProcessStartInfo
        {
            FileName = EnvironmentVariables.Python,
            Arguments = method,
            UseShellExecute = false
        };
        Process.Start(processInfo)?.WaitForExit();

        jsonString = File.ReadAllText(EnvironmentVariables.OutputFile);
        results = JsonSerializer.Deserialize<Output>(jsonString)!;
    }
}