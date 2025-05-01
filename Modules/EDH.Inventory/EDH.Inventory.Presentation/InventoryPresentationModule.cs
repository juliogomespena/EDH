using EDH.Core.Interfaces.IInventory;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Inventory.Application.Services;
using EDH.Inventory.Infrastructure.Repositories;
using EDH.Inventory.Presentation.Resources.Dialogs;
using EDH.Inventory.Application.Handlers.Interfaces;
using EDH.Inventory.Application.Handlers;

namespace EDH.Inventory.Presentation;

public sealed class InventoryPresentationModule : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		//Repositories
		containerRegistry.RegisterScoped<IInventoryItemRepository, InventoryItemRepository>();

		//Services
		containerRegistry.RegisterScoped<IInventoryItemService, InventoryItemService>();

		//Handlers
		containerRegistry.RegisterSingleton<IInventoryItemEventHandler, InventoryItemEventHandler>();

		//Dialogs
		containerRegistry.RegisterDialog<EditStockQuantitiesDialog, EditStockQuantitiesDialogViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		containerProvider.Resolve<IInventoryItemEventHandler>().InitializeSubscriptions();
	}
}