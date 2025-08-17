using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.ValueObjects;
using EDH.Items.Core.ValueObjects;

namespace EDH.Items.Core.Services.Interfaces;

public interface IProfitMarginCalculationService
{
    Result<ItemProfitMarginCalculation> CalculateProfitMargin(Money price, Money costs);
}