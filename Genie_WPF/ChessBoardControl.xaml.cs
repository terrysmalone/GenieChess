using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Genie_WPF
{
    public partial class ChessBoardControl
    {
        public ChessBoardControl()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent GreetEvent = EventManager.RegisterRoutedEvent("Greet", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ChessBoardControl));

        public event RoutedEventHandler Greet
        {
            add { AddHandler(ChessBoardControl.GreetEvent, value); }

            remove { RemoveHandler(ChessBoardControl.GreetEvent, value); }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChessBoardControl.GreetEvent, e));
        }
    }
}
