namespace EDH.Items.Presentation.ViewModels;

internal sealed class ItemEditViewModel : BindableBase, INavigationAware
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