using DesafioTarget.Estoque.Domain;
using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Application;

public sealed record RegistrarMovimentacaoCommand(
    int CodigoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string? Descricao
);

public sealed record ResultadoMovimentacao(
    long IdMovimentacao,
    int CodigoProduto,
    string DescricaoProduto,
    TipoMovimentacao Tipo,
    string DescricaoMovimentacao,
    int Quantidade,
    int EstoqueFinal
);

public sealed class MovimentacaoEstoqueService
{
    private readonly IProdutoRepository _produtos;
    private readonly IMovimentacaoRepository _movimentacoes;
    private readonly TimeProvider _relogio;

    public MovimentacaoEstoqueService(
        IProdutoRepository produtos,
        IMovimentacaoRepository movimentacoes,
        TimeProvider? relogio = null
    )
    {
        _produtos = produtos ?? throw new ArgumentNullException(nameof(produtos));
        _movimentacoes = movimentacoes ?? throw new ArgumentNullException(nameof(movimentacoes));
        _relogio = relogio ?? TimeProvider.System;
    }

    public ResultadoMovimentacao Registrar(RegistrarMovimentacaoCommand comando)
    {
        ArgumentNullException.ThrowIfNull(comando);

        var produto = _produtos.ObterPorCodigo(comando.CodigoProduto)
            ?? throw new DomainException($"Produto {comando.CodigoProduto} não encontrado.");

        produto.AplicarMovimentacao(comando.Tipo, comando.Quantidade);

        var movimentacao = new Movimentacao(
            _movimentacoes.ProximoId(),
            produto.Codigo,
            comando.Tipo,
            comando.Descricao,
            comando.Quantidade,
            produto.Estoque,
            _relogio.GetLocalNow());

        _movimentacoes.Adicionar(movimentacao);

        return new ResultadoMovimentacao(
            movimentacao.Id,
            produto.Codigo,
            produto.Descricao,
            movimentacao.Tipo,
            movimentacao.Descricao,
            movimentacao.Quantidade,
            movimentacao.EstoqueFinal);
    }
}