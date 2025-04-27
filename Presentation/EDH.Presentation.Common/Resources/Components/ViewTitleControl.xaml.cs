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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EDH.Presentation.Common.Resources.Components
{
    /// <summary>
    /// Interaction logic for ViewTitleControl.xaml
    /// </summary>
    public partial class ViewTitleControl : UserControl
    {
        public ViewTitleControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleTextProperty =
	        DependencyProperty.Register(
		        nameof(TitleText),
		        typeof(string),
		        typeof(ViewTitleControl),
		        new PropertyMetadata(string.Empty));

        public string TitleText
        {
	        get => (string)GetValue(TitleTextProperty);
	        set => SetValue(TitleTextProperty, value);
        }
	}
}
