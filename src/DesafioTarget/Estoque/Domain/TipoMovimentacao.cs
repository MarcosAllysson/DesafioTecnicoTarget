using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Domain;

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2
}

public static class TipoMovimentacaoExtensions
{
    public static string Descricao(this TipoMovimentacao tipo) => tipo switch
    {
        TipoMovimentacao.Entrada => "Entrada de mercadoria",
        TipoMovimentacao.Saida => "Saída de mercadoria",
        _ => throw new DomainException("Tipo de movimentação inválido.")
    };
}