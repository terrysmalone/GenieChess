using System;
using ChessGame.BoardRepresentation;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using ChessGame.MoveSearching;
using ChessGame.BoardSearching;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.NotationHelpers;
using log4net;
using ChessGame.Books;
using ChessGame.ResourceLoading;

namespace ChessGame
{
    public class Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private OpeningBook m_OpeningBook;

        private int m_ThinkingDepth = 8;
        
        private bool m_GameIsActive = true;

        private readonly IScoreCalculator m_ScoreCalculator;
        
        public bool UseOpeningBook { get; set; }

        public string OpeningBookFile { get; set; } = string.Empty;

        public bool UseIterativeDeepening { get; set; } = true;

        public int ThinkingDepth
        {
            get => m_ThinkingDepth;

            set 
            {
                Log.Info($"Setting default thinking depth to {value}");
                m_ThinkingDepth = value; 
            }
        }

        public SearchStrategy WhiteSearchType { get; set; } = SearchStrategy.AlphaBeta;

        public SearchStrategy BlackSearchType { get; set; } = SearchStrategy.AlphaBeta;
        
        public Board CurrentBoard { get; }
        
        public Game(IScoreCalculator scoreCalculator)
        {
            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
            
            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            CurrentBoard = new Board();
            CurrentBoard.InitaliseStartingPosition();

            Log.Info("Initialised Game to starting position");

            LogGameSettings();
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
            if (UseOpeningBook)
            {
                m_OpeningBook.RegisterMadeMove(move);
            }

            var pieceMove = UCIMoveTranslator.ToGameMove(move, CurrentBoard);

            CurrentBoard.MakeMove(pieceMove, true);
        }

        #endregion UCI commands

        public PieceMoves FindAndMakeBestMove()
        {
            var currentMove = GetBestMove();

            if (currentMove.Type != PieceType.None)
                CurrentBoard.MakeMove(currentMove, true);
            else
                m_GameIsActive = false;

            return currentMove;
        }

        public PieceMoves GetBestMove()
        {
            if(UseOpeningBook)
            {
                var openingMoveUci = m_OpeningBook.GetMove();

                if (!string.IsNullOrEmpty(openingMoveUci))
                {                  
                    var openingMove = UCIMoveTranslator.ToGameMove(openingMoveUci, CurrentBoard);

                    var uciMove = UCIMoveTranslator.ToUCIMove(openingMove);

                    Log.Info($"Move {uciMove} retrieved from opening book");

                    Console.WriteLine($"info string {uciMove} retrieved from opening book");

                    return openingMove;
                }

                UseOpeningBook = false;

                Console.WriteLine("Opening book was unable to make a move. Reverting to search");

                Log.Info("Opening book was unable to make a move. Reverting to search");
            }

            var currentMove = new PieceMoves();

            if (WhiteSearchType == SearchStrategy.MiniMax)
            {
                var miniMax = new MiniMax(CurrentBoard, m_ScoreCalculator);

                currentMove = miniMax.MoveCalculate(m_ThinkingDepth);

            }
            else if (WhiteSearchType == SearchStrategy.NegaMax)
            {
                var negaMax = new NegaMax(CurrentBoard, m_ScoreCalculator);

                currentMove = negaMax.MoveCalculate(m_ThinkingDepth);
            }
            else if (WhiteSearchType == SearchStrategy.AlphaBeta)
            {
                var search = new AlphaBetaSearch(CurrentBoard, m_ScoreCalculator);

                currentMove = UseIterativeDeepening ? search.StartSearch(m_ThinkingDepth) 
                                                    : search.MoveCalculate(m_ThinkingDepth);
            }

            return currentMove;
        }
        
        #region Board setup methods

        /// <summary>
        /// Initialises the pieces to a games starting position
        /// Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
        /// therefore the far left digit is a1 and the leftmost digit is h8
        /// </summary>
        public void InitaliseStartingPosition()
        {
            CurrentBoard.InitaliseStartingPosition();
        }

        /// <summary>
        /// Removes all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            CurrentBoard.ClearBoard();
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
            CurrentBoard.PlacePiece(typeToPlace, colour, file, rank);
        }

        public void AllowAllCastling(bool allow)
        {
            CurrentBoard.AllowAllCastling(allow);
        }

        /// <param name="fenNotation"></param>
        public void SetFENPosition(string fenNotation)
        {
            CurrentBoard.SetPosition(FenTranslator.ToBoardState(fenNotation));
        }

        #endregion  Board setup methods
        
        public void SetSearchType(SearchStrategy searchType)
        {
            WhiteSearchType = searchType;
            BlackSearchType = searchType;
        }

        /// <summary>
        /// Resets various flags to their defaults 
        /// 
        /// i.e. move count half move count
        /// allow castling
        /// </summary>
        internal void ResetFlags()
        {
            CurrentBoard.ResetFlags();
        }

        #region opening book

        public void LoadDefaultOpeningBook()
        {
            var bookFile = ResourceLoader.GetResourcePath("book.txt");

#if UCI
            Console.WriteLine($"Loading opening book: {bookFile}");               
#endif

            LoadOpeningBook(bookFile);
        }

        public void LoadOpeningBook(string bookFile)
        {
            try
            {
                m_OpeningBook = new OpeningBook(bookFile);

                Log.Info($"Opening book {bookFile} loaded");
#if UCI
                Console.WriteLine($"Opening book {bookName} loaded");               
#endif

                UseOpeningBook = true;
            }
            catch (Exception exc)
            {
                Log.Error($"Error loading opening book {bookFile}", exc);

#if UCI
                Console.WriteLine($"Error loading opening book {bookName}. Exception:{exc}");               
#endif

                UseOpeningBook = false;
            }
        }

        #endregion opening book

        #region logging

        private void LogGameSettings()
        {
            Log.Info($"Thinking depth:{m_ThinkingDepth}");
            Log.Info($"Use iterative deepening:{UseIterativeDeepening}");

            Log.Info($"White search strategy:{WhiteSearchType}");
            Log.Info($"Black search strategy:{BlackSearchType}");  
        }

        #endregion logging
    }
}
