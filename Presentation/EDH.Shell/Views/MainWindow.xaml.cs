using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EDH.Presentation.Common.Resources.Converters;

namespace EDH.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

            this.SizeChanged += (s, args) =>
            {
                ContentBorder.Margin = (Thickness)converter.Convert(this.ActualWidth, typeof(Thickness), null, null);
            };
        }
    }
}
