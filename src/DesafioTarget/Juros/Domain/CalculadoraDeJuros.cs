using DesafioTarget.Shared;

namespace DesafioTarget.Juros.Domain;

public sealed record CalculoJuros(
    decimal ValorOriginal,
    DateOnly DataVencimento,
    DateOnly DataCalculo,
    int DiasEmAtraso,
    decimal Juros,
    decimal ValorAtualizado
);

/// <summary>
/// Juros simples de 2,5% ao dia sobre o valor original, contando os dias corridos
/// entre o vencimento e a data do cálculo. Sem atraso, não há juros.
/// </summary>
public static class CalculadoraDeJuros
{
    public const decimal TaxaDiaria = 0.025m;

    public static CalculoJuros Calcular(decimal valor, DateOnly vencimento, DateOnly dataCalculo)
    {
        if (valor <= 0)
            throw new DomainException("O valor deve ser maior que zero.");

        var diasEmAtraso = Math.Max(0, dataCalculo.DayNumber - vencimento.DayNumber);

        try
        {
            var juros = Math.Round(valor * TaxaDiaria * diasEmAtraso, 2, MidpointRounding.AwayFromZero);

            return new CalculoJuros(
                valor,
                vencimento,
                dataCalculo,
                diasEmAtraso,
                juros,
                valor + juros
            );
        }
        catch (OverflowException)
        {
            throw new DomainException("O valor ou o período de atraso é grande demais para ser calculado.");
        }
    }
}