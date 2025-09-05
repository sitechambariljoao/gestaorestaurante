using System.Text.RegularExpressions;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um endereço de email válido
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        ApplicationConstants.ValidationPatterns.EmailPattern, 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(Email), BusinessRuleMessages.Validation.Required.Replace("{0}", "Email"));

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (normalizedValue.Length > ApplicationConstants.FieldLengths.EmailMaxLength)
            throw new ValidationException(nameof(Email), 
                BusinessRuleMessages.Validation.MaxLength
                    .Replace("{0}", "Email")
                    .Replace("{1}", ApplicationConstants.FieldLengths.EmailMaxLength.ToString()));

        if (!EmailRegex.IsMatch(normalizedValue))
            throw new ValidationException(nameof(Email), BusinessRuleMessages.Validation.InvalidEmail);

        Value = normalizedValue;
    }

    public static Email Create(string value) => new(value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalizedValue = value.Trim().ToLowerInvariant();
        return normalizedValue.Length <= ApplicationConstants.FieldLengths.EmailMaxLength && 
               EmailRegex.IsMatch(normalizedValue);
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
}