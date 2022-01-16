using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            LookupTables.InitialiseAllTables();
            var boardState = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            //var boardState = FenTranslator.ToBoardState("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

            Board.ItemsSource = new ObservableCollection<ChessPiece>
            {
                new ChessPiece{Pos=new Point(0, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(1, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(2, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(3, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(4, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(5, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(6, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(7, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(0, 7), Type=PieceType.Rook, Player=Player.White},
                new ChessPiece{Pos=new Point(1, 7), Type=PieceType.Knight, Player=Player.White},
                new ChessPiece{Pos=new Point(2, 7), Type=PieceType.Bishop, Player=Player.White},
                new ChessPiece{Pos=new Point(3, 7), Type=PieceType.King, Player=Player.White},
                new ChessPiece{Pos=new Point(4, 7), Type=PieceType.Queen, Player=Player.White},
                new ChessPiece{Pos=new Point(5, 7), Type=PieceType.Bishop, Player=Player.White},
                new ChessPiece{Pos=new Point(6, 7), Type=PieceType.Knight, Player=Player.White},
                new ChessPiece{Pos=new Point(7, 7), Type=PieceType.Rook, Player=Player.White},
                new ChessPiece{Pos=new Point(0, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(1, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(2, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(3, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(4, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(5, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(6, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(7, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(0, 0), Type=PieceType.Rook, Player=Player.Black},
                new ChessPiece{Pos=new Point(1, 0), Type=PieceType.Knight, Player=Player.Black},
                new ChessPiece{Pos=new Point(2, 0), Type=PieceType.Bishop, Player=Player.Black},
                new ChessPiece{Pos=new Point(3, 0), Type=PieceType.King, Player=Player.Black},
                new ChessPiece{Pos=new Point(4, 0), Type=PieceType.Queen, Player=Player.Black},
                new ChessPiece{Pos=new Point(5, 0), Type=PieceType.Bishop, Player=Player.Black},
                new ChessPiece{Pos=new Point(6, 0), Type=PieceType.Knight, Player=Player.Black},
                new ChessPiece{Pos=new Point(7, 0), Type=PieceType.Rook, Player=Player.Black}
            };

            var boardViewModel = new BoardViewModel();

            // foreach (var whitePawnMove in BitboardOperations.SplitBoardToArray(boardState.WhitePawns))
            // {
            //     AddPiece(Piece.WhitePawn, whitePawnMove);
            // }
            //
            // foreach (var whiteKnightMove in BitboardOperations.SplitBoardToArray(boardState.WhiteKnights))
            // {
            //     AddPiece(Piece.WhiteKnight, whiteKnightMove);
            // }
            //
            // foreach (var whiteBishopMove in BitboardOperations.SplitBoardToArray(boardState.WhiteBishops))
            // {
            //     AddPiece(Piece.WhiteBishop, whiteBishopMove);
            // }
            //
            // foreach (var whiteRookMove in BitboardOperations.SplitBoardToArray(boardState.WhiteRooks))
            // {
            //     AddPiece(Piece.WhiteRook, whiteRookMove);
            // }
            //
            // foreach (var whiteQueenMove in BitboardOperations.SplitBoardToArray(boardState.WhiteQueen))
            // {
            //     AddPiece(Piece.WhiteQueen, whiteQueenMove);
            // }
            //
            // foreach (var whiteKingMove in BitboardOperations.SplitBoardToArray(boardState.WhiteKing))
            // {
            //     AddPiece(Piece.WhiteKing, whiteKingMove);
            // }
            //
            // foreach (var blackPawnMove in BitboardOperations.SplitBoardToArray(boardState.BlackPawns))
            // {
            //     AddPiece(Piece.BlackPawn, blackPawnMove);
            // }
            //
            // foreach (var blackKnightMove in BitboardOperations.SplitBoardToArray(boardState.BlackKnights))
            // {
            //     AddPiece(Piece.BlackKnight, blackKnightMove);
            // }
            //
            // foreach (var blackBishopMove in BitboardOperations.SplitBoardToArray(boardState.BlackBishops))
            // {
            //     AddPiece(Piece.BlackBishop, blackBishopMove);
            // }
            //
            // foreach (var blackRookMove in BitboardOperations.SplitBoardToArray(boardState.BlackRooks))
            // {
            //     AddPiece(Piece.BlackRook, blackRookMove);
            // }
            //
            // foreach (var blackQueenMove in BitboardOperations.SplitBoardToArray(boardState.BlackQueen))
            // {
            //     AddPiece(Piece.BlackQueen, blackQueenMove);
            // }
            //
            // foreach (var blackKingMove in BitboardOperations.SplitBoardToArray(boardState.BlackKing))
            // {
            //     AddPiece(Piece.BlackKing, blackKingMove);
            // }
        }

        // private void AddPiece(Piece piece, ulong pieceMove)
        // {
        //     var position = TranslationHelper.GetSquareNotation(pieceMove).ToUpper();
        //     var control = (Image) this.FindName(position);
        //     control.Source = _pieceBitmaps[piece];
        // }
    }
}
