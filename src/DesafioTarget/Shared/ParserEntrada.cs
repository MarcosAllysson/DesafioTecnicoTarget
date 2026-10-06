using System.Globalization;

namespace DesafioTarget.Shared;

/// <summary>
/// Converte texto digitado/recebido por argumento. Aceita formatos brasileiros e internacionais,
/// pois não se sabe se quem executa digita "1.234,56" ou "1234.56".
/// </summary>
public static class ParserEntrada
{
    private static readonly string[] FormatosData = { "d/M/yyyy", "d-M-yyyy", "yyyy-MM-dd" };

    public static bool TentarConverterInteiro(string? texto, out int valor)
        => int.TryParse(texto?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out valor);

    public static bool TentarConverterData(string? texto, out DateOnly data)
        => DateOnly.TryParseExact(texto?.Trim(), FormatosData, CultureInfo.InvariantCulture, DateTimeStyles.None, out data);

    public static bool TentarConverterDecimal(string? texto, out decimal valor)
    {
        valor = 0m;

        if (string.IsNullOrWhiteSpace(texto)) return false;

        var limpo = texto
            .Replace("R$", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty);

        if (limpo.Length == 0) return false;

        var separadorDecimal = DescobrirSeparadorDecimal(limpo);
        var separadorMilhar = separadorDecimal == ',' ? '.' : ',';

        var normalizado = limpo
            .Replace(separadorMilhar.ToString(), string.Empty)
            .Replace(separadorDecimal, '.');

        return decimal.TryParse(
            normalizado,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out valor
        );
    }

    private static char DescobrirSeparadorDecimal(string texto)
    {
        var ultimaVirgula = texto.LastIndexOf(',');
        var ultimoPonto = texto.LastIndexOf('.');

        if (ultimaVirgula >= 0 && ultimoPonto >= 0)
            return ultimaVirgula > ultimoPonto ? ',' : '.';

        if (ultimaVirgula >= 0) return ',';
        if (ultimoPonto < 0) return '.';

        var pontoUnico = texto.IndexOf('.') == ultimoPonto;
        var digitosAposPonto = texto.Length - ultimoPonto - 1;

        return pontoUnico && digitosAposPonto != 3 ? '.' : ',';
    }
}