using System;
using ChessGame.BoardRepresentation;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using ChessGame.MoveSearching;
using ChessGame.BoardSearching;
using System.IO;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.NotationHelpers;
using log4net;
using ChessGame.Books;

namespace ChessGame
{
    public class Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region private properties

        #region Game settings

        private OpeningBook openingBook;

        private int thinkingDepth = 8;

        #endregion Game settings
        
        private bool gameIsActive = true;

        private ScoreCalculator scoreCalc;

        #endregion private properties

        #region public properties

        #region Game settings

        public bool UseOpeningBook { get; set; }

        public string OpeningBookFile { get; set; } = string.Empty;

        public bool UseIterativeDeepening { get; set; } = true;

        public int ThinkingDepth
        {
            get { return thinkingDepth; }
            set 
            {
                Log.Info($"Setting default thinking depth to {value}");
                thinkingDepth = value; 
            }
        }

        public SearchStrategy WhiteSearchType { get; set; } = SearchStrategy.AlphaBeta;

        public SearchStrategy BlackSearchType { get; set; } = SearchStrategy.AlphaBeta;

        #endregion Game settings
        
        public Board CurrentBoard { get; }

        #endregion public properties

        #region constructors

        /// <summary>
        /// Loads a game using the default score values "ScoreValues.xml"
        /// </summary>
        public Game()
        {
            Log.Info($"Loading default score set: {"ScoreValues.xml"}");

            this.scoreCalc = ResourceLoading.ResourceLoader.LoadScoreValues("ScoreValues.xml");

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            CurrentBoard = new Board();
            CurrentBoard.InitaliseStartingPosition();

            Log.Info("Initialised Game to starting position");

            LogGameSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public Game(string scoreFilePath)
        {
            Log.Info(string.Format("Loading score set: {0}", scoreFilePath));

            this.scoreCalc = new ScoreCalculator(File.ReadAllText(scoreFilePath));

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            CurrentBoard = new Board();
            CurrentBoard.InitaliseStartingPosition();

            Log.Info("Initialised Game to starting position");
            LogGameSettings();
        }

        public Game(ScoreCalculator scoreCalc)
        {
            this.scoreCalc = scoreCalc;

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            CurrentBoard = new Board();
            CurrentBoard.InitaliseStartingPosition();

            Log.Info("Initialised Game to starting position");
            LogGameSettings();
        }

        #endregion constructor

        #region UCI commands

        /// <summary>
        /// Initialise the game board and make moves
        /// </summary>
        /// <param name="moveString">The moves to make</param>
        public void ReceiveUciMoves(string moveString)
        {
            string[] moves = moveString.Split();

            foreach (string move in moves)
            {
                ReceiveUciMove(move);
            }
        }

        public void ReceiveUciMove(string move)
        {
            if (UseOpeningBook)
            {
                openingBook.RegisterMadeMove(move);
            }

            var pieceMove = UCIMoveTranslator.ToGameMove(move, CurrentBoard);

            CurrentBoard.MakeMove(pieceMove, true);
        }

        public string FindBestMove_UCI()
        {
            var bestMove = GetBestMove();

            var bestMoveString = UCIMoveTranslator.ToUCIMove(bestMove);

            return bestMoveString;
        }

        #endregion UCI commands

        public PieceMoves FindAndMakeBestMove()
        {
            var currentMove = GetBestMove();

            if (currentMove.Type != PieceType.None)
                CurrentBoard.MakeMove(currentMove, true);
            else
                gameIsActive = false;

            return currentMove;
        }

        public PieceMoves GetBestMove()
        {
            if(UseOpeningBook)
            {
                var openingMoveUci = openingBook.GetMove();

                if (!string.IsNullOrEmpty(openingMoveUci))
                {                  
                    PieceMoves openingMove = UCIMoveTranslator.ToGameMove(openingMoveUci, CurrentBoard);

                    string uciMove = UCIMoveTranslator.ToUCIMove(openingMove);
                    Log.Info($"Move {uciMove} retrieved from opening book");

                    Console.WriteLine($"info string {uciMove} retrieved from opening book");

                    return openingMove;
                }

                UseOpeningBook = false;

                Console.WriteLine("Opening book was unable to make a move. Reverting to search");
                Log.Info("Opening book was unable to make a move. Reverting to search");
            }

            PieceMoves currentMove = new PieceMoves();

            if (WhiteSearchType == SearchStrategy.MiniMax)
            {
                MiniMax miniMax = new MiniMax(CurrentBoard, scoreCalc);
                currentMove = miniMax.MoveCalculate(thinkingDepth);

            }
            else if (WhiteSearchType == SearchStrategy.NegaMax)
            {
                NegaMax negaMax = new NegaMax(CurrentBoard, scoreCalc);
                currentMove = negaMax.MoveCalculate(thinkingDepth);
            }
            else if (WhiteSearchType == SearchStrategy.AlphaBeta)
            {
                AlphaBetaSearch search = new AlphaBetaSearch(CurrentBoard, scoreCalc);

                if (UseIterativeDeepening)
                    currentMove = search.StartSearch(thinkingDepth);
                else
                    currentMove = search.MoveCalculate(thinkingDepth);

            }

            return currentMove;
        }
        
        #region Board setup methods

        /// <summary>
        /// Initialises the pieces to a games starting position
        /// Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
        /// therefore the far left digit is a1 and the leftmeos digit is h8
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
            var bookName = "book.txt";

#if UCI
            Console.WriteLine($"Loading opening book: {bookName}");               
#endif

            LoadOpeningBook(bookName);
        }

        public void LoadOpeningBook(string bookName)
        {
            try
            {
                openingBook = new OpeningBook(bookName);

                Log.Info($"Opening book {bookName} loaded");
#if UCI
                Console.WriteLine($"Opening book {bookName} loaded");               
#endif

                UseOpeningBook = true;
            }
            catch (Exception exc)
            {
                Log.Error(String.Format("Error loading opening book {0}", bookName), exc);

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
            Log.Info(string.Format("Max thinking depth:{0}", thinkingDepth));
            Log.Info(string.Format("Use iterative deepening:{0}", UseIterativeDeepening));

            Log.Info(string.Format("White search strategy:{0}", WhiteSearchType));
            Log.Info(string.Format("Black search strategy:{0}", BlackSearchType));  
        }

        #endregion logging
    }
}
