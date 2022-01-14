using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Genie_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            A1.Source = (System.Windows.Media.ImageSource)Resources["BlackBishop"];
        }
    }
}
