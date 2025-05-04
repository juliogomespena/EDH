using EDH.Sales.Application.DTOs;

namespace EDH.Sales.Application.Services.Interfaces;

public interface ISalesService
{
    Task<IEnumerable<GetInventoryItemsRecordSaleDto>> GetInventoryItemsByNameAsync(string itemName);
}