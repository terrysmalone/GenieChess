using System;
using ChessGame.BoardRepresentation;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using ChessGame.MoveSearching;
using ChessGame.BoardSearching;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.NotationHelpers;
using ChessGame.ResourceLoading;
using log4net;

namespace ChessGame
{
    public class Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly ResourceLoader.IOpeningBook m_OpeningBook;

        private bool m_GameIsActive = true;
        
        private IBoard m_CurrentBoard;

        private readonly IScoreCalculator m_ScoreCalculator;
        
        public bool UseOpeningBook { get; set; }

        public string OpeningBookFile { get; set; } = string.Empty;

        public bool UseIterativeDeepening { get; set; } = true;
        
        public int ThinkingDepth { get; set; }
        
        public Game(IScoreCalculator scoreCalculator, IBoard board, IOpeningBook openingBook)
        {
            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));

            m_CurrentBoard = board ?? throw new ArgumentNullException(nameof(board));

            m_OpeningBook = openingBook;

            if (openingBook != null)
            {
                UseOpeningBook = true;
            }

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            m_CurrentBoard.InitaliseStartingPosition();

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
            if (UseOpeningBook && m_OpeningBook != null)
            {
                m_OpeningBook.RegisterMadeMove(move);
            }

            var pieceMove = UCIMoveTranslator.ToGameMove(move, m_CurrentBoard);

            m_CurrentBoard.MakeMove(pieceMove, true);
        }

        #endregion UCI commands

        public PieceMoves FindAndMakeBestMove()
        {
            var currentMove = GetBestMove();

            if (currentMove.Type != PieceType.None)
            {
                m_CurrentBoard.MakeMove(currentMove, true);
            }
            else
            {
                m_GameIsActive = false;
            }

            return currentMove;
        }

        public PieceMoves GetBestMove()
        {
            if(UseOpeningBook && m_OpeningBook != null)
            {
                var openingMoveUci = m_OpeningBook.GetMove();

                if (!string.IsNullOrEmpty(openingMoveUci))
                {                  
                    var openingMove = UCIMoveTranslator.ToGameMove(openingMoveUci, m_CurrentBoard);

                    var uciMove = UCIMoveTranslator.ToUCIMove(openingMove);

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

            var search = new AlphaBetaSearch(m_CurrentBoard, m_ScoreCalculator);

            var bestMove = search.CalculateBestMove(ThinkingDepth);

            //TranspositionTable.ClearAll();

            return bestMove;
        }

        public BoardState GetCurrentBoardState()
        {
            return m_CurrentBoard.GetCurrentBoardState();
        }

        // TODO: I will split this logic out so that the request doesn't have to be passed down
        public void WriteBoardToConsole()
        {
            m_CurrentBoard.WriteBoardToConsole();
        }

        #region Board setup methods

        /// <summary>
        /// Initialises the pieces to a games starting position
        /// Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
        /// therefore the far left digit is a1 and the leftmost digit is h8
        /// </summary>
        public void InitaliseStartingPosition()
        {
            m_CurrentBoard.InitaliseStartingPosition();

            if (m_OpeningBook != null)
            {
                m_OpeningBook.ResetBook();
            }
        }

        /// <summary>
        /// Removes all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            m_CurrentBoard.ClearBoard();
        }

        /// <summary>
        /// Clears anything stored
        /// </summary>
        public void ClearAllStorage()
        {
            ZobristHash.Restart();
            TranspositionTable.Restart();
        }

        public void PlacePiece(PieceType typeToPlace, PieceColour colour, int file, int rank)
        {
            m_CurrentBoard.PlacePiece(typeToPlace, colour, file, rank);
        }

        public void AllowAllCastling(bool allow)
        {
            m_CurrentBoard.AllowAllCastling(allow);
        }

        /// <param name="fenNotation"></param>
        public void SetFENPosition(string fenNotation)
        {
            m_CurrentBoard.SetPosition(FenTranslator.ToBoardState(fenNotation));
        }

        #endregion  Board setup methods
        
        /// <summary>
        /// Resets various flags to their defaults 
        /// 
        /// i.e. move count half move count
        /// allow castling
        /// </summary>
        internal void ResetFlags()
        {
            m_CurrentBoard.ResetFlags();
        }
    }
}
