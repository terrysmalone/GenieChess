using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;

namespace Genie_WPF
{
    public sealed class BoardViewModel : ViewModelBase
    {
        private readonly Game _game;

        private ChessPiece _selectedPiece;

        private ObservableCollection<ChessPiece> _chessPieces;
        private string _fenPosition;

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

        public BoardViewModel(Game game)
        {
            _chessPieces = new ObservableCollection<ChessPiece>();

            SetFenButtonClickCommand = new RelayCommand(SetBoard);
            GetFenButtonClickCommand = new RelayCommand(GetFen);


            _game = game;

            SetPieces();
        }

        private void SetBoard(object obj = null)
        {
            try
            {
                _game.SetPosition(_fenPosition);

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
            var currentPlayer = _game.GetCurrentBoardState().WhiteToMove ? Player.White : Player.Black;


            if (_selectedPiece is not null) // A piece has been selected. Deal with the move to
            {
               // var validMoves = _game.GetCurrentBoardState()

                // Remove any existing piece
                var pieceToRemove = _chessPieces.SingleOrDefault(p => p.Pos.X == column && p.Pos.Y == row);

                if (pieceToRemove is not null)
                {
                    _chessPieces.Remove(pieceToRemove);
                }

                _selectedPiece.Pos = new Point(column, row);

                _selectedPiece.IsSelected = false;
                _selectedPiece = null;
            }
            else
            {
                var clickedPiece = _chessPieces.SingleOrDefault(p => p.Pos.X == column && p.Pos.Y == row && p.Player == currentPlayer);

                if (clickedPiece is not null)
                {
                    if (_selectedPiece != null)
                    {
                        _selectedPiece.IsSelected = false;
                    }

                    _selectedPiece = clickedPiece;
                    _selectedPiece.IsSelected = true;
                }
            }
        }

        private void SetPieces()
        {
            _chessPieces.Clear();

            var boardState = _game.GetCurrentBoardState();

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

            FenPosition = FenTranslator.ToFenString(boardState);
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
            FenPosition = FenTranslator.ToFenString(_game.GetCurrentBoardState());
        }
    }
}
