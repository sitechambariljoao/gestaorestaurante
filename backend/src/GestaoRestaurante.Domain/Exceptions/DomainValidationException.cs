namespace GestaoRestaurante.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um aggregate root falha na validação de seus invariantes
/// </summary>
public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }

    public DomainValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}