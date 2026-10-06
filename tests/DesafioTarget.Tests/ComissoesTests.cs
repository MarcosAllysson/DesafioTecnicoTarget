using DesafioTarget.Comissoes.Application;
using DesafioTarget.Comissoes.Domain;
using DesafioTarget.Comissoes.Infrastructure;
using DesafioTarget.Shared;
using Xunit;

namespace DesafioTarget.Tests;

public class PoliticaDeComissaoTests
{
    public static TheoryData<decimal, decimal> Faixas => new()
    {
        { 0m, 0m },
        { 99.99m, 0m },       // abaixo de 100: sem comissão
        { 100m, 1m },         // limite inferior da faixa de 1%
        { 499.99m, 4.9999m },
        { 500m, 25m },        // a partir de 500: 5%
        { 1000m, 50m },
    };

    [Theory]
    [MemberData(nameof(Faixas))]
    public void Calcular_AplicaTaxaCorretaPorFaixa(decimal valor, decimal esperado)
        => Assert.Equal(esperado, PoliticaDeComissao.Calcular(valor));
}

public class VendaTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_VendedorVazio_Lanca(string vendedor)
        => Assert.Throws<DomainException>(() => new Venda(vendedor, 10m));

    [Fact]
    public void Construtor_ValorNegativo_Lanca()
        => Assert.Throws<DomainException>(() => new Venda("Ana", -1m));
}

public class CalcularComissoesServiceTests
{
    private static Venda V(string vendedor, decimal valor) => new(vendedor, valor);

    [Fact]
    public void Calcular_JoaoSilva_RetornaComissaoEsperada()
    {
        var vendas = new[]
        {
            V("João Silva", 1200.50m), V("João Silva", 950.75m), V("João Silva", 1800.00m),
            V("João Silva", 1400.30m), V("João Silva", 1100.90m), V("João Silva", 1550.00m),
            V("João Silva", 1700.80m), V("João Silva", 250.30m), V("João Silva", 480.75m),
            V("João Silva", 320.40m),
        };

        var joao = Assert.Single(CalcularComissoesService.Calcular(vendas));

        Assert.Equal(10, joao.QuantidadeVendas);
        Assert.Equal(495.68m, joao.Comissao);
    }

    [Fact]
    public void Calcular_Carlos_ConsideraLimiteDe500EVendaAbaixoDe500()
    {
        var vendas = new[]
        {
            V("Carlos Oliveira", 800.50m), V("Carlos Oliveira", 1200.00m), V("Carlos Oliveira", 1950.30m),
            V("Carlos Oliveira", 1750.80m), V("Carlos Oliveira", 1300.60m), V("Carlos Oliveira", 300.40m),
            V("Carlos Oliveira", 500.00m), V("Carlos Oliveira", 125.75m),
        };

        var carlos = Assert.Single(CalcularComissoesService.Calcular(vendas));

        Assert.Equal(7928.35m, carlos.TotalVendido);
        Assert.Equal(379.37m, carlos.Comissao);
    }

    [Fact]
    public void Calcular_NomesComCaixaDiferente_AgrupaNoMesmoVendedor()
    {
        var vendas = new[] { V("Ana Lima", 1000m), V("ana lima", 1000m) };

        var ana = Assert.Single(CalcularComissoesService.Calcular(vendas));

        Assert.Equal(2, ana.QuantidadeVendas);
        Assert.Equal(100m, ana.Comissao);
    }

    [Fact]
    public void Calcular_SemVendas_RetornaListaVazia()
        => Assert.Empty(CalcularComissoesService.Calcular(Array.Empty<Venda>()));
}

public class VendaJsonRepositoryTests : IDisposable
{
    private readonly List<string> _temporarios = new();

    private string CriarArquivo(string conteudo)
    {
        var caminho = Path.Combine(Path.GetTempPath(), $"vendas-{Guid.NewGuid():N}.json");
        File.WriteAllText(caminho, conteudo);
        _temporarios.Add(caminho);
        return caminho;
    }

    public void Dispose()
    {
        foreach (var caminho in _temporarios) File.Delete(caminho);
    }

    [Fact]
    public void ObterTodas_JsonValido_RetornaVendas()
    {
        var caminho = CriarArquivo("""{ "vendas": [ { "vendedor": "Ana", "valor": 150.25 } ] }""");

        var venda = Assert.Single(new VendaJsonRepository(caminho).ObterTodas());

        Assert.Equal("Ana", venda.Vendedor);
        Assert.Equal(150.25m, venda.Valor);
    }

    [Fact]
    public void ObterTodas_ArquivoInexistente_Lanca()
        => Assert.Throws<DadosInvalidosException>(
            () => new VendaJsonRepository("nao-existe.json").ObterTodas());

    [Fact]
    public void ObterTodas_JsonMalformado_Lanca()
    {
        var caminho = CriarArquivo("{ isso nao e json");
        Assert.Throws<DadosInvalidosException>(() => new VendaJsonRepository(caminho).ObterTodas());
    }

    [Fact]
    public void ObterTodas_SemListaDeVendas_Lanca()
    {
        var caminho = CriarArquivo("{ }");
        Assert.Throws<DadosInvalidosException>(() => new VendaJsonRepository(caminho).ObterTodas());
    }

    [Fact]
    public void ObterTodas_VendaSemVendedor_Lanca()
    {
        var caminho = CriarArquivo("""{ "vendas": [ { "valor": 10 } ] }""");
        Assert.Throws<DadosInvalidosException>(() => new VendaJsonRepository(caminho).ObterTodas());
    }

    [Fact]
    public void ObterTodas_ValorNegativo_Lanca()
    {
        var caminho = CriarArquivo("""{ "vendas": [ { "vendedor": "Ana", "valor": -5 } ] }""");
        Assert.Throws<DadosInvalidosException>(() => new VendaJsonRepository(caminho).ObterTodas());
    }
}