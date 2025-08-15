using EDH.Core.Interfaces.IItems;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Services;
using EDH.Items.Core.Services;
using EDH.Items.Core.Services.Interfaces;
using EDH.Items.Infrastructure.Repositories;
using EDH.Items.Presentation.ViewModels;
using EDH.Items.Presentation.Views;

namespace EDH.Items.Presentation;

public sealed class ItemsPresentationModule() : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		//Repositories
		containerRegistry.RegisterScoped<IItemRepository, ItemRepository>();
		containerRegistry.RegisterScoped<IItemCategoryRepository, ItemCategoryRepository>();

		//Services
		containerRegistry.RegisterScoped<IProfitMarginCalculationService, ProfitMarginCalculationService>();
		containerRegistry.RegisterScoped<IItemService, ItemService>();
		containerRegistry.RegisterScoped<IItemCategoryService, ItemCategoryService>();

		//Views and viewmodels
		containerRegistry.RegisterForNavigation<ItemRegistrationView, ItemRegistrationViewModel>();
		containerRegistry.RegisterForNavigation<ItemEditView, ItemEditViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{

	}
}