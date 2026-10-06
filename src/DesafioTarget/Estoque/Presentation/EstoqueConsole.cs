using DesafioTarget.Estoque.Application;
using DesafioTarget.Estoque.Domain;
using DesafioTarget.Estoque.Infrastructure;
using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Presentation;

public static class EstoqueConsole
{
    /// <param name="argumentos">
    /// Opcional: [0] = caminho de outro arquivo de estoque.
    /// </param>
    public static int Executar(LeitorConsole leitor, string[] argumentos)
    {
        var caminho = argumentos.Length > 0
            ? argumentos[0]
            : Path.Combine(AppContext.BaseDirectory, "Estoque", "Data", "estoque.json");

        IProdutoRepository produtos;

        try
        {
            produtos = new ProdutoRepositoryEmMemoria(ProdutoJsonLoader.Carregar(caminho));
        }
        catch (DesafioException ex)
        {
            Console.Error.WriteLine($"Erro: {ex.Message}");
            return 1;
        }

        var movimentacoes = new MovimentacaoRepositoryEmMemoria();
        var servico = new MovimentacaoEstoqueService(produtos, movimentacoes);

        leitor.Escrever("=== Desafio 2 - Movimentação de estoque ===");
        ListarProdutos(leitor, produtos);

        while (true)
        {
            leitor.Escrever();
            leitor.Escrever("1 - Lançar ENTRADA");
            leitor.Escrever("2 - Lançar SAÍDA");
            leitor.Escrever("3 - Listar produtos");
            leitor.Escrever("4 - Histórico de movimentações");
            leitor.Escrever("0 - Voltar");

            var opcao = leitor.LerTexto("Opção: ");

            try
            {
                switch (opcao)
                {
                    case "1":
                        Lancar(leitor, servico, TipoMovimentacao.Entrada);
                        break;
                    case "2":
                        Lancar(leitor, servico, TipoMovimentacao.Saida);
                        break;
                    case "3":
                        ListarProdutos(leitor, produtos);
                        break;
                    case "4":
                        ListarHistorico(leitor, movimentacoes);
                        break;
                    case "0":
                        return 0;
                    default:
                        leitor.Escrever("Opção inválida.");
                        break;
                }
            }
            catch (DesafioException ex)
            {
                Console.Error.WriteLine($"Erro: {ex.Message}");
            }
        }
    }

    private static void Lancar(LeitorConsole leitor, MovimentacaoEstoqueService servico, TipoMovimentacao tipo)
    {
        var codigo = leitor.LerInteiro("Código do produto: ");
        var quantidade = leitor.LerInteiro("Quantidade: ");
        var descricao = leitor.LerTexto("Descrição (Enter para usar a padrão): ");

        var r = servico.Registrar(new RegistrarMovimentacaoCommand(codigo, tipo, quantidade, descricao));

        leitor.Escrever();
        leitor.Escrever($"Movimentação #{r.IdMovimentacao} registrada ({r.Tipo.Descricao()}).");
        leitor.Escrever($"Produto:       {r.CodigoProduto} - {r.DescricaoProduto}");
        leitor.Escrever($"Descrição:     {r.DescricaoMovimentacao}");
        leitor.Escrever($"Quantidade:    {r.Quantidade}");
        leitor.Escrever($"Estoque final: {r.EstoqueFinal}");
    }

    private static void ListarProdutos(LeitorConsole leitor, IProdutoRepository produtos)
    {
        leitor.Escrever();
        leitor.Escrever($"{"Código",-8}{"Produto",-30}{"Estoque",8}");

        foreach (var p in produtos.Listar())
            leitor.Escrever($"{p.Codigo,-8}{p.Descricao,-30}{p.Estoque,8}");
    }

    private static void ListarHistorico(LeitorConsole leitor, IMovimentacaoRepository movimentacoes)
    {
        var itens = movimentacoes.Listar();
        leitor.Escrever();

        if (itens.Count == 0)
        {
            leitor.Escrever("Nenhuma movimentação registrada nesta sessão.");
            return;
        }

        leitor.Escrever($"{"Id",-6}{"Tipo",-9}{"Produto",-9}{"Qtde",6}{"Final",8}  Descrição");

        foreach (var m in itens)
            leitor.Escrever($"{m.Id,-6}{m.Tipo,-9}{m.CodigoProduto,-9}{m.Quantidade,6}{m.EstoqueFinal,8}  {m.Descricao}");
    }
}