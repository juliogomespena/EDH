using EDH.Core.Interfaces.IInventory;
using EDH.Inventory.Application.DTOs;
using EDH.Inventory.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EDH.Inventory.Application.Services;

public class InventoryItemService : IInventoryItemService
{
	private readonly IInventoryItemRepository _inventoryItemRepository;

	public InventoryItemService(IInventoryItemRepository inventoryItemRepository)
	{
		_inventoryItemRepository = inventoryItemRepository;
	}

	public async Task<IEnumerable<GetInventoryItemDto>> GetInventoryItemsByNameAsync(string itemName)
	{
		string pattern = $"%{itemName}%";
		var inventoryItems = await _inventoryItemRepository
			.FindAsync(inventoryItem => EF.Functions.Like(inventoryItem.Item.Name, pattern));

		return inventoryItems.Select(inventoryItem => new GetInventoryItemDto(inventoryItem.Id, inventoryItem.Item.Name, inventoryItem.Quantity, inventoryItem.AlertThreshold));
	}
}