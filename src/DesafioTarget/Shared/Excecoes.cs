namespace DesafioTarget.Shared;

public abstract class DesafioException : Exception
{
    protected DesafioException(string message, Exception? inner = null) : base(message, inner) { }
}

public sealed class DomainException : DesafioException
{
    public DomainException(string message) : base(message) { }
}

public sealed class DadosInvalidosException : DesafioException
{
    public DadosInvalidosException(string message, Exception? inner = null) : base(message, inner) { }
}

public sealed class EntradaEncerradaException : Exception
{
    public EntradaEncerradaException() : base("A entrada de dados foi encerrada.") { }
}