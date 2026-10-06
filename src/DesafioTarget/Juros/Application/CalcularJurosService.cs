using DesafioTarget.Juros.Domain;

namespace DesafioTarget.Juros.Application;

public sealed class CalcularJurosService
{
    private readonly TimeProvider _relogio;

    // O relógio é injetado para o "hoje" poder ser controlado nos testes.
    public CalcularJurosService(TimeProvider? relogio = null)
    {
        _relogio = relogio ?? TimeProvider.System;
    }

    public CalculoJuros Executar(decimal valor, DateOnly vencimento)
    {
        var hoje = DateOnly.FromDateTime(_relogio.GetLocalNow().DateTime);
        return CalculadoraDeJuros.Calcular(valor, vencimento, hoje);
    }
}