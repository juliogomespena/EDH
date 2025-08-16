using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Items.Core.ValueObjects;

public sealed record ItemProfitMarginCalculation
{
    public required Money Value { get; init; }
    public required decimal Percentage { get; init; }

    public static ItemProfitMarginCalculation Calculate(Money price, Money costs)
    {
        if (price.Amount < 0 || costs.Amount < 0)
            throw new DomainValidationException("Price and costs must be greater or equal to 0.");
        if (price.Currency != costs.Currency)
            throw new InvalidCurrencyException();

        var value = Money.FromAmount(price - costs, price.Currency);
        
        return new ItemProfitMarginCalculation
        {
            Value = value,
            Percentage = value == 0
                ? 0
                : value.Amount / price.Amount
        };
    }
}