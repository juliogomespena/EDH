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
    
    public static SaleCalculation Calculate(IEnumerable<SaleLineCalculation> saleLineCalculations, Currency currency = Currency.Usd)
    {
        var saleLines = saleLineCalculations.ToList();
        
        if (saleLines.Any(sl => sl.Quantity < 0))
            throw new DomainValidationException("Quantity must be greater than zero.");
        if (saleLines.Any(sl => sl.Costs < 0))
            throw new DomainValidationException("Costs must be greater than zero.");
        if (saleLines.Any(sl => sl.UnitPrice < 0))
            throw new DomainValidationException("UnitPrice must be greater than zero.");

        if (!saleLines.Any())
        {
            return new SaleCalculation
            {
                Costs = Money.Zero(currency),
                Profit = Money.Zero(currency),
                Adjustment = Money.Zero(currency),
                Total = Money.Zero(currency)
            };
        }
        
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