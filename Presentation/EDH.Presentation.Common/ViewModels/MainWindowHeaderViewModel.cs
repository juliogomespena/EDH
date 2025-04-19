namespace EDH.Presentation.Common.ViewModels;

public class MainWindowHeaderViewModel : BindableBase
{
	private string? _exhibitionName = "Julio G. Pena";
	public string? ExhibitionName
	{
		get => _exhibitionName;
		set => SetProperty(ref _exhibitionName, value);
	}	
}