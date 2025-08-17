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

    public Quantity Add(Quantity quantity)
    {
        if (Value + quantity < 0)
            throw new InvalidQuantityException();
        
        return new Quantity
        {
            Value = Value + quantity
        };
    }
    
    public Quantity Subtract(Quantity quantity)
    {
        if (Value - quantity < 0)
            throw new InvalidQuantityException();
        
        return new Quantity
        {
            Value = Value - quantity
        };
    }
    
    public bool IsZero => Value == 0;
    
    public bool IsGreaterThan(int value) => Value > value;
    
    public bool IsLessThan(int value) => Value < value;

    public override string ToString() => Value.ToString();
    
    public static implicit operator int(Quantity quantity) => quantity.Value;
    
    public static implicit operator int?(Quantity? quantity) => quantity?.Value;
}