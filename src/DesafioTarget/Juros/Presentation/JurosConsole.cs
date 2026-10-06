using DesafioTarget.Juros.Application;
using DesafioTarget.Juros.Domain;
using DesafioTarget.Shared;

namespace DesafioTarget.Juros.Presentation;

public static class JurosConsole
{
    /// <param name="argumentos">
    /// Opcional: [0] = valor, [1] = vencimento. 
    /// O que faltar é perguntado.
    /// </param>
    public static int Executar(LeitorConsole leitor, string[] argumentos)
    {
        try
        {
            leitor.Escrever("=== Desafio 3 - Juros por atraso ===");

            var valor = argumentos.Length > 0
                ? ConverterValor(argumentos[0])
                : leitor.LerDecimal("Valor (R$): ");

            var vencimento = argumentos.Length > 1
                ? ConverterData(argumentos[1])
                : leitor.LerData("Data de vencimento (dd/MM/aaaa): ");

            Imprimir(leitor, new CalcularJurosService().Executar(valor, vencimento));
            return 0;
        }
        catch (DesafioException ex)
        {
            Console.Error.WriteLine($"Erro: {ex.Message}");
            return 1;
        }
    }

    private static decimal ConverterValor(string texto)
        => ParserEntrada.TentarConverterDecimal(texto, out var valor)
            ? valor
            : throw new DadosInvalidosException($"Valor inválido: '{texto}'.");

    private static DateOnly ConverterData(string texto)
        => ParserEntrada.TentarConverterData(texto, out var data)
            ? data
            : throw new DadosInvalidosException($"Data inválida: '{texto}'. Use dd/MM/aaaa.");

    private static void Imprimir(LeitorConsole leitor, CalculoJuros r)
    {
        leitor.Escrever($"Valor original:   {Formatacao.Moeda(r.ValorOriginal)}");
        leitor.Escrever($"Vencimento:       {Formatacao.Data(r.DataVencimento)}");
        leitor.Escrever($"Data do cálculo:  {Formatacao.Data(r.DataCalculo)}");
        leitor.Escrever($"Dias em atraso:   {r.DiasEmAtraso}");
        leitor.Escrever($"Taxa:             {Formatacao.Percentual(CalculadoraDeJuros.TaxaDiaria)} ao dia (juros simples)");
        leitor.Escrever($"Juros:            {Formatacao.Moeda(r.Juros)}");
        leitor.Escrever($"Valor atualizado: {Formatacao.Moeda(r.ValorAtualizado)}");

        if (r.DiasEmAtraso == 0)
            leitor.Escrever("Sem atraso: nenhum juro devido.");
    }
}