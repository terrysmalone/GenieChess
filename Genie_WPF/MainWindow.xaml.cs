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

            LookupTables.InitialiseAllTables();

            var resourceLoader = new ResourceLoader();
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            //var boardState = FenTranslator.ToBoardState("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

            _boardViewModel = new BoardViewModel(game);
            DataContext = _boardViewModel;
        }
        private void Test(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition((IInputElement)sender);

            _boardViewModel.BoardClicked((int)position.X, (int)position.Y);

        }
    }
}
