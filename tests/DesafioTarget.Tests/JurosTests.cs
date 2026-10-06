using DesafioTarget.Juros.Application;
using DesafioTarget.Juros.Domain;
using DesafioTarget.Shared;
using Xunit;

namespace DesafioTarget.Tests;

public class CalculadoraDeJurosTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 6);

    [Fact]
    public void Atraso_DeDezDias_Cobra25PorCentoSimples()
    {
        var r = CalculadoraDeJuros.Calcular(1000m, Hoje.AddDays(-10), Hoje);

        Assert.Equal(10, r.DiasEmAtraso);
        Assert.Equal(250m, r.Juros);
        Assert.Equal(1250m, r.ValorAtualizado);
    }

    [Fact]
    public void VenceHoje_NaoCobraJuros()
        => Assert.Equal(0m, CalculadoraDeJuros.Calcular(1000m, Hoje, Hoje).Juros);

    [Fact]
    public void AindaNaoVenceu_NaoCobraJuros()
    {
        var r = CalculadoraDeJuros.Calcular(1000m, Hoje.AddDays(5), Hoje);

        Assert.Equal(0, r.DiasEmAtraso);
        Assert.Equal(0m, r.Juros);
        Assert.Equal(1000m, r.ValorAtualizado);
    }

    [Fact]
    public void Juros_SaoArredondadosParaDuasCasas()
        // 100,10 * 0,025 * 3 = 7,5075 -> 7,51
        => Assert.Equal(7.51m, CalculadoraDeJuros.Calcular(100.10m, Hoje.AddDays(-3), Hoje).Juros);

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Valor_ZeroOuNegativo_Lanca(int valor)
        => Assert.Throws<DomainException>(() => CalculadoraDeJuros.Calcular(valor, Hoje, Hoje));

    [Fact]
    public void ValorEPeriodoGigantes_LancaEmVezDeEstourar()
        => Assert.Throws<DomainException>(
            () => CalculadoraDeJuros.Calcular(decimal.MaxValue, DateOnly.MinValue, DateOnly.MaxValue));
}

public class CalcularJurosServiceTests
{
    private sealed class RelogioFixo : TimeProvider
    {
        private readonly DateTimeOffset _agora;
        public RelogioFixo(DateTimeOffset agora) => _agora = agora;
        public override DateTimeOffset GetUtcNow() => _agora;
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    [Fact]
    public void Executar_UsaADataDeHojeDoRelogio()
    {
        var relogio = new RelogioFixo(new DateTimeOffset(2026, 10, 6, 12, 0, 0, TimeSpan.Zero));

        var r = new CalcularJurosService(relogio).Executar(200m, new DateOnly(2026, 10, 1));

        Assert.Equal(5, r.DiasEmAtraso);
        Assert.Equal(25m, r.Juros);
        Assert.Equal(new DateOnly(2026, 10, 6), r.DataCalculo);
    }
}