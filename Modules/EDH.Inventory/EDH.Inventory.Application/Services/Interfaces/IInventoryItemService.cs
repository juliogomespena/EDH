using EDH.Core.Common;
using EDH.Inventory.Application.DTOs.Requests.StockAdjustmentCalculation;
using EDH.Inventory.Application.DTOs.Requests.UpdateStockQuantities;
using EDH.Inventory.Application.DTOs.Responses.GetInventoryItems;
using EDH.Inventory.Application.DTOs.Responses.StockAdjustmentCalculation;
using EDH.Inventory.Application.DTOs.Responses.UpdateStockQuantities;

namespace EDH.Inventory.Application.Services.Interfaces;

public interface IInventoryItemService
{
	Task<Result<IEnumerable<GetInventoryItemsResponse>>> GetInventoryItemsByNameAsync(string itemName);
	
	Result<StockAdjustmentCalculationResponse> CalculateStockAdjustment(StockAdjustmentCalculationRequest request);

	Task<Result<UpdateStockQuantitiesResponse>> UpdateStockQuantitiesAsync(UpdateStockQuantitiesRequest request);
}