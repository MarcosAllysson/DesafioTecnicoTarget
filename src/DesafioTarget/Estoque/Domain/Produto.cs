using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Domain;

/// <summary>
/// Raiz de agregado: é o único que altera o próprio saldo, e só se a regra permitir.
/// </summary>
public sealed class Produto
{
    public int Codigo { get; }
    public string Descricao { get; }
    public int Estoque { get; private set; }

    public Produto(int codigo, string descricao, int estoque)
    {
        if (codigo <= 0)
            throw new DomainException("O código do produto deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException($"O produto {codigo} precisa de uma descrição.");

        if (estoque < 0)
            throw new DomainException($"O estoque inicial do produto {codigo} não pode ser negativo.");

        Codigo = codigo;
        Descricao = descricao.Trim();
        Estoque = estoque;
    }

    /// <summary>
    /// Atômico: se alguma regra falhar, lança exceção e o estoque permanece intacto.
    /// </summary>
    public void AplicarMovimentacao(TipoMovimentacao tipo, int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        switch (tipo)
        {
            case TipoMovimentacao.Entrada:
                if (quantidade > int.MaxValue - Estoque)
                    throw new DomainException($"A entrada excede a capacidade máxima de estoque de '{Descricao}'.");

                Estoque += quantidade;
                break;

            case TipoMovimentacao.Saida:
                if (quantidade > Estoque)
                    throw new DomainException(
                        $"Estoque insuficiente para '{Descricao}'. Disponível: {Estoque}, solicitado: {quantidade}.");

                Estoque -= quantidade;
                break;

            default:
                throw new DomainException("Tipo de movimentação inválido.");
        }
    }
}