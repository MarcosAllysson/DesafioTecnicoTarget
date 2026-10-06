namespace DesafioTarget.Estoque.Domain;

public interface IProdutoRepository
{
    Produto? ObterPorCodigo(int codigo);
    IReadOnlyList<Produto> Listar();
}

public interface IMovimentacaoRepository
{
    long ProximoId();
    void Adicionar(Movimentacao movimentacao);
    IReadOnlyList<Movimentacao> Listar();
}