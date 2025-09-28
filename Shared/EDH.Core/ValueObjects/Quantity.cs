using EDH.Core.Exceptions;

namespace EDH.Core.ValueObjects;

public sealed record Quantity
{
    public required int Value { get; init; }

    private Quantity() { }
    
    public static Quantity Zero => new()
    {
        Value = 0
    };

    public static Quantity FromValue(int value)
    {
        if (value < 0)
            throw new InvalidQuantityException();

        return new Quantity
        {
            Value = value
        };
    }

    public Quantity Add(Quantity? quantity)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        
        if (quantity.Value > 0 && Value > Int32.MaxValue - quantity.Value)
            throw new OverflowException();

        
        return new Quantity
        {
            Value = Value + quantity.Value
        };
    }
    
    
    public Quantity Subtract(Quantity? quantity)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        
        if (Value - quantity.Value < 0)
            throw new InvalidQuantityException();

        return new Quantity
        {
            Value = Value - quantity.Value
        };
    }
    
    public bool IsZero => Value == 0;
    
    public bool IsGreaterThan(int value) => Value > value;
    
    public bool IsLessThan(int value) => Value < value;

    public override string ToString() => Value.ToString();
    
    public static implicit operator int(Quantity quantity) => quantity.Value;
    
    public static implicit operator int?(Quantity? quantity) => quantity?.Value;
}