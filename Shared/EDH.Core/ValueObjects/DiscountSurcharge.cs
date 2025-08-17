using EDH.Core.Enums;
using EDH.Core.Exceptions;

namespace EDH.Core.ValueObjects;

public sealed record DiscountSurcharge
{
    public required decimal Value { get; init;  }
    public required AdjustmentType Type { get; init; }
    public required DiscountSurchargeMode Mode { get; init; }
    
    private DiscountSurcharge() { }
    
    public static DiscountSurcharge None => new()
    {
        Value = 0,
        Type = AdjustmentType.None,
        Mode = DiscountSurchargeMode.Money
    };

    public static DiscountSurcharge Discount(decimal value, DiscountSurchargeMode mode)
    {
        if (value > 0)
            throw new InvalidDiscountException();
        
        return new DiscountSurcharge
        {
            Value = value,
            Type = AdjustmentType.Discount,
            Mode = mode
        };
    }
    
    public static DiscountSurcharge Surcharge(decimal value, DiscountSurchargeMode mode)
    {
        if (value < 0)
            throw new InvalidSurchargeException();

        return new DiscountSurcharge
        {
            Value = value,
            Type = AdjustmentType.Surcharge,
            Mode = mode
        };
    }

    public Money Apply(Money money) =>
        Mode == DiscountSurchargeMode.Percentage 
            ? money.ApplyPercentage(Value) 
            : money.Add(Money.FromAmount(Value, money.Currency));
    
    public Money DiscountSurchargeValue(Money money) =>
        Mode == DiscountSurchargeMode.Percentage
            ? money.GetPercentageCorrespondingValue(Value)
            : Money.FromAmount(Value, money.Currency);
}