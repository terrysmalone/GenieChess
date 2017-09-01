using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region private properties

        #region Game settings

        private bool useOpeningBook = false;
        private string openingBookFile = string.Empty;

        private OpeningBook openingBook;

        private bool useIterativeDeepening = true;

        private int thinkingDepth = 8;

        #endregion Game settings
        
        private bool gameIsActive = true;

        private Board board;
        private ScoreCalculator scoreCalc;        

        private SearchStrategy whiteSearchType = SearchStrategy.AlphaBeta;
        private SearchStrategy blackSearchType = SearchStrategy.AlphaBeta;
        
        #endregion private properties

        #region public properties

        #region Game settings

        public bool UseOpeningBook
        {
            get { return useOpeningBook; }
            set { useOpeningBook = value; }
        }

        public string OpeningBookFile
        {
            get { return openingBookFile; }
            set { openingBookFile = value; }
        }

        public bool UseIterativeDeepening
        {
            get { return useIterativeDeepening; }
            set { useIterativeDeepening = value; }
        }

        public int ThinkingDepth
        {
            get { return thinkingDepth; }
            set 
            {
                log.Info(string.Format("Setting default thinking depth to {0}", value));
                thinkingDepth = value; 
            }
        }

        public SearchStrategy WhiteSearchType
        {
            get { return whiteSearchType; }
            set { whiteSearchType = value; }
        }

        public SearchStrategy BlackSearchType
        {
            get { return blackSearchType; }
            set { blackSearchType = value; }
        }

        #endregion Game settings
        
        public Board CurrentBoard
        {
            get
            {
                return board;
            }
        }

        #endregion public properties

        #region constructors

        /// <summary>
        /// Loads a game using the default score values "ScoreValues.xml"
        /// </summary>
        public Game()
        {
            log.Info(string.Format("Loading default score set: {0}", "ScoreValues.xml"));

            this.scoreCalc = ResourceLoading.ResourceLoader.LoadScoreValues("ScoreValues.xml");

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            board = new Board();
            board.InitaliseStartingPosition();

            log.Info("Initialised Game to starting position");

            LogGameSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public Game(string scoreFilePath)
        {
            log.Info(string.Format("Loading score set: {0}", scoreFilePath));

            this.scoreCalc = new ScoreCalculator(File.ReadAllText(scoreFilePath));

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            board = new Board();
            board.InitaliseStartingPosition();

            log.Info("Initialised Game to starting position");
            LogGameSettings();
        }

        public Game(ScoreCalculator scoreCalc)
        {
            this.scoreCalc = scoreCalc;

            LookupTables.InitialiseAllTables();
            ZobristHash.Initialise();
            TranspositionTable.InitialiseTable();

            board = new Board();
            board.InitaliseStartingPosition();

            log.Info("Initialised Game to starting position");
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
            if (useOpeningBook)
            {
                openingBook.RegisterMadeMove(move);
            }

            var pieceMove = UCIMoveTranslator.ToGameMove(move, board);

            board.MakeMove(pieceMove, true);
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
                board.MakeMove(currentMove, true);
            else
                gameIsActive = false;

            return currentMove;
        }

        public PieceMoves GetBestMove()
        {
            if(useOpeningBook)
            {
                var openingMoveUci = openingBook.GetMove();

                if (!string.IsNullOrEmpty(openingMoveUci))
                {                  
                    PieceMoves openingMove = UCIMoveTranslator.ToGameMove(openingMoveUci, board);

                    string uciMove = UCIMoveTranslator.ToUCIMove(openingMove);
                    log.Info($"Move {uciMove} retrieved from opening book");

                    Console.WriteLine($"info string {uciMove} retrieved from opening book");

                    return openingMove;
                }

                useOpeningBook = false;

                Console.WriteLine("Opening book was unable to make a move. Reverting to search");
                log.Info("Opening book was unable to make a move. Reverting to search");
            }

            PieceMoves currentMove = new PieceMoves();

            if (whiteSearchType == SearchStrategy.MiniMax)
            {
                MiniMax miniMax = new MiniMax(board, scoreCalc);
                currentMove = miniMax.MoveCalculate(thinkingDepth);

            }
            else if (whiteSearchType == SearchStrategy.NegaMax)
            {
                NegaMax negaMax = new NegaMax(board, scoreCalc);
                currentMove = negaMax.MoveCalculate(thinkingDepth);
            }
            else if (whiteSearchType == SearchStrategy.AlphaBeta)
            {
                AlphaBetaSearch search = new AlphaBetaSearch(board, scoreCalc);

                if (useIterativeDeepening)
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
            board.InitaliseStartingPosition();
        }

        /// <summary>
        /// Removes all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            board.ClearBoard();
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
            board.PlacePiece(typeToPlace, colour, file, rank);
        }

        public void AllowAllCastling(bool allow)
        {
            board.AllowAllCastling(allow);
        }

        /// <param name="fenNotation"></param>
        public void SetFENPosition(string fenNotation)
        {
            CurrentBoard.SetPosition(FenTranslator.ToBoardState(fenNotation));
        }

        #endregion  Board setup methods
        
        public void SetSearchType(SearchStrategy searchType)
        {
            whiteSearchType = searchType;
            blackSearchType = searchType;
        }

        /// <summary>
        /// Resets various flags to their defaults 
        /// 
        /// i.e. move count half move count
        /// allow castling
        /// </summary>
        internal void ResetFlags()
        {
            board.ResetFlags();
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

                log.Info($"Opening book {bookName} loaded");
#if UCI
                Console.WriteLine($"Opening book {bookName} loaded");               
#endif

                useOpeningBook = true;
            }
            catch (Exception exc)
            {
                log.Error(String.Format("Error loading opening book {0}", bookName), exc);

#if UCI
                Console.WriteLine($"Error loading opening book {bookName}. Exception:{exc}");               
#endif

                useOpeningBook = false;
            }
            
            
        }


        #endregion opening book

        #region logging

        private void LogGameSettings()
        {
            log.Info(string.Format("Max thinking depth:{0}", thinkingDepth));
            log.Info(string.Format("Use iterative deepening:{0}", useIterativeDeepening));

            log.Info(string.Format("White search strategy:{0}", whiteSearchType));
            log.Info(string.Format("Black search strategy:{0}", blackSearchType));  
        }

        #endregion logging
    }
}
