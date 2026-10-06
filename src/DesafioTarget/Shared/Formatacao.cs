using System.Globalization;

namespace DesafioTarget.Shared;

public static class Formatacao
{
    public static CultureInfo Cultura { get; } = ObterCultura();

    public static string Moeda(decimal valor) => $"R$ {valor.ToString("N2", Cultura)}";

    public static string Data(DateOnly data) => data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public static string Percentual(decimal taxa) => $"{(taxa * 100).ToString("0.##", Cultura)}%";

    private static CultureInfo ObterCultura()
    {
        try
        {
            return new CultureInfo("pt-BR");
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}