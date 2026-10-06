using DesafioTarget.Estoque.Domain;
using DesafioTarget.Shared;

namespace DesafioTarget.Estoque.Infrastructure;

public static class ProdutoJsonLoader
{
    public static IReadOnlyList<Produto> Carregar(string caminhoArquivo)
    {
        var arquivo = JsonArquivo.Ler<EstoqueArquivoDto>(caminhoArquivo);

        if (arquivo.Estoque is null)
            throw new DadosInvalidosException("O JSON não possui a lista 'estoque'.");

        var produtos = new List<Produto>(arquivo.Estoque.Count);

        for (var i = 0; i < arquivo.Estoque.Count; i++)
        {
            var posicao = i + 1;
            var item = arquivo.Estoque[i];
            var codigo = item?.CodigoProduto;
            var estoque = item?.Estoque;

            if (codigo is null || estoque is null)
                throw new DadosInvalidosException($"Produto #{posicao}: 'codigoProduto' e 'estoque' são obrigatórios.");

            try
            {
                produtos.Add(new Produto(codigo.Value, item?.DescricaoProduto ?? string.Empty, estoque.Value));
            }
            catch (DomainException ex)
            {
                throw new DadosInvalidosException($"Produto #{posicao} inválido: {ex.Message}", ex);
            }
        }

        return produtos;
    }
}

internal sealed class EstoqueArquivoDto
{
    public List<ProdutoDto?>? Estoque { get; set; }
}

internal sealed class ProdutoDto
{
    public int? CodigoProduto { get; set; }
    public string? DescricaoProduto { get; set; }
    public int? Estoque { get; set; }
}