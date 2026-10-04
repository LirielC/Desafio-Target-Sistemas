using System.Text.Json;

namespace TargetChallenge.Infrastructure;

public static class JsonDataLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T Load<T>(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Arquivo JSON não encontrado.", path);

        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream, Options)
            ?? throw new InvalidDataException($"O arquivo '{path}' não contém um JSON válido para {typeof(T).Name}.");
    }
}
