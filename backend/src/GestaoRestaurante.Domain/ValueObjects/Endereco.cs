using System.Text.RegularExpressions;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar endereços completos
/// </summary>
public class Endereco : IEquatable<Endereco>
{
    public Endereco(
        string logradouro,
        string? numero,
        string? complemento,
        string cep,
        string bairro,
        string cidade,
        string estado)
    {
        Logradouro = logradouro?.Trim() ?? throw new ArgumentNullException(nameof(logradouro));
        Numero = numero?.Trim();
        Complemento = complemento?.Trim();
        Cep = cep?.Trim() ?? throw new ArgumentNullException(nameof(cep));
        Bairro = bairro?.Trim() ?? throw new ArgumentNullException(nameof(bairro));
        Cidade = cidade?.Trim() ?? throw new ArgumentNullException(nameof(cidade));
        Estado = estado?.Trim() ?? throw new ArgumentNullException(nameof(estado));

        ValidateProperties();
    }

    public string Logradouro { get; private set; }
    public string? Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string Cep { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }

    /// <summary>
    /// Retorna o endereço completo formatado
    /// </summary>
    public string EnderecoCompleto =>
        $"{Logradouro}" +
        (!string.IsNullOrWhiteSpace(Numero) ? $", {Numero}" : "") +
        (!string.IsNullOrWhiteSpace(Complemento) ? $", {Complemento}" : "") +
        $", {Bairro}, {Cidade} - {Estado}, CEP: {CepFormatado}";

    /// <summary>
    /// Retorna o CEP formatado (12345-678)
    /// </summary>
    public string CepFormatado =>
        Cep.Length == 8 ? $"{Cep.Substring(0, 5)}-{Cep.Substring(5)}" : Cep;

    private void ValidateProperties()
    {
        if (string.IsNullOrWhiteSpace(Logradouro))
            throw new ArgumentException("Logradouro é obrigatório", nameof(Logradouro));

        if (string.IsNullOrWhiteSpace(Cep))
            throw new ArgumentException("CEP é obrigatório", nameof(Cep));

        if (string.IsNullOrWhiteSpace(Bairro))
            throw new ArgumentException("Bairro é obrigatório", nameof(Bairro));

        if (string.IsNullOrWhiteSpace(Cidade))
            throw new ArgumentException("Cidade é obrigatória", nameof(Cidade));

        if (string.IsNullOrWhiteSpace(Estado))
            throw new ArgumentException("Estado é obrigatório", nameof(Estado));

        // Validar CEP usando o padrão das constantes
        var cepPattern = new Regex(ApplicationConstants.ValidationPatterns.CepPattern);
        if (!cepPattern.IsMatch(Cep) && !Regex.IsMatch(Cep, @"^\d{8}$"))
            throw new ValidationException(nameof(Endereco), BusinessRuleMessages.Validation.InvalidCep);

        // Validar Estado (2 caracteres)
        if (Estado.Length != 2)
            throw new ArgumentException("Estado deve conter exatamente 2 caracteres", nameof(Estado));
    }

    // Implementação de IEquatable<Endereco> para Value Object
    public bool Equals(Endereco? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Logradouro.Equals(other.Logradouro, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Numero, other.Numero, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Complemento, other.Complemento, StringComparison.OrdinalIgnoreCase) &&
               Cep.Equals(other.Cep, StringComparison.OrdinalIgnoreCase) &&
               Bairro.Equals(other.Bairro, StringComparison.OrdinalIgnoreCase) &&
               Cidade.Equals(other.Cidade, StringComparison.OrdinalIgnoreCase) &&
               Estado.Equals(other.Estado, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Endereco);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Logradouro.ToLowerInvariant(),
            Numero?.ToLowerInvariant(),
            Complemento?.ToLowerInvariant(),
            Cep.ToLowerInvariant(),
            Bairro.ToLowerInvariant(),
            Cidade.ToLowerInvariant(),
            Estado.ToLowerInvariant());
    }

    public static bool operator ==(Endereco? left, Endereco? right)
    {
        return EqualityComparer<Endereco>.Default.Equals(left, right);
    }

    public static bool operator !=(Endereco? left, Endereco? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return EnderecoCompleto;
    }

    /// <summary>
    /// Método estático para criar um endereço com validação
    /// </summary>
    public static Endereco Criar(
        string logradouro,
        string? numero,
        string? complemento,
        string cep,
        string bairro,
        string cidade,
        string estado)
    {
        return new Endereco(logradouro, numero, complemento, cep, bairro, cidade, estado);
    }

    /// <summary>
    /// Construtor privado para Entity Framework
    /// </summary>
    private Endereco()
    {
        Logradouro = string.Empty;
        Numero = null;
        Cep = string.Empty;
        Bairro = string.Empty;
        Cidade = string.Empty;
        Estado = string.Empty;
    }
}