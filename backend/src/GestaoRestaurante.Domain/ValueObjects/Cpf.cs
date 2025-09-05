using System.Text.RegularExpressions;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um CPF válido
/// </summary>
public sealed class Cpf : IEquatable<Cpf>
{
    private static readonly Regex CpfRegex = new(
        ApplicationConstants.ValidationPatterns.CpfPattern, 
        RegexOptions.Compiled);

    public string Value { get; }
    public string OnlyDigits { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(Cpf), BusinessRuleMessages.Validation.Required.Replace("{0}", "CPF"));

        var normalizedValue = value.Trim();
        OnlyDigits = Regex.Replace(normalizedValue, @"[^\d]", "");

        if (OnlyDigits.Length != 11)
            throw new ValidationException(nameof(Cpf), BusinessRuleMessages.Validation.InvalidCpf);

        if (!IsValidCpf(OnlyDigits))
            throw new ValidationException(nameof(Cpf), BusinessRuleMessages.Validation.InvalidCpf);

        // Format as XXX.XXX.XXX-XX
        Value = $"{OnlyDigits[..3]}.{OnlyDigits[3..6]}.{OnlyDigits[6..9]}-{OnlyDigits[9..11]}";
    }

    public static Cpf Create(string value) => new(value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var onlyDigits = Regex.Replace(value.Trim(), @"[^\d]", "");
        return onlyDigits.Length == 11 && IsValidCpf(onlyDigits);
    }

    private static bool IsValidCpf(string cpf)
    {
        // Check for known invalid patterns
        var invalidCpfs = new[]
        {
            "00000000000", "11111111111", "22222222222", "33333333333", "44444444444",
            "55555555555", "66666666666", "77777777777", "88888888888", "99999999999"
        };

        if (invalidCpfs.Contains(cpf))
            return false;

        var digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        // First verification digit
        var sum1 = 0;
        for (var i = 0; i < 9; i++)
        {
            sum1 += digits[i] * (10 - i);
        }
        var remainder1 = sum1 % 11;
        var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

        if (digits[9] != digit1)
            return false;

        // Second verification digit
        var sum2 = 0;
        for (var i = 0; i < 10; i++)
        {
            sum2 += digits[i] * (11 - i);
        }
        var remainder2 = sum2 % 11;
        var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

        return digits[10] == digit2;
    }

    public static implicit operator string(Cpf cpf) => cpf.Value;
    public static explicit operator Cpf(string value) => new(value);

    public bool Equals(Cpf? other) => other is not null && OnlyDigits == other.OnlyDigits;

    public override bool Equals(object? obj) => obj is Cpf other && Equals(other);

    public override int GetHashCode() => OnlyDigits.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(Cpf? left, Cpf? right) => Equals(left, right);
    public static bool operator !=(Cpf? left, Cpf? right) => !Equals(left, right);
}