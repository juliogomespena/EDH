using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using EDH.Core.Constants;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Infrastructure.Common.Events;
using EDH.Infrastructure.Common.Exceptions;
using EDH.Infrastructure.Common.Exceptions.Enums;
using EDH.Infrastructure.Common.Exceptions.Interface;
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
using IEventAggregator = EDH.Core.Events.Abstractions.IEventAggregator;

namespace EDH.Shell;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
	private readonly IConfiguration _configuration;

	public App()
	{
		try
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
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to start up.");
			ShowCriticalAndExit("Entrepreneur Digital Hub failed to start up.");
			throw;
		}
	}

	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		try
		{
			Log.Information("Registering main services.");

			//Builder
			containerRegistry.RegisterInstance(_configuration);

			//Logging
			containerRegistry.RegisterInstance<ILoggerFactory>(new SerilogLoggerFactory());
			containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

			//Global exception coordinator
			containerRegistry.RegisterSingleton<IGlobalExceptionCoordinator, GlobalExceptionCoordinator>();

			//Database
			string databaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "Database");
			if (!Directory.Exists(databaseFolder))
			{
				Directory.CreateDirectory(databaseFolder);
			}

			string connectionString = _configuration.GetConnectionString("DefaultConnection")
			                          ?? throw new InvalidOperationException(
				                          "The connection string 'DefaultConnection' is not configured or is null.");

			Log.Information("Database connection string: {ConnectionString}.", connectionString);

			var options = new DbContextOptionsBuilder<EdhDbContext>()
				.UseSqlite(connectionString)
				.Options;

			//Main configuration
			containerRegistry.RegisterInstance(options);
			containerRegistry.RegisterScoped<EdhDbContext>();
			containerRegistry.RegisterScoped<IUnitOfWork, UnitOfWork>();

			// Event wrapper
			containerRegistry
				.RegisterSingleton<IEventAggregator, PrismEventAggregatorAdapter>();

			//Main view and viewmodel
			containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

			Log.Information("Main services registered.");
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to register main services.");
			ShowCriticalAndExit("Entrepreneur Digital Hub failed to start up.");
			throw;
		}
	}

	protected override Window CreateShell()
	{
		try
		{
			Log.Information("Creating shell...");
			return Container.Resolve<MainWindow>();
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to create shell.");
			ShowCriticalAndExit("Entrepreneur Digital Hub failed to start up.");
			throw;
		}
	}

	protected override void OnInitialized()
	{
		try
		{
			base.OnInitialized();
			
			DispatcherUnhandledException += OnDispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
			TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

			var dbContext = Container.Resolve<EdhDbContext>();
			dbContext.Database.Migrate();
			
			Log.Information("Entrepreneur Digital Hub initialized.");
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to initialize.");
			ShowCriticalAndExit("Entrepreneur Digital Hub failed to start up.");
			throw;
		}
	}

	protected override IModuleCatalog CreateModuleCatalog()
	{
		try
		{
			string modulesPath = _configuration.GetValue("Application:ModulesPath", @".\Modules");
			int mainModulesCount = _configuration.GetValue("Application:MainModulesCount", 0);
			
			Log.Information("Creating module catalog from {ModulePath}.", modulesPath);
			
			if (!Directory.Exists(modulesPath))
			{
				Log.Fatal("Modules directory not found!");
				ShowCriticalAndExit("Modules directory not found!");
				return new DirectoryModuleCatalog() { ModulePath = modulesPath };
			}

			string[] modulesFiles = Directory.GetFiles(modulesPath, "*.dll");
			if (modulesFiles.Length != mainModulesCount)
			{
				Log.Fatal("Required modules not found!");
				ShowCriticalAndExit("Required modules not found!");
				return new DirectoryModuleCatalog() { ModulePath = modulesPath };
			}

			Log.Information("Modules directory '{Modules}' found with {ModulesCount} modules.", modulesPath,
				modulesFiles.Length);
			return new DirectoryModuleCatalog() { ModulePath = modulesPath };
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to create module catalog.");
			ShowCriticalAndExit("Entrepreneur Digital Hub failed to start up.");
			throw;
		}
	}

	protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
	{
		try
		{
			Log.Information("Configuring module catalog.");
			base.ConfigureModuleCatalog(moduleCatalog);
			moduleCatalog.AddModule<PresentationCommonModule>();
			Log.Information("Module catalog configured.");
		}
		catch (Exception e)
		{
			Log.Fatal(e, "Entrepreneur Digital Hub failed to configure module catalog.");
			ShowCriticalAndExit("Entrepreneur Digital Hub failed to start up.");
			throw;
		}
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
	
	private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
	{
		var coordinator = Container.Resolve<IGlobalExceptionCoordinator>();
		var ex = e.Exception.InnerException ?? e.Exception;
		var result = coordinator.Handle(ex, onUiThread: false, context: "TaskScheduler.UnobservedTaskException");

		e.SetObserved();
		if (result == HandlingResult.Exit) Current.Shutdown();
	}

	private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		var ex = e.ExceptionObject as Exception ?? new Exception("Unknown AppDomain exception.");
		var coordinator = Container.Resolve<IGlobalExceptionCoordinator>();
		coordinator.Handle(ex, onUiThread: false, context: "AppDomain.UnhandledException");
		
		Current.Shutdown();
	}

	private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		var coordinator = Container.Resolve<IGlobalExceptionCoordinator>();
		var result = coordinator.Handle(e.Exception, onUiThread: true, context: "DispatcherUnhandledException");

		e.Handled = result == HandlingResult.Continue;
		if (result == HandlingResult.Exit) Current.Shutdown();
	}

	private void ShowCriticalAndExit(string message)
	{
		try
		{
			MessageBox.Show(message, "EDH - Critical initialization error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		finally
		{
			Log.Fatal("Application shutting down due to critical error: {Message}", message);
			Log.CloseAndFlush();
			Environment.Exit(1);
		}
	}
}