using EDH.Core.Enums;
using EDH.Core.Exceptions;

namespace EDH.Core.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public static Money Zero(Currency currency) => new(0, currency);
    
    public static Money FromAmount (decimal amount, Currency currency) => new(amount, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidCurrencyException(Currency, other.Currency);
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidCurrencyException(Currency, other.Currency);
        
        return new Money(Amount - other.Amount, Currency);
    }

    public Money MultiplyBy(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    public Money ApplyPercentage(decimal percentage)
    {
        return new Money(Amount + (Amount * (percentage / 100)), Currency);   
    }
    
    public Money GetPercentageCorrespondingValue(decimal percentage)
    {
        return new Money(Amount * (percentage / 100), Currency);   
    }

    public override string ToString() => $"{Currency.ToString()} {Amount:N2}";
    
    public string ToString(string format) => Amount.ToString(format);
    
    public static implicit operator decimal(Money money) => money.Amount;
}