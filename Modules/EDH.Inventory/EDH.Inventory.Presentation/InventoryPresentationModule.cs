using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Infrastructure.Data.UnitOfWork;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Inventory.Application.Services;
using EDH.Inventory.Infrastructure.Repositories;
using EDH.Inventory.Presentation.Resources.Dialogs;
using EDH.Inventory.Application.Handlers.Interfaces;
using EDH.Inventory.Application.Handlers;
using EDH.Inventory.Core.Services;
using EDH.Inventory.Core.Services.Interfaces;

namespace EDH.Inventory.Presentation;

public sealed class InventoryPresentationModule : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		//Repositories
		containerRegistry.RegisterScoped<IInventoryItemRepository, InventoryItemRepository>();

		//Services
		containerRegistry.RegisterScoped<IStockAdjustmentCalculationService, StockAdjustmentCalculationService>();
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