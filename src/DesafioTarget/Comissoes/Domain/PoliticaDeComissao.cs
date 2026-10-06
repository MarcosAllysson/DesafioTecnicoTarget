namespace DesafioTarget.Comissoes.Domain;

/// <summary>
/// Regra de comissão por venda:
/// abaixo de R$ 100 = sem comissão; de R$ 100 até R$ 499,99 = 1%; a partir de R$ 500 = 5%.
/// Devolve o valor sem arredondar; o arredondamento é feito sobre o total do vendedor.
/// </summary>
public static class PoliticaDeComissao
{
    private const decimal ValorMinimoParaComissao = 100m;
    private const decimal ValorInicioFaixaAlta = 500m;
    private const decimal TaxaFaixaBaixa = 0.01m;
    private const decimal TaxaFaixaAlta = 0.05m;

    public static decimal Calcular(decimal valorVenda)
    {
        if (valorVenda < ValorMinimoParaComissao) return 0m;
        if (valorVenda < ValorInicioFaixaAlta) return valorVenda * TaxaFaixaBaixa;
        return valorVenda * TaxaFaixaAlta;
    }
}