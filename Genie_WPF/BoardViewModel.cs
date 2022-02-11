using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;

namespace Genie_WPF
{
    public sealed class BoardViewModel : ViewModelBase
    {
        private readonly Game _game;

        private ChessPiece _selectedPiece;

        private ObservableCollection<ChessPiece> _chessPieces;
        private string _fenPosition;

        public ObservableCollection<MoveItem> Moves { get; } = new();

        public RelayCommand SetFenButtonClickCommand { get; }

        public RelayCommand GetFenButtonClickCommand { get; }

        public RelayCommand NewGameCommand { get; }

        public RelayCommand MakeAiMoveCommand { get; }
        

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

            NewGameCommand = new RelayCommand(StartNewGame);
            MakeAiMoveCommand = new RelayCommand(MakeAiMove);

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

        private void StartNewGame(object obj = null)
        {
            _game.Reset();
            SetPieces();

            UpdateMoves();
        }

        private void UpdateMoves()
        {
            Moves.Clear();

            var gameMoves = _game.GetGameMoves();

            for (var i = 0; i < gameMoves.Count; i++)
            {
                var move = gameMoves[i];

                Moves.Add(new MoveItem(i+1, move.WhiteMove, move.BlackMove));
            }
        }

        private void MakeAiMove(object obj = null)
        {
            _game.MakeBestMove();

             SetPieces();
             UpdateMoves();
        }
        
        public void BoardClicked(int column, int row)
        {
            var currentPlayer = _game.GetCurrentBoardState().WhiteToMove ? Player.White : Player.Black;
            
            var clickedPiece = _chessPieces.SingleOrDefault(p => p.Pos.X == column && p.Pos.Y == row && p.Player == currentPlayer);

            if (_selectedPiece is not null) // A piece has been selected. Deal with the move to
            {
                if (clickedPiece is not null && clickedPiece.Player == currentPlayer)
                {
                    _selectedPiece.IsSelected = false;
                    _selectedPiece = clickedPiece;
                    _selectedPiece.IsSelected = true;

                    return;
                }
                
                var validMoves = _game.GetValidMoves().Where(m => ConvertToPoint(m.Position) == new Point(_selectedPiece.Pos.X, _selectedPiece.Pos.Y));

                if (!validMoves.Any(v => ConvertToPoint(v.Moves) == new Point(column, row)))
                {
                    return;
                }

                var selectedMoves = validMoves.Where(v => ConvertToPoint(v.Moves) == new Point(column, row));

                if (selectedMoves.Count() == 1)
                {
                    _game.ReceiveMove(selectedMoves.Single());
                }
                else
                {
                    // There are multiple moves selected. This can only mean promotion
                    if (validMoves.Any(m => m.SpecialMove == SpecialMoveType.KnightPromotion
                                                 || m.SpecialMove == SpecialMoveType.BishopPromotion
                                                 || m.SpecialMove == SpecialMoveType.RookPromotion
                                                 || m.SpecialMove == SpecialMoveType.QueenPromotion
                                                 || m.SpecialMove == SpecialMoveType.KnightPromotionCapture
                                                 || m.SpecialMove == SpecialMoveType.BishopPromotionCapture
                                                 || m.SpecialMove == SpecialMoveType.RookPromotionCapture
                                                 || m.SpecialMove == SpecialMoveType.QueenPromotionCapture))
                    {
                        _game.ReceiveMove(selectedMoves.First());
                    }
                }

                UpdateMoves();

                _selectedPiece.Pos = new Point(column, row);
                _selectedPiece.IsSelected = false;
                _selectedPiece = null;

                // Set all pieces based on this move because it may have involved other changes (i.e castling)
                SetPieces();
            }
            else
            {
                if (clickedPiece is not null)
                {
                    _selectedPiece = clickedPiece;
                    _selectedPiece.IsSelected = true;
                }
            }
        }
        
        private static Point ConvertToPoint(ulong bitPosition)
        {
            var (column, row) = TranslationHelper.GetPosition(bitPosition);

            //Normalise the positions
            column -= 1;
            row = 8 - row - 1;

            return new Point(column, row);
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
            _chessPieces.Add(new ChessPiece{ Pos = ConvertToPoint(pieceMove),
                                                 Type = pieceType,
                                                 Player = player });
        }

        public void GetFen(object obj = null)
        {
            FenPosition = FenTranslator.ToFenString(_game.GetCurrentBoardState());
        }
    }

}
