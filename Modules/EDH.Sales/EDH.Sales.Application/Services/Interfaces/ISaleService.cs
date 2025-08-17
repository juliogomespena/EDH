using EDH.Core.Common;
using EDH.Sales.Application.DTOs.Requests.CreateSale;
using EDH.Sales.Application.DTOs.Requests.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Requests.SaleTotalCalculation;
using EDH.Sales.Application.DTOs.Responses.CreateSale;
using EDH.Sales.Application.DTOs.Responses.GetInventoryItem;
using EDH.Sales.Application.DTOs.Responses.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Responses.SaleTotalCalculationResponse;

namespace EDH.Sales.Application.Services.Interfaces;

public interface ISaleService
{
    Task<Result<IEnumerable<GetInventoryItemResponse>>> GetInventoryItemsByNameAsync(string itemName);

    Task<Result<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest createSaleRequest);
    
    Result<SaleLineCalculationResponse> CalculateSaleLine(SaleLineCalculationRequest request);
    
    Result<SaleTotalCalculationResponse> CalculateSaleTotal(SaleTotalCalculationRequest request);
}