using System.Windows;
using System.Windows.Input;

namespace Genie_WPF
{
    public partial class ChessBoardControl
    {
        public ChessBoardControl()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent ChessBoardClickEvent = EventManager.RegisterRoutedEvent("ChessBoardClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ChessBoardControl));

        public event RoutedEventHandler ChessBoardClick
        {
            add { AddHandler(ChessBoardClickEvent, value); }

            remove { RemoveHandler(ChessBoardClickEvent, value); }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChessBoardClickEvent, e));
        }
    }
}
