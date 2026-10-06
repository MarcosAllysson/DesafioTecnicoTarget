using System.Globalization;
using DesafioTarget.Shared;
using Xunit;

namespace DesafioTarget.Tests;

public class ParserEntradaTests
{
    [Theory]
    [InlineData("1000", "1000")]
    [InlineData("1000,50", "1000.50")]
    [InlineData("1.000,50", "1000.50")]
    [InlineData("1000.50", "1000.50")]
    [InlineData("1,000.50", "1000.50")]
    [InlineData("R$ 1.234,56", "1234.56")]
    [InlineData("1.234", "1234")]
    public void Decimal_AceitaFormatosBrasileirosEInternacionais(string entrada, string esperado)
    {
        Assert.True(ParserEntrada.TentarConverterDecimal(entrada, out var valor));
        Assert.Equal(decimal.Parse(esperado, CultureInfo.InvariantCulture), valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12a")]
    [InlineData("1,234,567")]
    public void Decimal_RejeitaEntradasInvalidas(string entrada)
        => Assert.False(ParserEntrada.TentarConverterDecimal(entrada, out _));

    [Theory]
    [InlineData("15/09/2026", 2026, 9, 15)]
    [InlineData("5/9/2026", 2026, 9, 5)]
    [InlineData("2026-09-15", 2026, 9, 15)]
    public void Data_AceitaFormatosComuns(string entrada, int ano, int mes, int dia)
    {
        Assert.True(ParserEntrada.TentarConverterData(entrada, out var data));
        Assert.Equal(new DateOnly(ano, mes, dia), data);
    }

    [Theory]
    [InlineData("31/02/2026")]
    [InlineData("abc")]
    [InlineData("")]
    public void Data_RejeitaEntradasInvalidas(string entrada)
        => Assert.False(ParserEntrada.TentarConverterData(entrada, out _));
}