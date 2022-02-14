using System;
using System.Windows;
using System.Windows.Input;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using ResourceLoading;

namespace Genie_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        BoardViewModel _boardViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ChessBoard.AddHandler(ChessBoardControl.GreetEvent, new RoutedEventHandler(myCustomGreeter));

            LookupTables.InitialiseAllTables();

            var scoreCalculator = ScoreCalculatorFactory.Create();
            var game = new Game(scoreCalculator, new Board(), null);

            _boardViewModel = new BoardViewModel(game);
            DataContext = _boardViewModel;
        }

        void myCustomGreeter(object sender, RoutedEventArgs e)
        {
            var position = ((MouseEventArgs)e.OriginalSource).GetPosition((IInputElement)sender);

            _boardViewModel.BoardClicked((int)position.X, (int)position.Y);

            e.Handled = true;

        }
    }
}
