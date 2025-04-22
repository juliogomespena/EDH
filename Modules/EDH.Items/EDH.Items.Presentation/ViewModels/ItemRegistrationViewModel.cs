namespace EDH.Items.Presentation.ViewModels;

public class ItemRegistrationViewModel : BindableBase, INavigationAware
{
	private string _itemName;
	public string ItemName
	{
		get => _itemName;
		set => SetProperty(ref _itemName, value);
	}

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