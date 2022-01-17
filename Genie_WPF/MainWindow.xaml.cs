using System.Windows;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;

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

            LookupTables.InitialiseAllTables();
            //var boardState = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var boardState = FenTranslator.ToBoardState("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

            var boardViewModel = new BoardViewModel();
            DataContext = boardViewModel;

            boardViewModel.SetBoard(boardState);
        }
    }
}
