using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Domain;

public sealed class Movimentacao
{
    public long Id { get; }
    public int CodigoProduto { get; }
    public TipoMovimentacao Tipo { get; }
    public string Descricao { get; }
    public int Quantidade { get; }
    public int EstoqueFinal { get; }
    public DateTimeOffset RegistradaEm { get; }

    public Movimentacao(
        long id,
        int codigoProduto,
        TipoMovimentacao tipo,
        string? descricao,
        int quantidade,
        int estoqueFinal,
        DateTimeOffset registradaEm
    )
    {
        if (id <= 0)
            throw new DomainException("O identificador da movimentação deve ser positivo.");

        Id = id;
        CodigoProduto = codigoProduto;
        Tipo = tipo;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? tipo.Descricao() : descricao.Trim();
        Quantidade = quantidade;
        EstoqueFinal = estoqueFinal;
        RegistradaEm = registradaEm;
    }
}