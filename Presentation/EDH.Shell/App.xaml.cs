using System.IO;
using System.Windows;
using EDH.Core.Interfaces.Infrastructure;
using EDH.Core.Interfaces.Items;
using EDH.Infrastructure.Data.ApplicationDbContext;
using EDH.Infrastructure.Data.UnitOfWork;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Services;
using EDH.Items.Infrastructure.Repositories;
using EDH.Presentation.Common;
using EDH.Presentation.Common.Resources.Components.Dialogs;
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

		containerRegistry.RegisterInstance(options);

		containerRegistry.RegisterScoped<EdhDbContext>();
		containerRegistry.RegisterScoped<IUnitOfWork, UnitOfWork>();
		containerRegistry.RegisterScoped<IItemService, ItemService>();
		containerRegistry.RegisterScoped<IItemRepository, ItemRepository>();

		containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

		containerRegistry.RegisterDialog<OkDialog, OkDialogViewModel>();
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