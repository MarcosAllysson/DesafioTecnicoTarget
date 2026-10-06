namespace DesafioTarget.Comissoes.Domain;

public sealed record ResumoComissaoVendedor(
    string Vendedor,
    int QuantidadeVendas,
    decimal TotalVendido,
    decimal Comissao
);