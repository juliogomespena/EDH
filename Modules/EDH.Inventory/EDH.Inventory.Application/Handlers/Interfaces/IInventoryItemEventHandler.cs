using EDH.Core.Events.Inventory.Parameters;

namespace EDH.Inventory.Application.Handlers.Interfaces;

public interface IInventoryItemEventHandler
{
	void HandleCreateInventoryItem(CreateInventoryItemEventParameters parameters);
	void HandleGetInventoryItemsByName(GetInventoryItemsByNameEventParameters parameters);
	void InitializeSubscriptions();
}