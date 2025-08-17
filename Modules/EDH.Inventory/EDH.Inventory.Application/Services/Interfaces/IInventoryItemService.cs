using EDH.Core.Common;
using EDH.Inventory.Application.DTOs.Request.StockAdjustmentCalculation;
using EDH.Inventory.Application.DTOs.Request.UpdateStockQuantities;
using EDH.Inventory.Application.DTOs.Response.GetInventoryItems;
using EDH.Inventory.Application.DTOs.Response.StockAdjustmentCalculation;
using EDH.Inventory.Application.DTOs.Response.UpdateStockQuantities;

namespace EDH.Inventory.Application.Services.Interfaces;

public interface IInventoryItemService
{
	Task<Result<IEnumerable<GetInventoryItemsResponse>>> GetInventoryItemsByNameAsync(string itemName);
	
	Result<StockAdjustmentCalculationResponse> CalculateStockAdjustment(StockAdjustmentCalculationRequest request);

	Task<Result<UpdateStockQuantitiesResponse>> UpdateStockQuantitiesAsync(UpdateStockQuantitiesRequest request);
}