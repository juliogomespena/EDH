using EDH.Inventory.Application.DTOs;
using EDH.Inventory.Application.Services.Interfaces;

namespace EDH.Inventory.Application.Services;

public class InventoryItemService : IInventoryItemService
{
	public async Task<int> CreateInventoryItemAsync(CreateInventoryItemDto createInventoryItemDto)
	{
		return await Task.FromResult(1);
	}
}