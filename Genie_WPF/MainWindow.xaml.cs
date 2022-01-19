using System.Windows;
using System.Windows.Input;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;

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

            LookupTables.InitialiseAllTables();
            //var boardState = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var boardState = FenTranslator.ToBoardState("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

            _boardViewModel = new BoardViewModel();
            DataContext = _boardViewModel;

            _boardViewModel.SetBoard(boardState);
        }

        // TODO: Look into making this a relay command
        private void SetFenOnClick(object sender, RoutedEventArgs e)
        {
            _boardViewModel.SetBoard();
        }
        private void GetFenOnClick(object sender, RoutedEventArgs e)
        {
            _boardViewModel.GetFen();
        }
    }
}
