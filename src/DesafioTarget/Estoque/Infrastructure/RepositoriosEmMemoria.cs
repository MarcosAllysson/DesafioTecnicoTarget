using DesafioTarget.Estoque.Domain;
using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Infrastructure;

public sealed class ProdutoRepositoryEmMemoria : IProdutoRepository
{
    private readonly Dictionary<int, Produto> _produtos = new();

    public ProdutoRepositoryEmMemoria(IEnumerable<Produto> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);

        foreach (var produto in produtos)
        {
            if (!_produtos.TryAdd(produto.Codigo, produto))
                throw new DadosInvalidosException($"Código de produto duplicado: {produto.Codigo}.");
        }
    }

    public Produto? ObterPorCodigo(int codigo) => _produtos.GetValueOrDefault(codigo);

    public IReadOnlyList<Produto> Listar() => _produtos.Values.OrderBy(p => p.Codigo).ToList();
}

public sealed class MovimentacaoRepositoryEmMemoria : IMovimentacaoRepository
{
    private readonly object _trava = new();
    private readonly List<Movimentacao> _movimentacoes = new();
    private long _ultimoId;

    public long ProximoId()
    {
        lock (_trava) return ++_ultimoId;
    }

    public void Adicionar(Movimentacao movimentacao)
    {
        ArgumentNullException.ThrowIfNull(movimentacao);
        lock (_trava) _movimentacoes.Add(movimentacao);
    }

    public IReadOnlyList<Movimentacao> Listar()
    {
        lock (_trava) return _movimentacoes.ToList();
    }
}