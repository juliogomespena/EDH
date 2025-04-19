using EDH.Presentation.Common.ViewModels;
using EDH.Presentation.Common.Views;

namespace EDH.Presentation.Common;

public class PresentationCommonModule(IRegionManager regionManager) : IModule
{
	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		ViewModelLocationProvider.Register<MainWindowHeaderView, MainWindowHeaderViewModel>();
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		regionManager.RegisterViewWithRegion("MainWindowHeader", typeof(MainWindowHeaderView));
	}
}