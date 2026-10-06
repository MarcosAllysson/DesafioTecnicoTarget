using DesafioTarget.Comissoes.Domain;

namespace DesafioTarget.Comissoes.Application;

public sealed class CalcularComissoesService
{
    private readonly IVendaRepository _vendas;

    public CalcularComissoesService(IVendaRepository vendas)
    {
        _vendas = vendas ?? throw new ArgumentNullException(nameof(vendas));
    }

    public IReadOnlyList<ResumoComissaoVendedor> Executar() => Calcular(_vendas.ObterTodas());

    public static IReadOnlyList<ResumoComissaoVendedor> Calcular(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);

        return vendas
            .GroupBy(venda => venda.Vendedor, StringComparer.OrdinalIgnoreCase)
            .Select(grupo => new ResumoComissaoVendedor(
                Vendedor: grupo.Key,
                QuantidadeVendas: grupo.Count(),
                TotalVendido: grupo.Sum(venda => venda.Valor),
                Comissao: Math.Round(
                    grupo.Sum(venda => PoliticaDeComissao.Calcular(venda.Valor)),
                    2,
                    MidpointRounding.AwayFromZero)))
            .ToList();
    }
}