using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Sales.Core.ValueObjects;

public sealed record SaleCalculation
{
    public required Money Costs { get; init; }
    public required Money Profit { get; init; }
    public required Money Adjustment { get; init; }
    public required Money Total { get; init; }
    
    public static SaleCalculation Calculate(IEnumerable<SaleLineCalculation> saleLineCalculations)
    {
        var saleLines = saleLineCalculations.ToList();

        if (!saleLines.Any())
        {
            return new SaleCalculation
            {
                Costs = Money.Zero(Currency.Usd),
                Profit = Money.Zero(Currency.Usd),
                Adjustment = Money.Zero(Currency.Usd),
                Total = Money.Zero(Currency.Usd)
            };
        }
        
        var currency = saleLines[0].Currency;
        if (saleLines.Any(s => s.Currency != currency))
            throw new InvalidCurrencyException(saleLines.Select(c => c.Currency).ToArray());

        return new SaleCalculation
        {
            Costs = Money.FromAmount(saleLines.Sum(s => s.Costs), currency),
            Profit = Money.FromAmount(saleLines.Sum(s => s.Profit), currency),
            Adjustment = Money.FromAmount(saleLines.Sum(s => s.Adjustment), currency),
            Total = Money.FromAmount(saleLines.Sum(s => s.Subtotal), currency)
        };
    }
}