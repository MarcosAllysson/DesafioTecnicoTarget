using DesafioTarget.Estoque.Application;
using DesafioTarget.Estoque.Domain;
using DesafioTarget.Estoque.Infrastructure;
using DesafioTarget.Shared;
using Xunit;

namespace DesafioTarget.Tests;

public class EstoqueTests
{
    private static MovimentacaoEstoqueService CriarServico()
    {
        var produtos = new ProdutoRepositoryEmMemoria(new[]
        {
            new Produto(101, "Caneta Azul", 150),
            new Produto(102, "Caderno Universitário", 75),
        });

        return new MovimentacaoEstoqueService(produtos, new MovimentacaoRepositoryEmMemoria());
    }

    private static RegistrarMovimentacaoCommand Cmd(int codigo, TipoMovimentacao tipo, int qtde, string? desc = null)
        => new(codigo, tipo, qtde, desc);

    [Fact]
    public void Entrada_AumentaEstoqueERetornaSaldoFinal()
    {
        var resultado = CriarServico().Registrar(Cmd(101, TipoMovimentacao.Entrada, 50, "Compra"));

        Assert.Equal(200, resultado.EstoqueFinal);
        Assert.Equal("Compra", resultado.DescricaoMovimentacao);
    }

    [Fact]
    public void Saida_DiminuiEstoqueERetornaSaldoFinal()
    {
        var resultado = CriarServico().Registrar(Cmd(101, TipoMovimentacao.Saida, 30));

        Assert.Equal(120, resultado.EstoqueFinal);
    }

    [Fact]
    public void Saida_MaiorQueEstoque_LancaEMantemSaldo()
    {
        var servico = CriarServico();

        Assert.Throws<DomainException>(() => servico.Registrar(Cmd(101, TipoMovimentacao.Saida, 151)));

        // O saldo continua 150: tirar tudo agora deve funcionar e zerar.
        Assert.Equal(0, servico.Registrar(Cmd(101, TipoMovimentacao.Saida, 150)).EstoqueFinal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Quantidade_ZeroOuNegativa_Lanca(int quantidade)
        => Assert.Throws<DomainException>(
            () => CriarServico().Registrar(Cmd(101, TipoMovimentacao.Entrada, quantidade)));

    [Fact]
    public void ProdutoInexistente_Lanca()
        => Assert.Throws<DomainException>(
            () => CriarServico().Registrar(Cmd(999, TipoMovimentacao.Entrada, 1)));

    [Fact]
    public void TipoInvalido_Lanca()
        => Assert.Throws<DomainException>(
            () => CriarServico().Registrar(Cmd(101, (TipoMovimentacao)99, 1)));

    [Fact]
    public void Ids_SaoUnicosSequenciaisESemBuracoAposFalha()
    {
        var servico = CriarServico();

        var primeira = servico.Registrar(Cmd(101, TipoMovimentacao.Entrada, 1));
        Assert.Throws<DomainException>(() => servico.Registrar(Cmd(101, TipoMovimentacao.Saida, 9999)));
        var segunda = servico.Registrar(Cmd(102, TipoMovimentacao.Saida, 1));

        Assert.Equal(1, primeira.IdMovimentacao);
        Assert.Equal(2, segunda.IdMovimentacao);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Descricao_Vazia_UsaDescricaoPadraoDoTipo(string? descricao)
    {
        var resultado = CriarServico().Registrar(Cmd(101, TipoMovimentacao.Saida, 1, descricao));

        Assert.Equal("Saída de mercadoria", resultado.DescricaoMovimentacao);
    }

    [Fact]
    public void Entrada_QueEstouraLimiteDoInteiro_Lanca()
    {
        var produtos = new ProdutoRepositoryEmMemoria(new[] { new Produto(1, "Cheio", int.MaxValue) });
        var servico = new MovimentacaoEstoqueService(produtos, new MovimentacaoRepositoryEmMemoria());

        Assert.Throws<DomainException>(() => servico.Registrar(Cmd(1, TipoMovimentacao.Entrada, 1)));
    }

    [Fact]
    public void Repositorio_CodigoDuplicado_Lanca()
        => Assert.Throws<DadosInvalidosException>(() => new ProdutoRepositoryEmMemoria(new[]
        {
            new Produto(1, "A", 1),
            new Produto(1, "B", 1),
        }));
}