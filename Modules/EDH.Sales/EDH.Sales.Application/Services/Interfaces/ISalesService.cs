using EDH.Sales.Application.DTOs;
using EDH.Sales.Application.DTOs.RecordSale;

namespace EDH.Sales.Application.Services.Interfaces;

public interface ISalesService
{
    Task<IEnumerable<GetInventoryItemsRecordSaleDto>> GetInventoryItemsByNameAsync(string itemName);
}