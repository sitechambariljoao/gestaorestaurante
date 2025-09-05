using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar valores monetários com validação
/// </summary>
public sealed class Moeda : IEquatable<Moeda>, IComparable<Moeda>
{
    public decimal Value { get; }
    public string FormattedValue { get; }
    public string Currency { get; }

    public Moeda(decimal value, string currency = "BRL")
    {
        if (value < 0)
            throw new ValidationException(nameof(Moeda), "Valor monetário não pode ser negativo");

        if (value > ApplicationConstants.BusinessRules.ProdutoMaxPrice)
            throw new ValidationException(nameof(Moeda), 
                $"Valor monetário não pode exceder {ApplicationConstants.BusinessRules.ProdutoMaxPrice:C}");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ValidationException(nameof(Moeda), "Moeda é obrigatória");

        Value = Math.Round(value, 2); // Arredondar para 2 casas decimais
        Currency = currency.Trim().ToUpperInvariant();
        FormattedValue = FormatCurrency(Value, Currency);
    }

    public static Moeda Create(decimal value, string currency = "BRL") => new(value, currency);
    
    public static Moeda Zero(string currency = "BRL") => new(0, currency);

    public static bool IsValid(decimal value, string currency = "BRL")
    {
        return value >= 0 && 
               value <= ApplicationConstants.BusinessRules.ProdutoMaxPrice && 
               !string.IsNullOrWhiteSpace(currency);
    }

    // Operações matemáticas
    public Moeda Add(Moeda other)
    {
        ValidateSameCurrency(other);
        return new Moeda(Value + other.Value, Currency);
    }

    public Moeda Subtract(Moeda other)
    {
        ValidateSameCurrency(other);
        var result = Value - other.Value;
        return new Moeda(result >= 0 ? result : 0, Currency); // Não permitir valores negativos
    }

    public Moeda Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Fator de multiplicação não pode ser negativo", nameof(factor));
            
        return new Moeda(Value * factor, Currency);
    }

    public Moeda Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new ArgumentException("Divisor deve ser maior que zero", nameof(divisor));
            
        return new Moeda(Value / divisor, Currency);
    }

    // Aplicar desconto percentual
    public Moeda ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Desconto deve estar entre 0% e 100%", nameof(discountPercentage));
            
        var discountAmount = Value * (discountPercentage / 100);
        return new Moeda(Value - discountAmount, Currency);
    }

    // Aplicar acréscimo percentual
    public Moeda ApplyMarkup(decimal markupPercentage)
    {
        if (markupPercentage < 0)
            throw new ArgumentException("Acréscimo não pode ser negativo", nameof(markupPercentage));
            
        var markupAmount = Value * (markupPercentage / 100);
        return new Moeda(Value + markupAmount, Currency);
    }

    private void ValidateSameCurrency(Moeda other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Operação entre moedas diferentes: {Currency} e {other.Currency}");
    }

    private static string FormatCurrency(decimal value, string currency)
    {
        return currency switch
        {
            "BRL" => value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
            "USD" => value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US")),
            "EUR" => value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("fr-FR")),
            _ => $"{value:F2} {currency}"
        };
    }

    // Conversões implícitas
    public static implicit operator decimal(Moeda moeda) => moeda.Value;
    public static explicit operator Moeda(decimal value) => new(value);

    // Operadores de comparação
    public static bool operator ==(Moeda? left, Moeda? right) => Equals(left, right);
    public static bool operator !=(Moeda? left, Moeda? right) => !Equals(left, right);
    public static bool operator <(Moeda left, Moeda right) => left.CompareTo(right) < 0;
    public static bool operator <=(Moeda left, Moeda right) => left.CompareTo(right) <= 0;
    public static bool operator >(Moeda left, Moeda right) => left.CompareTo(right) > 0;
    public static bool operator >=(Moeda left, Moeda right) => left.CompareTo(right) >= 0;

    // Operadores matemáticos
    public static Moeda operator +(Moeda left, Moeda right) => left.Add(right);
    public static Moeda operator -(Moeda left, Moeda right) => left.Subtract(right);
    public static Moeda operator *(Moeda left, decimal right) => left.Multiply(right);
    public static Moeda operator /(Moeda left, decimal right) => left.Divide(right);

    // Implementações de interface
    public bool Equals(Moeda? other)
    {
        return other is not null && 
               Value == other.Value && 
               Currency == other.Currency;
    }

    public override bool Equals(object? obj) => obj is Moeda other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, Currency);

    public int CompareTo(Moeda? other)
    {
        if (other is null) return 1;
        ValidateSameCurrency(other);
        return Value.CompareTo(other.Value);
    }

    public override string ToString() => FormattedValue;
}