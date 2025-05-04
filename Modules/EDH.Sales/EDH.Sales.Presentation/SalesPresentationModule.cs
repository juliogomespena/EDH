using EDH.Sales.Application.Services;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Presentation.ViewModels;
using EDH.Sales.Presentation.Views;

namespace EDH.Sales.Presentation;

public sealed class SalesPresentationModule : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		//Repositories
		
		//Services
		containerRegistry.RegisterScoped<ISalesService, SalesService>();
		
		//Views and viewmodels
		containerRegistry.RegisterForNavigation<RecordSaleView, RecordSaleViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		
	}
}