using DesafioTarget.Comissoes.Application;
using DesafioTarget.Comissoes.Domain;
using DesafioTarget.Comissoes.Infrastructure;
using DesafioTarget.Shared;

namespace DesafioTarget.Comissoes.Presentation;

public static class ComissoesConsole
{
    /// <param name="argumentos">
    /// Opcional: [0] = caminho de outro arquivo de vendas.
    /// </param>
    public static int Executar(LeitorConsole leitor, string[] argumentos)
    {
        var caminho = argumentos.Length > 0
            ? argumentos[0]
            : Path.Combine(AppContext.BaseDirectory, "Comissoes", "Data", "vendas.json");

        try
        {
            var servico = new CalcularComissoesService(new VendaJsonRepository(caminho));
            Imprimir(leitor, servico.Executar());
            return 0;
        }
        catch (DesafioException ex)
        {
            Console.Error.WriteLine($"Erro: {ex.Message}");
            return 1;
        }
    }

    private static void Imprimir(LeitorConsole leitor, IReadOnlyList<ResumoComissaoVendedor> resumos)
    {
        leitor.Escrever("=== Desafio 1 - Comissão por vendedor ===");

        if (resumos.Count == 0)
        {
            leitor.Escrever("Nenhuma venda encontrada.");
            return;
        }

        leitor.Escrever($"{"Vendedor",-20}{"Vendas",8}{"Total vendido",20}{"Comissão",16}");

        foreach (var r in resumos)
        {
            leitor.Escrever(
                $"{r.Vendedor,-20}{r.QuantidadeVendas,8}{Formatacao.Moeda(r.TotalVendido),20}{Formatacao.Moeda(r.Comissao),16}"
            );
        }

        leitor.Escrever(new string('-', 64));

        leitor.Escrever(
            $"{"TOTAL",-20}{resumos.Sum(r => r.QuantidadeVendas),8}" +
            $"{Formatacao.Moeda(resumos.Sum(r => r.TotalVendido)),20}" +
            $"{Formatacao.Moeda(resumos.Sum(r => r.Comissao)),16}"
        );
    }
}