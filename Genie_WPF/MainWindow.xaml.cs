using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;

namespace Genie_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Dictionary<Piece, BitmapImage> _pieceBitmaps = new Dictionary<Piece, BitmapImage>();
        
        public MainWindow()
        {
            InitializeComponent();
            
            InitialiseBitmaps();
            
            LookupTables.InitialiseAllTables();
            var boardState = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            //var boardState = FenTranslator.ToBoardState("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

            var num = 3;
            var boardViewModel = new BoardViewModel();

            foreach (var whitePawnMove in BitboardOperations.SplitBoardToArray(boardState.WhitePawns))
            {
                AddPiece(Piece.WhitePawn, whitePawnMove);
            }
            
            foreach (var whiteKnightMove in BitboardOperations.SplitBoardToArray(boardState.WhiteKnights))
            {
                AddPiece(Piece.WhiteKnight, whiteKnightMove);
            }
            
            foreach (var whiteBishopMove in BitboardOperations.SplitBoardToArray(boardState.WhiteBishops))
            {
                AddPiece(Piece.WhiteBishop, whiteBishopMove);
            }
            
            foreach (var whiteRookMove in BitboardOperations.SplitBoardToArray(boardState.WhiteRooks))
            {
                AddPiece(Piece.WhiteRook, whiteRookMove);
            }
            
            foreach (var whiteQueenMove in BitboardOperations.SplitBoardToArray(boardState.WhiteQueen))
            {
                AddPiece(Piece.WhiteQueen, whiteQueenMove);
            }
            
            foreach (var whiteKingMove in BitboardOperations.SplitBoardToArray(boardState.WhiteKing))
            {
                AddPiece(Piece.WhiteKing, whiteKingMove);
            }
            
            foreach (var blackPawnMove in BitboardOperations.SplitBoardToArray(boardState.BlackPawns))
            {
                AddPiece(Piece.BlackPawn, blackPawnMove);
            }
            
            foreach (var blackKnightMove in BitboardOperations.SplitBoardToArray(boardState.BlackKnights))
            {
                AddPiece(Piece.BlackKnight, blackKnightMove);
            }
            
            foreach (var blackBishopMove in BitboardOperations.SplitBoardToArray(boardState.BlackBishops))
            {
                AddPiece(Piece.BlackBishop, blackBishopMove);
            }
            
            foreach (var blackRookMove in BitboardOperations.SplitBoardToArray(boardState.BlackRooks))
            {
                AddPiece(Piece.BlackRook, blackRookMove);
            }
            
            foreach (var blackQueenMove in BitboardOperations.SplitBoardToArray(boardState.BlackQueen))
            {
                AddPiece(Piece.BlackQueen, blackQueenMove);
            }
            
            foreach (var blackKingMove in BitboardOperations.SplitBoardToArray(boardState.BlackKing))
            {
                AddPiece(Piece.BlackKing, blackKingMove);
            }
        }

        private void AddPiece(Piece piece, ulong pieceMove)
        {
            var position = TranslationHelper.GetSquareNotation(pieceMove).ToUpper();
            var control = (Image) this.FindName(position);
            control.Source = _pieceBitmaps[piece];
        }

        private void InitialiseBitmaps()
        {
            _pieceBitmaps.Add(Piece.WhitePawn, (BitmapImage) Resources["WhitePawn"]);
            _pieceBitmaps.Add(Piece.WhiteKnight, (BitmapImage) Resources["WhiteKnight"]);
            _pieceBitmaps.Add(Piece.WhiteBishop, (BitmapImage) Resources["WhiteBishop"]);
            _pieceBitmaps.Add(Piece.WhiteRook, (BitmapImage) Resources["WhiteRook"]);
            _pieceBitmaps.Add(Piece.WhiteQueen, (BitmapImage) Resources["WhiteQueen"]);
            _pieceBitmaps.Add(Piece.WhiteKing, (BitmapImage) Resources["WhiteKing"]);
            _pieceBitmaps.Add(Piece.BlackPawn, (BitmapImage) Resources["BlackPawn"]);
            _pieceBitmaps.Add(Piece.BlackBishop, (BitmapImage) Resources["BlackBishop"]);
            _pieceBitmaps.Add(Piece.BlackKnight, (BitmapImage) Resources["BlackKnight"]);
            _pieceBitmaps.Add(Piece.BlackRook, (BitmapImage) Resources["BlackRook"]);
            _pieceBitmaps.Add(Piece.BlackQueen, (BitmapImage) Resources["BlackQueen"]);
            _pieceBitmaps.Add(Piece.BlackKing, (BitmapImage) Resources["BlackKing"]);
        }
    }
}
