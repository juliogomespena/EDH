using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Infrastructure.Common.Events;
using EDH.Infrastructure.Data.ApplicationDbContext;
using EDH.Infrastructure.Data.UnitOfWork;
using EDH.Presentation.Common;
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
	private readonly IConfiguration _configuration;

	public App()
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

		_configuration = builder.Build();
		
		Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
 	
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;
		
		FrameworkElement.LanguageProperty.OverrideMetadata(
			typeof(FrameworkElement),
			new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
	}

	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
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
    
		// Event wrapper
		containerRegistry.RegisterSingleton<EDH.Core.Events.Abstractions.IEventAggregator, PrismEventAggregatorAdapter>();


		//Main view and viewmodel
		containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
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