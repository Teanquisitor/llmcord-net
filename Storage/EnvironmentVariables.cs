namespace Storage;

public static class EnvironmentVariables
{
    private static readonly Dictionary<string, string> variables = new Dictionary<string, string>();

    public static string Token => GetVariable("STATIC_TOKEN")!;

    public static string LLM => GetVariable("LLM")!;
    public static string Vision => GetVariable("VISION")!;
    public static string LLM_URL => GetVariable("LLM_URL")!;
    
    public static string ContextFile => GetVariable("CONTEXT_FILE")!;
    public static string MaxTokens => GetVariable("MAX_TOKENS")!;

    public static string Python => GetVariable("PYTHON")!;
    public static string PythonTranslate => GetVariable("PYTHON_TRANSLATE")!;
    public static string PythonReddit => GetVariable("PYTHON_REDDIT")!;

    public static string InputFile => GetVariable("INPUT_FILE")!;
    public static string OutputFile => GetVariable("OUTPUT_FILE")!;

    public static string ReminderFile => GetVariable("REMINDER_FILE")!;

    public static string Whitelisted => GetVariable("WHITELIST")!;

    static EnvironmentVariables() => LoadVariablesFromFile();

    private static void LoadVariablesFromFile()
    {
        var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"C:\Users\White\Desktop\llmcord\llmcord-net\Storage\config");
        if (!File.Exists(envFilePath))
            return;

        foreach (var line in File.ReadAllLines(envFilePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                variables[key] = value;
            }
        }
    }

    private static string? GetVariable(string key)
    {
        if (!variables.TryGetValue(key, out var value))
            return null;

        return value;
    }

}