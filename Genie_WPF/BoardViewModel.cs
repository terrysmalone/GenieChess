using System;
using System.Collections.ObjectModel;
using System.Windows;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;

namespace Genie_WPF
{
    public sealed class BoardViewModel : ViewModelBase
    {

        private ObservableCollection<ChessPiece> _chessPieces;
        private string _fenPosition;

        private BoardState _boardState;

        private readonly DelegateCommand _setFenButtonClickCommand;
        private readonly DelegateCommand _getFenButtonClickCommand;

        public DelegateCommand SetFenButtonClickCommand
        {
            get { return _setFenButtonClickCommand; }
        }

        public DelegateCommand GetFenButtonClickCommand
        {
            get { return _getFenButtonClickCommand; }
        }

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

            _setFenButtonClickCommand = new DelegateCommand(SetBoard);
            _getFenButtonClickCommand = new DelegateCommand(GetFen);

        }

        public void AddPiece(object whitePawn, string position)
        {
            
        }

        internal void SetBoard(object obj = null)
        {
            try
            {
                var boardState = FenTranslator.ToBoardState(_fenPosition);

                _chessPieces.Clear();
                SetBoard(boardState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Maybe stick up a warning
            }
        }

        internal void SetBoard(BoardState boardState)
        {
            _chessPieces.Clear();

            foreach (var whitePawnMove in BitboardOperations.SplitBoardToArray(boardState.WhitePawns))
            {
                AddPiece(Player.White, PieceType.Pawn, whitePawnMove);
            }
            
            foreach (var whiteKnightMove in BitboardOperations.SplitBoardToArray(boardState.WhiteKnights))
            {
                AddPiece(Player.White, PieceType.Knight, whiteKnightMove);
            }
            
            foreach (var whiteBishopMove in BitboardOperations.SplitBoardToArray(boardState.WhiteBishops))
            {
                AddPiece(Player.White, PieceType.Bishop, whiteBishopMove);
            }
            
            foreach (var whiteRookMove in BitboardOperations.SplitBoardToArray(boardState.WhiteRooks))
            {
                AddPiece(Player.White, PieceType.Rook, whiteRookMove);
            }
            
            foreach (var whiteQueenMove in BitboardOperations.SplitBoardToArray(boardState.WhiteQueen))
            {
                AddPiece(Player.White, PieceType.Queen, whiteQueenMove);
            }
            
            foreach (var whiteKingMove in BitboardOperations.SplitBoardToArray(boardState.WhiteKing))
            {
                AddPiece(Player.White, PieceType.King, whiteKingMove);
            }
            
            foreach (var blackPawnMove in BitboardOperations.SplitBoardToArray(boardState.BlackPawns))
            {
                AddPiece(Player.Black, PieceType.Pawn, blackPawnMove);
            }
            
            foreach (var blackKnightMove in BitboardOperations.SplitBoardToArray(boardState.BlackKnights))
            {
                AddPiece(Player.Black, PieceType.Knight, blackKnightMove);
            }
            
            foreach (var blackBishopMove in BitboardOperations.SplitBoardToArray(boardState.BlackBishops))
            {
                AddPiece(Player.Black, PieceType.Bishop, blackBishopMove);
            }
            
            foreach (var blackRookMove in BitboardOperations.SplitBoardToArray(boardState.BlackRooks))
            {
                AddPiece(Player.Black, PieceType.Rook, blackRookMove);
            }
            
            foreach (var blackQueenMove in BitboardOperations.SplitBoardToArray(boardState.BlackQueen))
            {
                AddPiece(Player.Black, PieceType.Queen, blackQueenMove);
            }
            
            foreach (var blackKingMove in BitboardOperations.SplitBoardToArray(boardState.BlackKing))
            {
                AddPiece(Player.Black, PieceType.King, blackKingMove);
            }

            _boardState = boardState;

            FenPosition = FenTranslator.ToFenString(boardState);
        }

        private void AddPiece(Player player, PieceType pieceType, ulong pieceMove)
        {
            var position = TranslationHelper.GetPosition(pieceMove);

            //Normalise the positions
            var column = position.Item1 - 1;
            var row = 8 - position.Item2 - 1;

            _chessPieces.Add(new ChessPiece{ Pos = new Point(column, row),
                                                 Type = pieceType,
                                                 Player = player });
        }

        public void GetFen(object obj = null)
        {
        }
    }
}
