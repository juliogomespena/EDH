using EDH.Core.Common;
using EDH.Core.Enums;
using EDH.Core.ValueObjects;
using EDH.Sales.Core.ValueObjects;

namespace EDH.Sales.Core.Services.Interfaces;

public interface ISaleCalculationService
{
    Result<SaleLineCalculation> CalculateLine(Money unitPrice, Quantity quantity, Money unitCosts, DiscountSurcharge discountSurcharge);
    
    Result<SaleCalculation> CalculateTotal(IEnumerable<SaleLineCalculation> saleLineCalculations);

    Result<SaleLineCalculation> ReconstructSaleLine(Money unitPrice, Quantity quantity, Money costs,
        Money adjustment, Money profit, Money subtotal, Currency currency);

    Result HasAvailability(Quantity requested, Quantity available);
}