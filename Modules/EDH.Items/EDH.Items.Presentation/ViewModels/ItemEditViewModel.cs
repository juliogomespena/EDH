﻿namespace EDH.Items.Presentation.ViewModels;

public sealed class ItemEditViewModel : BindableBase, INavigationAware
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