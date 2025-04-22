namespace EDH.Items.Presentation.ViewModels;

public class ItemEditViewModel : BindableBase, INavigationAware
{
	public void OnNavigatedTo(NavigationContext navigationContext)
	{

	}
	public bool IsNavigationTarget(NavigationContext navigationContext)
	{
		return true;
	}
	public void OnNavigatedFrom(NavigationContext navigationContext)
	{

	}
}