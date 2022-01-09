using System;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace ChessEngine
{
    public class Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PieceMover _pieceMover;
        
        private readonly IOpeningBook openingBook;

        private bool _gameIsActive = true;
        
        private Board currentBoard;

        private readonly IScoreCalculator scoreCalculator;
        
        public bool UseOpeningBook { get; set; }

        public string OpeningBookFile { get; set; } = string.Empty;

        public bool UseIterativeDeepening { get; set; } = true;
        
        public int ThinkingDepth { get; set; }
        
        public Game(IScoreCalculator scoreCalculator, Board board, IOpeningBook openingBook)
        {
            this.scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));

            currentBoard = board ?? throw new ArgumentNullException(nameof(board));

            this.openingBook = openingBook;

            if (openingBook != null)
            {
                UseOpeningBook = true;
            }

            ThinkingDepth = 1;

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            currentBoard.InitaliseStartingPosition();

            _pieceMover = new PieceMover(currentBoard);

            Log.Info("Initialised Game to starting position");
        }

        #region UCI commands

        /// <summary>
        /// Initialise the game board and make moves
        /// </summary>
        /// <param name="moveString">The moves to make</param>
        public void ReceiveUciMoves(string moveString)
        {
            var moves = moveString.Split();

            foreach (var move in moves)
            {
                ReceiveUciMove(move);
            }
        }

        public void ReceiveUciMove(string move)
        {
            if (UseOpeningBook && openingBook != null)
            {
                openingBook.RegisterMadeMove(move);
            }

            var pieceMove = UciMoveTranslator.ToGameMove(move, currentBoard);

            _pieceMover.MakeMove(pieceMove, true);
        }

        #endregion UCI commands

        public PieceMoves FindAndMakeBestMove()
        {
            var currentMove = GetBestMove();

            if (currentMove.Type != PieceType.None)
            {
                _pieceMover.MakeMove(currentMove, true);
            }
            else
            {
                _gameIsActive = false;
            }

            return currentMove;
        }

        public PieceMoves GetBestMove()
        {
            if(UseOpeningBook && openingBook != null)
            {
                var openingMoveUci = openingBook.GetMove();

                if (!string.IsNullOrEmpty(openingMoveUci))
                {                  
                    var openingMove = UciMoveTranslator.ToGameMove(openingMoveUci, currentBoard);

                    var uciMove = UciMoveTranslator.ToUciMove(openingMove);

                    Log.Info($"Move {uciMove} retrieved from opening book");

                    Console.WriteLine($"info string {uciMove} retrieved from opening book");

                    return openingMove;
                }

                // If the opening book didn't return any moves it means it's exhausted
                // it's possibilities. Turn it off
                UseOpeningBook = false;

                Console.WriteLine("Opening book was unable to make a move. Reverting to search");

                Log.Info("Opening book was unable to make a move. Reverting to search");
            }

            Log.Info("===============================================================");
            Log.Info($"Starting move - Thinking depth: {ThinkingDepth}");

            var search = new AlphaBetaSearch(currentBoard, scoreCalculator);

            var bestMove = search.CalculateBestMove(ThinkingDepth);

            //TranspositionTable.ClearAll();

            return bestMove;
        }

        public BoardState GetCurrentBoardState()
        {
            return currentBoard.GetCurrentBoardState();
        }

        // TODO: I will split this logic out so that the request doesn't have to be passed down
        public void WriteBoardToConsole()
        {
            currentBoard.WriteBoardToConsole();
        }
        
        // Initialises the pieces to a games starting position
        // Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
        // therefore the far left digit is a1 and the leftmost digit is h8
        public void InitaliseStartingPosition()
        {
            currentBoard.InitaliseStartingPosition();

            if (openingBook != null)
            {
                openingBook.ResetBook();
            }
        }
        
        public void ClearBoard()
        {
            currentBoard.ClearBoard();
        }
        
        public void ClearAllStorage()
        {
            ZobristHash.Restart();
            TranspositionTable.Restart();
        }

        public void AllowAllCastling(bool allow)
        {
            currentBoard.AllowAllCastling(allow);
        }

        /// <param name="fenNotation"></param>
        public void SetFENPosition(string fenNotation)
        {
            currentBoard.SetPosition(FenTranslator.ToBoardState(fenNotation));
        }
        
        /// <summary>
        /// Resets various flags to their defaults 
        /// 
        /// i.e. move count half move count
        /// allow castling
        /// </summary>
        internal void ResetFlags()
        {
            currentBoard.ResetFlags();
        }
    }
}
