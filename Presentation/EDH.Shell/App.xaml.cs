using System.IO;
using System.Windows;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Core.Interfaces.IItems;
using EDH.Infrastructure.Data.ApplicationDbContext;
using EDH.Infrastructure.Data.UnitOfWork;
using EDH.Inventory.Application.Handlers;
using EDH.Inventory.Application.Handlers.Interfaces;
using EDH.Inventory.Application.Services;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Inventory.Infrastructure.Repositories;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Services;
using EDH.Items.Infrastructure.Repositories;
using EDH.Presentation.Common;
using EDH.Presentation.Common.Resources.Dialogs;
using EDH.Shell.ViewModels;
using EDH.Shell.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EDH.Shell;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
	private IConfiguration _configuration;
	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

		_configuration = builder.Build();

		containerRegistry.RegisterInstance(_configuration);

		string databaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "Database");
		if (!Directory.Exists(databaseFolder))
		{
			Directory.CreateDirectory(databaseFolder);
		}

		string connectionString = _configuration.GetConnectionString("DefaultConnection")
		                          ?? throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured or is null.");

		var options = new DbContextOptionsBuilder<EdhDbContext>()
			.UseSqlite(connectionString)
			.Options;

		//Main configuration
		containerRegistry.RegisterInstance(options);
		containerRegistry.RegisterScoped<EdhDbContext>();
		containerRegistry.RegisterScoped<IUnitOfWork, UnitOfWork>();

		//Repositories
		containerRegistry.RegisterScoped<IItemRepository, ItemRepository>();
		containerRegistry.RegisterScoped<IItemCategoryRepository, ItemCategoryRepository>();
		containerRegistry.RegisterScoped<IInventoryItemRepository, InventoryItemRepository>();

		//Services
		containerRegistry.RegisterScoped<IItemService, ItemService>();
		containerRegistry.RegisterScoped<IItemCategoryService, ItemCategoryService>();
		containerRegistry.RegisterScoped<IInventoryItemService, InventoryItemService>();

		//Handlers
		containerRegistry.RegisterSingleton<IInventoryItemEventHandler, InventoryItemEventHandler>();

		//Views and viewmodels
		containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

		//Dialogs
		containerRegistry.RegisterDialog<OkDialog, OkDialogViewModel>();
		containerRegistry.RegisterDialog<YesNoDialog, YesNoDialogViewModel>();
	}

	protected override Window CreateShell()
	{
		return Container.Resolve<MainWindow>();
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var dbContext = Container.Resolve<EdhDbContext>();

		dbContext.Database.Migrate();

		Container.Resolve<IInventoryItemEventHandler>().InitializeSubscriptions();
	}

	protected override IModuleCatalog CreateModuleCatalog()
	{
		return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
	}

	protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
	{
		base.ConfigureModuleCatalog(moduleCatalog);
		moduleCatalog.AddModule<PresentationCommonModule>();
	}
}