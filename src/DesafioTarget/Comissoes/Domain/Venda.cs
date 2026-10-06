using DesafioTarget.Shared;

namespace DesafioTarget.Comissoes.Domain;

public sealed record Venda
{
    public string Vendedor { get; }
    public decimal Valor { get; }

    public Venda(string vendedor, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(vendedor))
            throw new DomainException("O vendedor da venda é obrigatório.");

        if (valor < 0)
            throw new DomainException($"O valor da venda de '{vendedor.Trim()}' não pode ser negativo.");

        Vendedor = vendedor.Trim();
        Valor = valor;
    }
}