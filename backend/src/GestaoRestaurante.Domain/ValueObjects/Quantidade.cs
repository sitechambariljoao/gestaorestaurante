using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.ValueObjects;

/// <summary>
/// Value Object para representar quantidades com unidade de medida
/// </summary>
public sealed class Quantidade : IEquatable<Quantidade>, IComparable<Quantidade>
{
    public decimal Value { get; }
    public string UnidadeMedida { get; }
    public string FormattedValue { get; }

    public Quantidade(decimal value, string unidadeMedida)
    {
        if (value < 0)
            throw new ValidationException(nameof(Quantidade), "Quantidade não pode ser negativa");

        if (value > ApplicationConstants.BusinessRules.EstoqueMaxQuantity)
            throw new ValidationException(nameof(Quantidade), 
                $"Quantidade não pode exceder {ApplicationConstants.BusinessRules.EstoqueMaxQuantity}");

        if (string.IsNullOrWhiteSpace(unidadeMedida))
            throw new ValidationException(nameof(Quantidade), "Unidade de medida é obrigatória");

        if (unidadeMedida.Length > 10)
            throw new ValidationException(nameof(Quantidade), 
                $"Unidade de medida não pode exceder 10 caracteres");

        Value = Math.Round(value, 3); // Arredondar para 3 casas decimais
        UnidadeMedida = unidadeMedida.Trim().ToUpperInvariant();
        FormattedValue = $"{Value:F3} {UnidadeMedida}";
    }

    public static Quantidade Create(decimal value, string unidadeMedida) => new(value, unidadeMedida);
    
    public static Quantidade Zero(string unidadeMedida) => new(0, unidadeMedida);

    public static bool IsValid(decimal value, string unidadeMedida)
    {
        return value >= 0 && 
               value <= 999999.999m && 
               !string.IsNullOrWhiteSpace(unidadeMedida) &&
               unidadeMedida.Length <= 10;
    }

    // Operações matemáticas
    public Quantidade Add(Quantidade other)
    {
        ValidateSameUnit(other);
        return new Quantidade(Value + other.Value, UnidadeMedida);
    }

    public Quantidade Subtract(Quantidade other)
    {
        ValidateSameUnit(other);
        var result = Value - other.Value;
        return new Quantidade(result >= 0 ? result : 0, UnidadeMedida); // Não permitir valores negativos
    }

    public Quantidade Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Fator de multiplicação não pode ser negativo", nameof(factor));
            
        return new Quantidade(Value * factor, UnidadeMedida);
    }

    public Quantidade Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new ArgumentException("Divisor deve ser maior que zero", nameof(divisor));
            
        return new Quantidade(Value / divisor, UnidadeMedida);
    }

    // Verificações de estoque
    public bool IsZero() => Value == 0;
    public bool IsPositive() => Value > 0;
    public bool IsSufficient(Quantidade required)
    {
        ValidateSameUnit(required);
        return Value >= required.Value;
    }

    // Conversões de unidade (exemplos básicos)
    public Quantidade ConvertToKg()
    {
        return UnidadeMedida switch
        {
            "G" => new Quantidade(Value / 1000, "KG"),
            "LB" => new Quantidade(Value * 0.453592m, "KG"),
            "KG" => this,
            _ => throw new InvalidOperationException($"Conversão de {UnidadeMedida} para KG não suportada")
        };
    }

    public Quantidade ConvertToLitros()
    {
        return UnidadeMedida switch
        {
            "ML" => new Quantidade(Value / 1000, "L"),
            "L" => this,
            _ => throw new InvalidOperationException($"Conversão de {UnidadeMedida} para L não suportada")
        };
    }

    private void ValidateSameUnit(Quantidade other)
    {
        if (UnidadeMedida != other.UnidadeMedida)
            throw new InvalidOperationException($"Operação entre unidades diferentes: {UnidadeMedida} e {other.UnidadeMedida}");
    }

    // Conversões implícitas
    public static implicit operator decimal(Quantidade quantidade) => quantidade.Value;
    public static explicit operator Quantidade(decimal value) => new(value, "UN");

    // Operadores de comparação
    public static bool operator ==(Quantidade? left, Quantidade? right) => Equals(left, right);
    public static bool operator !=(Quantidade? left, Quantidade? right) => !Equals(left, right);
    public static bool operator <(Quantidade left, Quantidade right) => left.CompareTo(right) < 0;
    public static bool operator <=(Quantidade left, Quantidade right) => left.CompareTo(right) <= 0;
    public static bool operator >(Quantidade left, Quantidade right) => left.CompareTo(right) > 0;
    public static bool operator >=(Quantidade left, Quantidade right) => left.CompareTo(right) >= 0;

    // Operadores matemáticos
    public static Quantidade operator +(Quantidade left, Quantidade right) => left.Add(right);
    public static Quantidade operator -(Quantidade left, Quantidade right) => left.Subtract(right);
    public static Quantidade operator *(Quantidade left, decimal right) => left.Multiply(right);
    public static Quantidade operator /(Quantidade left, decimal right) => left.Divide(right);

    // Implementações de interface
    public bool Equals(Quantidade? other)
    {
        return other is not null && 
               Value == other.Value && 
               UnidadeMedida == other.UnidadeMedida;
    }

    public override bool Equals(object? obj) => obj is Quantidade other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, UnidadeMedida);

    public int CompareTo(Quantidade? other)
    {
        if (other is null) return 1;
        ValidateSameUnit(other);
        return Value.CompareTo(other.Value);
    }

    public override string ToString() => FormattedValue;
}