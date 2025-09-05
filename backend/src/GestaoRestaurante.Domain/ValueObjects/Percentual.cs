using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar percentuais com validação
/// </summary>
public sealed class Percentual : IEquatable<Percentual>, IComparable<Percentual>
{
    public decimal Value { get; }
    public decimal AsDecimal => Value / 100;
    public string FormattedValue { get; }

    public Percentual(decimal value)
    {
        if (value < 0)
            throw new ValidationException(nameof(Percentual), "Percentual não pode ser negativo");

        if (value > 100)
            throw new ValidationException(nameof(Percentual), "Percentual não pode ser maior que 100%");

        Value = Math.Round(value, 2); // Arredondar para 2 casas decimais
        FormattedValue = $"{Value:F2}%";
    }

    public static Percentual Create(decimal value) => new(value);
    
    public static Percentual Zero() => new(0);
    public static Percentual OneHundred() => new(100);

    public static bool IsValid(decimal value)
    {
        return value >= 0 && value <= 100;
    }

    // Factory methods para cenários comuns
    public static Percentual FromDecimal(decimal decimalValue)
    {
        if (decimalValue < 0 || decimalValue > 1)
            throw new ArgumentException("Valor decimal deve estar entre 0 e 1", nameof(decimalValue));
            
        return new Percentual(decimalValue * 100);
    }

    public static Percentual FromFraction(decimal numerator, decimal denominator)
    {
        if (denominator == 0)
            throw new ArgumentException("Denominador não pode ser zero", nameof(denominator));
            
        if (numerator < 0 || denominator < 0)
            throw new ArgumentException("Numerador e denominador devem ser positivos");
            
        var percentage = (numerator / denominator) * 100;
        return new Percentual(Math.Min(percentage, 100)); // Limitar a 100%
    }

    // Operações matemáticas
    public Percentual Add(Percentual other)
    {
        var result = Value + other.Value;
        return new Percentual(Math.Min(result, 100)); // Limitar a 100%
    }

    public Percentual Subtract(Percentual other)
    {
        var result = Value - other.Value;
        return new Percentual(Math.Max(result, 0)); // Não permitir valores negativos
    }

    public Percentual Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Fator de multiplicação não pode ser negativo", nameof(factor));
            
        var result = Value * factor;
        return new Percentual(Math.Min(result, 100));
    }

    // Aplicar percentual a um valor
    public decimal ApplyTo(decimal value)
    {
        return value * AsDecimal;
    }

    public Moeda ApplyTo(Moeda value)
    {
        var discountAmount = value.Value * AsDecimal;
        return new Moeda(discountAmount, value.Currency);
    }

    public Quantidade ApplyTo(Quantidade quantidade)
    {
        var newValue = quantidade.Value * AsDecimal;
        return new Quantidade(newValue, quantidade.UnidadeMedida);
    }

    // Calcular percentual de um valor em relação a outro
    public static Percentual Of(decimal part, decimal whole)
    {
        if (whole == 0)
            throw new ArgumentException("Valor total não pode ser zero", nameof(whole));
            
        if (part < 0 || whole < 0)
            throw new ArgumentException("Valores devem ser positivos");
            
        var percentage = (part / whole) * 100;
        return new Percentual(Math.Min(percentage, 100));
    }

    // Verificações de categoria
    public bool IsZero() => Value == 0;
    public bool IsOneHundred() => Value == 100;
    public bool IsLow() => Value < 25;
    public bool IsMedium() => Value >= 25 && Value < 75;
    public bool IsHigh() => Value >= 75;

    // Categorizar margem de lucro
    public string GetProfitMarginCategory()
    {
        return Value switch
        {
            < 10 => "Margem Baixa",
            >= 10 and < 30 => "Margem Adequada",
            >= 30 and < 50 => "Margem Boa",
            >= 50 => "Margem Excelente"
        };
    }

    // Conversões implícitas
    public static implicit operator decimal(Percentual percentual) => percentual.Value;
    public static explicit operator Percentual(decimal value) => new(value);

    // Operadores de comparação
    public static bool operator ==(Percentual? left, Percentual? right) => Equals(left, right);
    public static bool operator !=(Percentual? left, Percentual? right) => !Equals(left, right);
    public static bool operator <(Percentual left, Percentual right) => left.CompareTo(right) < 0;
    public static bool operator <=(Percentual left, Percentual right) => left.CompareTo(right) <= 0;
    public static bool operator >(Percentual left, Percentual right) => left.CompareTo(right) > 0;
    public static bool operator >=(Percentual left, Percentual right) => left.CompareTo(right) >= 0;

    // Operadores matemáticos
    public static Percentual operator +(Percentual left, Percentual right) => left.Add(right);
    public static Percentual operator -(Percentual left, Percentual right) => left.Subtract(right);
    public static Percentual operator *(Percentual left, decimal right) => left.Multiply(right);

    // Implementações de interface
    public bool Equals(Percentual? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is Percentual other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(Percentual? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }

    public override string ToString() => FormattedValue;
}