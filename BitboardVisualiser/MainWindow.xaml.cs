using System;
using System.Windows;
using System.Windows.Input;

namespace BitboardVisualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitboardVisualiserViewModel _bitboardViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _bitboardViewModel = new BitboardVisualiserViewModel();
            DataContext = _bitboardViewModel;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
