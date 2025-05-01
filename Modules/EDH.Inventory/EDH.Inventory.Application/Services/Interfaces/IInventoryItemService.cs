using EDH.Inventory.Application.DTOs;

namespace EDH.Inventory.Application.Services.Interfaces;

public interface IInventoryItemService
{
	Task<IEnumerable<GetInventoryItemDto>> GetInventoryItemsByNameAsync(string itemName);
}