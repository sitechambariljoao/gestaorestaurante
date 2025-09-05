using System.Text.RegularExpressions;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um CEP válido
/// </summary>
public sealed class Cep : IEquatable<Cep>
{
    private static readonly Regex CepRegex = new(
        ApplicationConstants.ValidationPatterns.CepPattern, 
        RegexOptions.Compiled);

    public string Value { get; }
    public string OnlyDigits { get; }

    public Cep(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(Cep), BusinessRuleMessages.Validation.Required.Replace("{0}", "CEP"));

        var normalizedValue = value.Trim();
        OnlyDigits = Regex.Replace(normalizedValue, @"[^\d]", "");

        if (OnlyDigits.Length != 8)
            throw new ValidationException(nameof(Cep), BusinessRuleMessages.Validation.InvalidCep);

        // Verificar se todos os dígitos são iguais (CEPs inválidos)
        if (OnlyDigits.All(c => c == OnlyDigits[0]))
            throw new ValidationException(nameof(Cep), BusinessRuleMessages.Validation.InvalidCep);

        // Format as XXXXX-XXX
        Value = $"{OnlyDigits[..5]}-{OnlyDigits[5..8]}";
    }

    public static Cep Create(string value) => new(value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var onlyDigits = Regex.Replace(value.Trim(), @"[^\d]", "");
        
        if (onlyDigits.Length != 8)
            return false;

        // Verificar se todos os dígitos são iguais
        if (onlyDigits.All(c => c == onlyDigits[0]))
            return false;

        return CepRegex.IsMatch(value.Trim()) || Regex.IsMatch(onlyDigits, @"^\d{8}$");
    }

    public static implicit operator string(Cep cep) => cep.Value;
    public static explicit operator Cep(string value) => new(value);

    public bool Equals(Cep? other) => other is not null && OnlyDigits == other.OnlyDigits;

    public override bool Equals(object? obj) => obj is Cep other && Equals(other);

    public override int GetHashCode() => OnlyDigits.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(Cep? left, Cep? right) => Equals(left, right);
    public static bool operator !=(Cep? left, Cep? right) => !Equals(left, right);
}