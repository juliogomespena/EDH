using EDH.Items.Presentation.ViewModels;
using EDH.Items.Presentation.Views;

namespace EDH.Items.Presentation;

public sealed class ItemsPresentationModule() : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		containerRegistry.RegisterForNavigation<ItemRegistrationView, ItemRegistrationViewModel>();
		containerRegistry.RegisterForNavigation<ItemEditView, ItemEditViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{

	}
}