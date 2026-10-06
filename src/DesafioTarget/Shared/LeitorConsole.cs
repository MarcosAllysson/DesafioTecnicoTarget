namespace DesafioTarget.Shared;

/// <summary>
/// Entrada/saída de console com validação e re-pergunta. 
/// Facilita testes e evita loop infinito no EOF.
/// </summary>
public sealed class LeitorConsole
{
    private delegate bool TentarConverter<T>(string? texto, out T valor);

    private readonly TextReader _entrada;
    private readonly TextWriter _saida;

    public LeitorConsole(TextReader entrada, TextWriter saida)
    {
        _entrada = entrada ?? throw new ArgumentNullException(nameof(entrada));
        _saida = saida ?? throw new ArgumentNullException(nameof(saida));
    }

    public void Escrever(string texto = "") => _saida.WriteLine(texto);

    public string LerTexto(string prompt)
    {
        _saida.Write(prompt);
        var linha = _entrada.ReadLine();
        if (linha is null) throw new EntradaEncerradaException();

        return linha.Trim();
    }

    public int LerInteiro(string prompt)
        => LerValido<int>(prompt, ParserEntrada.TentarConverterInteiro, "Informe um número inteiro válido.");

    public decimal LerDecimal(string prompt)
        => LerValido<decimal>(prompt, ParserEntrada.TentarConverterDecimal, "Informe um valor numérico válido (ex.: 1500,50).");

    public DateOnly LerData(string prompt)
        => LerValido<DateOnly>(prompt, ParserEntrada.TentarConverterData, "Informe uma data válida (ex.: 15/09/2026).");

    private T LerValido<T>(string prompt, TentarConverter<T> conversor, string mensagemErro)
    {
        while (true)
        {
            var texto = LerTexto(prompt);

            if (conversor(texto, out var valor)) return valor;
            _saida.WriteLine(mensagemErro);
        }
    }
}