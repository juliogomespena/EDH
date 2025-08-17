using EDH.Core.Interfaces.ISales;
using EDH.Sales.Application.Services;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Core.Services;
using EDH.Sales.Core.Services.Interfaces;
using EDH.Sales.Infrastructure.Repositories;
using EDH.Sales.Presentation.ViewModels;
using EDH.Sales.Presentation.Views;

namespace EDH.Sales.Presentation;

public sealed class SalesPresentationModule : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		//Repositories
		containerRegistry.RegisterScoped<ISaleRepository, SaleRepository>();
		
		//Services
		containerRegistry.RegisterScoped<ISaleService, SaleService>();
		containerRegistry.RegisterScoped<ISaleCalculationService, SaleCalculationService>();
		
		//Views and viewmodels
		containerRegistry.RegisterForNavigation<RecordSaleView, RecordSaleViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		
	}
}