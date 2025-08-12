using EDH.Core.Common;
using EDH.Sales.Application.DTOs;
using EDH.Sales.Application.DTOs.RecordSale;

namespace EDH.Sales.Application.Services.Interfaces;

public interface ISaleService
{
    Task<Result<IEnumerable<GetInventoryItemRecordSaleDto>>> GetInventoryItemsByNameAsync(string itemName);

    Task<Result<SaleRecordSaleDto>> CreateSaleAsync(SaleRecordSaleDto sale);
}