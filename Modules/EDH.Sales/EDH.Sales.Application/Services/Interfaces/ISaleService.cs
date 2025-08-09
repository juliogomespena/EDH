using EDH.Sales.Application.DTOs;
using EDH.Sales.Application.DTOs.RecordSale;

namespace EDH.Sales.Application.Services.Interfaces;

public interface ISaleService
{
    Task<IEnumerable<GetInventoryItemsRecordSaleDto>> GetInventoryItemsByNameAsync(string itemName);

    Task<int> CreateSaleAsync(SaleRecordSaleDto sale);
}