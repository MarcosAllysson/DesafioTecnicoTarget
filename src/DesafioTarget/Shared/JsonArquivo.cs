using System.Text.Json;

namespace DesafioTarget.Shared;

public static class JsonArquivo
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public static T Ler<T>(string caminho) where T : class
    {
        if (string.IsNullOrWhiteSpace(caminho))
            throw new DadosInvalidosException("O caminho do arquivo não foi informado.");

        if (!File.Exists(caminho))
            throw new DadosInvalidosException($"Arquivo não encontrado: {caminho}");

        try
        {
            var json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<T>(json, Opcoes)
                   ?? throw new DadosInvalidosException($"O arquivo não possui conteúdo: {caminho}");
        }
        catch (JsonException ex)
        {
            throw new DadosInvalidosException($"JSON inválido em '{caminho}': {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new DadosInvalidosException($"Não foi possível ler '{caminho}': {ex.Message}", ex);
        }
    }
}