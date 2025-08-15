namespace EDH.Core.ValueObjects;

public sealed record Quantity
{
    public int Value { get; }

    private Quantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(value));
        
        Value = value;
    }
    
    public static Quantity Zero => new(0);

    public static Quantity FromValue(int value) => new Quantity(value);

    public Quantity Add(int quantity)
    {
        if (Value + quantity < 0)
            throw new InvalidOperationException("Quantity cannot be negative.");
        
        return new Quantity(Value + quantity);
    }
    
    public Quantity Subtract(int quantity)
    {
        if (Value - quantity < 0)
            throw new InvalidOperationException("Quantity cannot be negative.");
        
        return new Quantity(Value - quantity);
    }
    
    public bool IsZero => Value <= 0;
    
    public bool IsGreaterThan(int value) => Value > value;
    
    public bool IsLessThan(int value) => Value < value;

    public override string ToString() => Value.ToString();
    
    public static implicit operator int(Quantity quantity) => quantity.Value;
    
    public static implicit operator int?(Quantity? quantity) => quantity?.Value;
}