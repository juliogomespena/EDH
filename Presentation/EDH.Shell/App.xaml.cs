using System.Windows;
using EDH.Shell.Views;

namespace EDH.Shell;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		
	}

	protected override Window CreateShell()
	{
		return Container.Resolve<MainWindow>();
	}

	protected override IModuleCatalog CreateModuleCatalog()
	{
		return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
	}
}