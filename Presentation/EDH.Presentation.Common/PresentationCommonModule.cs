using EDH.Presentation.Common.ViewModels;
using EDH.Presentation.Common.Views;
using EDH.Core.Constants;
using EDH.Presentation.Common.Resources.Dialogs;

namespace EDH.Presentation.Common;

public sealed class PresentationCommonModule(IRegionManager regionManager) : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		//Views and viewmodels
		ViewModelLocationProvider.Register<MainWindowHeaderView, MainWindowHeaderViewModel>();
		ViewModelLocationProvider.Register<MainWindowMenuView, MainWindowMenuViewModel>();

		//Dialogs
		containerRegistry.RegisterDialog<OkDialog, OkDialogViewModel>();
		containerRegistry.RegisterDialog<YesNoDialog, YesNoDialogViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		regionManager.RegisterViewWithRegion(NavigationConstants.Regions.MainWindowHeader, typeof(MainWindowHeaderView));
		regionManager.RegisterViewWithRegion(NavigationConstants.Regions.MainWindowMenu, typeof(MainWindowMenuView));
	}
}