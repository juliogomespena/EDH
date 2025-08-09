using EDH.Presentation.Common.ViewModels;
using System.Collections.ObjectModel;

namespace EDH.Presentation.Common.UIModels;

internal sealed class MenuItemModel : BaseViewModel
{
	public MenuItemModel(string iconKind, string header)
	{
		IconKind = iconKind;
		Header = header;
		IsExpanded = false;
		SubItems = new ObservableCollection<SubMenuItemModel>();
	}

	private string _iconKind = null!;
	public string IconKind
	{
		get => _iconKind;
		set => SetProperty(ref _iconKind, value);
	}

	private string _header = null!;
	public string Header
	{
		get => _header;
		set => SetProperty(ref _header, value);
	}

	private bool _isExpanded;
	public bool IsExpanded
	{
		get => _isExpanded;
		set => SetProperty(ref _isExpanded, value);
	}

	private ObservableCollection<SubMenuItemModel> _subItems = null!;
	public ObservableCollection<SubMenuItemModel> SubItems
	{
		get => _subItems;
		set => SetProperty(ref _subItems, value);
	}
}