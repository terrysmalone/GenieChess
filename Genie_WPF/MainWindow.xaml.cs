using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using Logging;

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
            ChessBoard.AddHandler(ChessBoardControl.ChessBoardClickEvent, new RoutedEventHandler(ChessBoardClicked));

            LookupTables.InitialiseAllTables();

            var moveGeneration = new MoveGeneration();
            var scoreCalculator = ScoreCalculatorFactory.Create();

            var logger = new SerilogLog(Path.Combine(Environment.CurrentDirectory, "Debug.log"));

            var game = new Game(moveGeneration, scoreCalculator, new Board(), null, logger);

            _boardViewModel = new BoardViewModel(game);
            DataContext = _boardViewModel;
        }

        private void ChessBoardClicked(object sender, RoutedEventArgs e)
        {
            var position = ((MouseEventArgs)e.OriginalSource).GetPosition((IInputElement)sender);

            // The number comes out different than when it's sender was the Canvas (now it's the ChessBoardControl).
            // We have to offset it now. There must be a way to fix this
            var x = (int)((position.X - 22) / 100);
            var y = (int)((position.Y - 22) / 100);

            _boardViewModel.BoardClicked(x, y);

            e.Handled = true;

        }
    }
}
