using EDH.Core.Common;
using EDH.Inventory.Application.DTOs.EditStockQuantities;

namespace EDH.Inventory.Application.Services.Interfaces;

public interface IInventoryItemService
{
	Task<Result<IEnumerable<GetInventoryItems>>> GetInventoryItemsByNameAsync(string itemName);

	Task<Result<UpdateStockQuantities>> UpdateStockQuantitiesAsync(UpdateStockQuantities updateStockQuantities);
}