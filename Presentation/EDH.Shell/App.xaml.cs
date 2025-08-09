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
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

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
		
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(_configuration)
			.CreateLogger();
		
		Log.Information("Entrepreneur Digital Hub is starting up...");
		
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
		Log.Information("Registering main services.");
		
		//Configuration
		containerRegistry.RegisterInstance(_configuration);
		
		//Logging
		containerRegistry.RegisterInstance<ILoggerFactory>(new SerilogLoggerFactory());
		containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));
		
		//Database
		string databaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "Database");
		if (!Directory.Exists(databaseFolder))
		{
			Directory.CreateDirectory(databaseFolder);
		}

		string connectionString = _configuration.GetConnectionString("DefaultConnection")
		                          ?? throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured or is null.");
		
		Log.Information("Database connection string: {ConnectionString}.", connectionString);

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
		
		Log.Information("Main services registered.");
	}

	protected override Window CreateShell()
	{
		Log.Information("Creating shell...");
		return Container.Resolve<MainWindow>();
	}

	protected override void OnInitialized()
	{
		try
		{
			base.OnInitialized();

			var dbContext = Container.Resolve<EdhDbContext>();
			dbContext.Database.Migrate();
			
			Log.Information("Entrepreneur Digital Hub initialized.");
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to initialize.");
			throw;
		}
	}

	protected override IModuleCatalog CreateModuleCatalog()
	{
		Log.Information("Creating module catalog from {ModulePath}.", @".\Modules");
		return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
	}

	protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
	{
		Log.Information("Configuring module catalog.");
		base.ConfigureModuleCatalog(moduleCatalog);
		moduleCatalog.AddModule<PresentationCommonModule>();
		Log.Information("Module catalog configured.");
	}

	protected override void OnExit(ExitEventArgs e)
	{
		try
		{
			Log.Information("Entrepreneur Digital Hub is shutting down...");
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Entrepreneur Digital Hub failed during shutdown.");
			throw;
		}
		finally
		{
			Log.Information("Entrepreneur Digital Hub has shut down.");
			Log.CloseAndFlush();
			base.OnExit(e);
		}
	}
}