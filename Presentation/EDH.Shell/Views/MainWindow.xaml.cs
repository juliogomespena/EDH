using System.Windows;
using EDH.Presentation.Common.Resources.Converters;

namespace EDH.Shell.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        this.Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
		var converter = (ResolutionToMarginConverter)FindResource("ResolutionToMarginConverter");
        ContentBorder.Margin = (Thickness)converter.Convert(this.ActualWidth, typeof(Thickness), null, null);

        SizeChanged += (s, args) =>
        {
            ContentBorder.Margin = (Thickness)converter.Convert(this.ActualWidth, typeof(Thickness), null, null);
        };
	}
}

