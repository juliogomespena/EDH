using EDH.Presentation.Common.ViewModels;
using EDH.Presentation.Common.Views;
using EDH.Core.Constants;

namespace EDH.Presentation.Common;

public sealed class PresentationCommonModule(IRegionManager regionManager) : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		ViewModelLocationProvider.Register<MainWindowHeaderView, MainWindowHeaderViewModel>();
		ViewModelLocationProvider.Register<MainWindowMenuView, MainWindowMenuViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		regionManager.RegisterViewWithRegion(NavigationConstants.Regions.MainWindowHeader, typeof(MainWindowHeaderView));
		regionManager.RegisterViewWithRegion(NavigationConstants.Regions.MainWindowMenu, typeof(MainWindowMenuView));
	}
}