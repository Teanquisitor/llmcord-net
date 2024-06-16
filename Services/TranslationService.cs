using Python;
using Storage;

namespace Services;

public class TranslationService
{
    public async Task<string> TranslateTextAsync(string language, string text)
    {
        Pythonet<Input, Output>.Execute(EnvironmentVariables.PythonTranslate, new Input(language, text), out var results);
        await Task.CompletedTask;
        return results.Text;
    }

    private record Input(string Language, string Text);
    private record Output(string Text);
}