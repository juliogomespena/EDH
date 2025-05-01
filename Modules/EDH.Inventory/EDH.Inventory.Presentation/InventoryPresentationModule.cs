using EDH.Inventory.Presentation.Resources.Dialogs;

namespace EDH.Inventory.Presentation;

public sealed class InventoryPresentationModule : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		containerRegistry.RegisterDialog<AddStockQuantitiesDialog, AddStockQuantitiesDialogViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		
	}
}