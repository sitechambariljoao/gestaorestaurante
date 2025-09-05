using System.Text.RegularExpressions;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um número de telefone válido
/// </summary>
public sealed class Telefone : IEquatable<Telefone>
{
    private static readonly Regex TelefoneRegex = new(
        ApplicationConstants.ValidationPatterns.TelefonePattern, 
        RegexOptions.Compiled);

    public string Value { get; }
    public string OnlyDigits { get; }

    public Telefone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(Telefone), BusinessRuleMessages.Validation.Required.Replace("{0}", "Telefone"));

        var normalizedValue = value.Trim();
        OnlyDigits = Regex.Replace(normalizedValue, @"[^\d]", "");

        if (OnlyDigits.Length < 10 || OnlyDigits.Length > 11)
            throw new ValidationException(nameof(Telefone), BusinessRuleMessages.Validation.InvalidTelefone);

        if (!TelefoneRegex.IsMatch(normalizedValue))
            throw new ValidationException(nameof(Telefone), BusinessRuleMessages.Validation.InvalidTelefone);

        // Format as (XX) XXXXX-XXXX or (XX) XXXX-XXXX
        Value = OnlyDigits.Length == 11 
            ? $"({OnlyDigits[..2]}) {OnlyDigits[2..7]}-{OnlyDigits[7..11]}"
            : $"({OnlyDigits[..2]}) {OnlyDigits[2..6]}-{OnlyDigits[6..10]}";
    }

    public static Telefone Create(string value) => new(value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var onlyDigits = Regex.Replace(value.Trim(), @"[^\d]", "");
        return (onlyDigits.Length == 10 || onlyDigits.Length == 11) && 
               TelefoneRegex.IsMatch(value.Trim());
    }

    public static implicit operator string(Telefone telefone) => telefone.Value;
    public static explicit operator Telefone(string value) => new(value);

    public bool Equals(Telefone? other) => other is not null && OnlyDigits == other.OnlyDigits;

    public override bool Equals(object? obj) => obj is Telefone other && Equals(other);

    public override int GetHashCode() => OnlyDigits.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(Telefone? left, Telefone? right) => Equals(left, right);
    public static bool operator !=(Telefone? left, Telefone? right) => !Equals(left, right);
}