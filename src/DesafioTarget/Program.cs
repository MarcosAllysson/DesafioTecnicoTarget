using System.Text;
using DesafioTarget.Comissoes.Presentation;
using DesafioTarget.Estoque.Presentation;
using DesafioTarget.Juros.Presentation;
using DesafioTarget.Shared;

TentarConfigurarUtf8();

var leitor = new LeitorConsole(Console.In, Console.Out);

try
{
    return args.Length > 0 ? ExecutarPorArgumento(args, leitor) : ExecutarMenu(leitor);
}
catch (EntradaEncerradaException)
{
    Console.WriteLine();
    return 0;
}

static int ExecutarPorArgumento(string[] args, LeitorConsole leitor)
{
    var argumentos = args
        .Skip(1)
        .ToArray();

    switch (args[0])
    {
        case "1":
            return ComissoesConsole.Executar(leitor, argumentos);
        case "2":
            return EstoqueConsole.Executar(leitor, argumentos);
        case "3":
            return JurosConsole.Executar(leitor, argumentos);
        default:
            Console.Error.WriteLine($"Desafio '{args[0]}' inválido. Use 1, 2 ou 3.");
            return 1;
    }
}

static int ExecutarMenu(LeitorConsole leitor)
{
    while (true)
    {
        leitor.Escrever();
        leitor.Escrever("==== Desafio Técnico ====");
        leitor.Escrever("1 - Comissão de vendedores");
        leitor.Escrever("2 - Movimentação de estoque");
        leitor.Escrever("3 - Juros por atraso");
        leitor.Escrever("0 - Sair");

        switch (leitor.LerTexto("Escolha uma opção: "))
        {
            case "1":
                ComissoesConsole.Executar(leitor, Array.Empty<string>());
                break;
            case "2":
                EstoqueConsole.Executar(leitor, Array.Empty<string>());
                break;
            case "3":
                JurosConsole.Executar(leitor, Array.Empty<string>());
                break;
            case "0":
                return 0;
            default:
                leitor.Escrever("Opção inválida."); break;
        }
    }
}

static void TentarConfigurarUtf8()
{
    try
    {
        Console.OutputEncoding = new UTF8Encoding(false);
    }
    catch (IOException)
    {
        /* sem console associado: segue com o padrão */
    }
}