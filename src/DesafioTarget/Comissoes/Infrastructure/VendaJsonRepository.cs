using DesafioTarget.Comissoes.Domain;
using DesafioTarget.Shared;

namespace DesafioTarget.Comissoes.Infrastructure;

public sealed class VendaJsonRepository : IVendaRepository
{
    private readonly string _caminhoArquivo;

    public VendaJsonRepository(string caminhoArquivo)
    {
        _caminhoArquivo = caminhoArquivo;
    }

    public IReadOnlyList<Venda> ObterTodas()
    {
        var arquivo = JsonArquivo.Ler<VendasArquivoDto>(_caminhoArquivo);

        if (arquivo.Vendas is null)
            throw new DadosInvalidosException("O JSON não possui a lista 'vendas'.");

        var vendas = new List<Venda>(arquivo.Vendas.Count);

        for (var i = 0; i < arquivo.Vendas.Count; i++)
        {
            var posicao = i + 1;
            var item = arquivo.Vendas[i];
            var vendedor = item?.Vendedor;
            var valor = item?.Valor;

            if (vendedor is null || valor is null)
                throw new DadosInvalidosException($"Venda #{posicao}: 'vendedor' e 'valor' são obrigatórios.");

            try
            {
                vendas.Add(new Venda(vendedor, valor.Value));
            }
            catch (DomainException ex)
            {
                throw new DadosInvalidosException($"Venda #{posicao} inválida: {ex.Message}", ex);
            }
        }

        return vendas;
    }
}

internal sealed class VendasArquivoDto
{
    public List<VendaDto?>? Vendas { get; set; }
}

internal sealed class VendaDto
{
    public string? Vendedor { get; set; }
    public decimal? Valor { get; set; }
}