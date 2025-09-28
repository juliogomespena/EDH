using EDH.Core.Enums;
using EDH.Core.Exceptions;

namespace EDH.Core.ValueObjects;

public sealed record Money
{
    public required decimal Amount { get; init; }
    public required Currency Currency { get; init; }

    private Money() { }
    
    public static Money Zero(Currency currency) => new()
    { 
        Amount = 0, 
        Currency = currency
    };
    
    public static Money FromAmount (decimal amount, Currency currency) => new()
    {
        Amount = amount, 
        Currency = currency
    };

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidCurrencyException(Currency, other.Currency);
        
        return this with
        {
            Amount = Amount + other.Amount
        };
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidCurrencyException(Currency, other.Currency);
        
        return this with
        {
            Amount = Amount - other.Amount
        };
    }

    public Money MultiplyBy(decimal multiplier) =>
        this with
        {
            Amount = Amount * multiplier
        };

    public Money ApplyPercentage(decimal percentage) =>
        this with
        {
            Amount = Amount + (Amount * (percentage / 100))
        };
    
    public Money GetPercentageCorrespondingValue(decimal percentage) =>
        this with
        {
            Amount = Amount * (percentage / 100)
        };
    
    public override string ToString() => $"{Currency.ToString()} {Amount:N2}";
    
    public string ToString(string format) => Amount.ToString(format);
    
    public static implicit operator decimal(Money money) => money.Amount;
}