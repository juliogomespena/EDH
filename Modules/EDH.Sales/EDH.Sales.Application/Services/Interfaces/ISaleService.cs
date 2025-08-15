using EDH.Core.Common;
using EDH.Sales.Application.DTOs.Request.CreateSale;
using EDH.Sales.Application.DTOs.Request.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Request.SaleTotalCalculation;
using EDH.Sales.Application.DTOs.Response.CreateSale;
using EDH.Sales.Application.DTOs.Response.GetInventoryItem;
using EDH.Sales.Application.DTOs.Response.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Response.SaleTotalCalculationResponse;

namespace EDH.Sales.Application.Services.Interfaces;

public interface ISaleService
{
    Task<Result<IEnumerable<GetInventoryItemResponse>>> GetInventoryItemsByNameAsync(string itemName);

    Task<Result<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest createSaleRequest);
    
    Result<SaleLineCalculationResponse> CalculateSaleLine(SaleLineCalculationRequest request);
    
    Result<SaleTotalCalculationResponse> CalculateSaleTotal(SaleTotalCalculationRequest request);
}