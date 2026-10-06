namespace DesafioTarget.Comissoes.Domain;

public interface IVendaRepository
{
    IReadOnlyList<Venda> ObterTodas();
}