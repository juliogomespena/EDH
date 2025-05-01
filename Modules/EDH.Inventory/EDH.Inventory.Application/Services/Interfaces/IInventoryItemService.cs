using EDH.Inventory.Application.DTOs.EditStockQuantities;

namespace EDH.Inventory.Application.Services.Interfaces;

public interface IInventoryItemService
{
	Task<IEnumerable<GetInventoryItemsEditStockQuantitiesDto>> GetInventoryItemsByNameAsync(string itemName);

	Task UpdateStockQuantitiesAsync(UpdateStockQuantitiesDto updateStockQuantitiesDto);
}