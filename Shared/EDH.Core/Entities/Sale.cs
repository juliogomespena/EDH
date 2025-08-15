using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Core.Entities;

public sealed class Sale
{
    public int Id { get; set; }

    private decimal _totalVariableCostsAmount;
    public required Money TotalVariableCosts
    {
        get => Money.FromAmount(_totalVariableCostsAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _totalVariableCostsAmount = value.Amount;
        }
    }

    private decimal _totalProfitAmount;
    public required Money TotalProfit
    {
        get => Money.FromAmount(_totalProfitAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _totalProfitAmount = value.Amount;
        }
    }

    private decimal _totalAdjustmentAmount;
    public Money TotalAdjustment
    {
        get => Money.FromAmount(_totalAdjustmentAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _totalAdjustmentAmount = value.Amount;
        }
    }

    private decimal _totalValueAmount;
    public required Money TotalValue
    {
        get => Money.FromAmount(_totalValueAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _totalValueAmount = value.Amount;
        }
    }
    
    public required Currency Currency { get; set; }
    
    public DateTime Date { get; set; }
    
    public ICollection<SaleLine> SaleLines { get; set; } = [];
}