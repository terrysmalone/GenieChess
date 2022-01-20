using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;

namespace Genie_WPF
{
    public sealed class BoardViewModel : ViewModelBase
    {
    private ChessPiece _selectedPiece;

        private ObservableCollection<ChessPiece> _chessPieces;
        private string _fenPosition;

        private BoardState _boardState;

        public RelayCommand SetFenButtonClickCommand { get; }

        public RelayCommand GetFenButtonClickCommand { get; }

        public ObservableCollection<ChessPiece> ChessPieces
        {
            get => _chessPieces;

            set
            {
                _chessPieces = value;
                OnPropertyChanged();
            }
        }

        public string FenPosition
        {
            get { return _fenPosition; }

            set
            {
                _fenPosition = value;
                OnPropertyChanged();
            }
        }

        public BoardViewModel()
        {
            _chessPieces = new ObservableCollection<ChessPiece>();

            SetFenButtonClickCommand = new RelayCommand(SetBoard);
            GetFenButtonClickCommand = new RelayCommand(GetFen);
        }

        internal void SetBoard(BoardState boardState)
        {
            _boardState = boardState;

            SetPieces();
        }

        internal void SetBoard(object obj = null)
        {
            try
            {
                _boardState = FenTranslator.ToBoardState(_fenPosition);

                SetPieces();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Maybe stick up a warning
            }
        }
        
        public void BoardClicked(int column, int row)
        {
            var clickedPiece = _chessPieces.SingleOrDefault(p => p.Pos.X == column && p.Pos.Y == row);

            if (clickedPiece != null)
            {
                _selectedPiece = clickedPiece;
            }
            else if (_selectedPiece != null)
            {
                _selectedPiece.Pos = new Point(column, row);

                _selectedPiece = null;
            }
        }

        private void SetPieces()
        {
            _chessPieces.Clear();

            foreach (var whitePawnMove in BitboardOperations.SplitBoardToArray(_boardState.WhitePawns))
            {
                AddPiece(Player.White, PieceType.Pawn, whitePawnMove);
            }
            
            foreach (var whiteKnightMove in BitboardOperations.SplitBoardToArray(_boardState.WhiteKnights))
            {
                AddPiece(Player.White, PieceType.Knight, whiteKnightMove);
            }
            
            foreach (var whiteBishopMove in BitboardOperations.SplitBoardToArray(_boardState.WhiteBishops))
            {
                AddPiece(Player.White, PieceType.Bishop, whiteBishopMove);
            }
            
            foreach (var whiteRookMove in BitboardOperations.SplitBoardToArray(_boardState.WhiteRooks))
            {
                AddPiece(Player.White, PieceType.Rook, whiteRookMove);
            }
            
            foreach (var whiteQueenMove in BitboardOperations.SplitBoardToArray(_boardState.WhiteQueen))
            {
                AddPiece(Player.White, PieceType.Queen, whiteQueenMove);
            }
            
            foreach (var whiteKingMove in BitboardOperations.SplitBoardToArray(_boardState.WhiteKing))
            {
                AddPiece(Player.White, PieceType.King, whiteKingMove);
            }
            
            foreach (var blackPawnMove in BitboardOperations.SplitBoardToArray(_boardState.BlackPawns))
            {
                AddPiece(Player.Black, PieceType.Pawn, blackPawnMove);
            }
            
            foreach (var blackKnightMove in BitboardOperations.SplitBoardToArray(_boardState.BlackKnights))
            {
                AddPiece(Player.Black, PieceType.Knight, blackKnightMove);
            }
            
            foreach (var blackBishopMove in BitboardOperations.SplitBoardToArray(_boardState.BlackBishops))
            {
                AddPiece(Player.Black, PieceType.Bishop, blackBishopMove);
            }
            
            foreach (var blackRookMove in BitboardOperations.SplitBoardToArray(_boardState.BlackRooks))
            {
                AddPiece(Player.Black, PieceType.Rook, blackRookMove);
            }
            
            foreach (var blackQueenMove in BitboardOperations.SplitBoardToArray(_boardState.BlackQueen))
            {
                AddPiece(Player.Black, PieceType.Queen, blackQueenMove);
            }
            
            foreach (var blackKingMove in BitboardOperations.SplitBoardToArray(_boardState.BlackKing))
            {
                AddPiece(Player.Black, PieceType.King, blackKingMove);
            }

            FenPosition = FenTranslator.ToFenString(_boardState);
        }

        private void AddPiece(Player player, PieceType pieceType, ulong pieceMove)
        {
            var (column, row) = TranslationHelper.GetPosition(pieceMove);

            //Normalise the positions
            column = column - 1;
            row = 8 - row - 1;

            _chessPieces.Add(new ChessPiece{ Pos = new Point(column, row),
                                                 Type = pieceType,
                                                 Player = player });
        }

        public void GetFen(object obj = null)
        {
            FenPosition = FenTranslator.ToFenString(_boardState);
        }
    }
}
