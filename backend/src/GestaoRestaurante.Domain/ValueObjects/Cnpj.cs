using System.Text.RegularExpressions;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um CNPJ válido
/// </summary>
public sealed class Cnpj : IEquatable<Cnpj>
{
    private static readonly Regex CnpjRegex = new(
        ApplicationConstants.ValidationPatterns.CnpjPattern, 
        RegexOptions.Compiled);

    public string Value { get; }
    public string OnlyDigits { get; }

    public Cnpj(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(Cnpj), BusinessRuleMessages.Validation.Required.Replace("{0}", "CNPJ"));

        var normalizedValue = value.Trim();
        OnlyDigits = Regex.Replace(normalizedValue, @"[^\d]", "");

        if (OnlyDigits.Length != 14)
            throw new ValidationException(nameof(Cnpj), BusinessRuleMessages.Validation.InvalidCnpj);

        if (!IsValidCnpj(OnlyDigits))
            throw new ValidationException(nameof(Cnpj), BusinessRuleMessages.Validation.InvalidCnpj);

        // Format as XX.XXX.XXX/XXXX-XX
        Value = $"{OnlyDigits[..2]}.{OnlyDigits[2..5]}.{OnlyDigits[5..8]}/{OnlyDigits[8..12]}-{OnlyDigits[12..14]}";
    }

    public static Cnpj Create(string value) => new(value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var onlyDigits = Regex.Replace(value.Trim(), @"[^\d]", "");
        return onlyDigits.Length == 14 && IsValidCnpj(onlyDigits);
    }

    private static bool IsValidCnpj(string cnpj)
    {
        // CNPJ validation algorithm
        var digits = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

        // First verification digit
        var sum1 = 0;
        var weight1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (var i = 0; i < 12; i++)
        {
            sum1 += digits[i] * weight1[i];
        }
        var remainder1 = sum1 % 11;
        var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

        if (digits[12] != digit1)
            return false;

        // Second verification digit
        var sum2 = 0;
        var weight2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (var i = 0; i < 13; i++)
        {
            sum2 += digits[i] * weight2[i];
        }
        var remainder2 = sum2 % 11;
        var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

        return digits[13] == digit2;
    }

    public static implicit operator string(Cnpj cnpj) => cnpj.Value;
    public static explicit operator Cnpj(string value) => new(value);

    public bool Equals(Cnpj? other) => other is not null && OnlyDigits == other.OnlyDigits;

    public override bool Equals(object? obj) => obj is Cnpj other && Equals(other);

    public override int GetHashCode() => OnlyDigits.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(Cnpj? left, Cnpj? right) => Equals(left, right);
    public static bool operator !=(Cnpj? left, Cnpj? right) => !Equals(left, right);
}