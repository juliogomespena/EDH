using EDH.Core.Enums;
using EDH.Core.Exceptions;

namespace EDH.Core.ValueObjects;

public sealed record DiscountSurcharge
{
    public decimal Value { get; }
    public DiscountSurchargeMode Mode { get; }
    
    private DiscountSurcharge(decimal value, DiscountSurchargeMode mode)
    {
        Value = value;
        Mode = mode;
    }
    
    public static DiscountSurcharge None => new(0, DiscountSurchargeMode.Money);

    public static DiscountSurcharge Discount(decimal value, DiscountSurchargeMode mode)
    {
        if (value > 0)
            throw new InvalidDiscountException();
        
        return new DiscountSurcharge(value, mode);
    }
    
    public static DiscountSurcharge Surcharge(decimal value, DiscountSurchargeMode mode)
    {
        if (value < 0)
            throw new InvalidSurchargeException();

        return new DiscountSurcharge(value, mode);
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