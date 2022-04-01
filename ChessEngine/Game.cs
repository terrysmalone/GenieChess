using System.Reflection;
using System.Runtime.CompilerServices;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using Logging;
using ResourceLoading;

[assembly: InternalsVisibleTo("ChessEngineTests")]
namespace ChessEngine;

public class Game
{
    private MoveGeneration _moveGeneration;

    private readonly ILog _log;

    private readonly Board _currentBoard;

    private readonly PieceMover _pieceMover;

    private readonly IOpeningBook _openingBook;

    private bool _gameIsActive = true;

    private readonly IScoreCalculator _scoreCalculator;

    public bool UseOpeningBook { get; set; }

    public string OpeningBookFile { get; set; } = string.Empty;

    public bool UseIterativeDeepening { get; set; } = true;

    public int ThinkingDepth { get; set; }

    private List<GameTurn> _gameTurns;

    public Game(MoveGeneration moveGeneration, IScoreCalculator scoreCalculator, Board board, IOpeningBook openingBook, ILog log = null)
    {
        _moveGeneration = moveGeneration ?? throw new ArgumentNullException(nameof(moveGeneration));

        _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));

        _currentBoard = board ?? throw new ArgumentNullException(nameof(board));

        _openingBook = openingBook;

        if (openingBook is not null)
        {
            UseOpeningBook = true;
        }

        _log = log ?? new NullLogger();

        ThinkingDepth = 1;

        LookupTables.InitialiseAllTables();
        ZobristHash.Initialise();
        TranspositionTable.InitialiseTable();

        _currentBoard.InitaliseStartingPosition();

        _pieceMover = new PieceMover(_currentBoard);

        _gameTurns = new List<GameTurn>();

        _log.Info("Initialised Game to starting position");
    }

    public void Reset()
    {
        _currentBoard.InitaliseStartingPosition();
        _gameTurns = new List<GameTurn>();
        _log.Info("Reset game to starting position");
    }

    public void ReceiveMove(PieceMove move)
    {
        MakeMove(move);
    }

    public void MakeBestMove()
    {
        var currentMove = GetBestMove();

        if (currentMove.Type != PieceType.None)
        {
            MakeMove(currentMove);
        }
        else
        {
            _gameIsActive = false;
        }
    }

    public PieceMove GetBestMove()
    {
        if(UseOpeningBook && _openingBook is not null)
        {
            var openingMoveUci = _openingBook.GetMove();

            if (!string.IsNullOrEmpty(openingMoveUci))
            {
                var openingMove = UciMoveTranslator.ToGameMove(openingMoveUci, _currentBoard);

                var uciMove = UciMoveTranslator.ToUciMove(openingMove);

                _log.Info($"Move {uciMove} retrieved from opening book");

                Console.WriteLine($"info string {uciMove} retrieved from opening book");

                return openingMove;
            }

            // If the opening book didn't return any moves it means it's exhausted
            // it's possibilities. Turn it off
            UseOpeningBook = false;

            Console.WriteLine("Opening book was unable to make a move. Reverting to search");

            _log.Info("Opening book was unable to make a move. Reverting to search");
        }

        _log.Info("===============================================================");
        _log.Info($"Starting move - Thinking depth: {ThinkingDepth}");

        var search = new AlphaBetaSearch(_moveGeneration, _currentBoard, _scoreCalculator, _log);

        var bestMove = search.CalculateBestMove(ThinkingDepth);

        return bestMove;
    }

    public BoardState GetCurrentBoardState()
    {
        return _currentBoard.GetCurrentBoardState();
    }

    // TODO: This will be removed when we still board state away from board
    public Board GetCurrentBoard()
    {
        return _currentBoard;
    }

    public void WriteBoardToConsole()
    {
        const string piece = " ";

        var squares = new char[64];

        AddPieceLetterToSquares(squares, _currentBoard.WhitePawns, 'p');
        AddPieceLetterToSquares(squares, _currentBoard.BlackPawns, 'P');

        AddPieceLetterToSquares(squares, _currentBoard.WhiteKnights, 'n');
        AddPieceLetterToSquares(squares, _currentBoard.BlackKnights, 'N');

        AddPieceLetterToSquares(squares, _currentBoard.WhiteBishops, 'b');
        AddPieceLetterToSquares(squares, _currentBoard.BlackBishops, 'B');

        AddPieceLetterToSquares(squares, _currentBoard.WhiteRooks, 'r');
        AddPieceLetterToSquares(squares, _currentBoard.BlackRooks, 'R');

        AddPieceLetterToSquares(squares, _currentBoard.WhiteQueen, 'q');
        AddPieceLetterToSquares(squares, _currentBoard.BlackQueen, 'Q');

        AddPieceLetterToSquares(squares, _currentBoard.WhiteKing, 'k');
        AddPieceLetterToSquares(squares, _currentBoard.BlackKing, 'K');

        for (var rank = 7; rank >= 0; rank--)
        {
            Console.WriteLine("");
            Console.WriteLine(@"-------------------------");
            Console.Write(@"|");

            for (var file = 0; file < 8; file++)
            {
                var index = rank * 8 + file;

                if (char.IsLetter(squares[index]))
                {
                    Console.Write(squares[index]);
                }
                else
                {
                    Console.Write(@" ");
                }

                Console.Write(piece + @"|");
            }
        }

        Console.WriteLine("");
        Console.WriteLine(@"-------------------------");
    }

    private static void AddPieceLetterToSquares(IList<char> squares, ulong piecePosition, char letterToAdd)
    {
        var pieceSquares = BitboardOperations.GetSquareIndexesFromBoardValue(piecePosition);

        foreach (var pieceSquare in pieceSquares)
        {
            squares[pieceSquare] = letterToAdd;
        }
    }


    // Initialises the pieces to a games starting position
    // Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
    // therefore the far left digit is a1 and the leftmost digit is h8
    public void InitaliseStartingPosition()
    {
        _currentBoard.InitaliseStartingPosition();

        _openingBook?.ResetBook();
    }

    public void ClearBoard()
    {
        _currentBoard.ClearBoard();
    }

    public void ClearAllStorage()
    {
        ZobristHash.Restart();
        TranspositionTable.Restart();
    }

    public void AllowAllCastling(bool allow)
    {
        _currentBoard.AllowAllCastling(allow);
    }

    /// <param name="fenNotation"></param>
    public void SetPosition(string fenNotation)
    {
        _currentBoard.SetPosition(fenNotation);
    }

    public string GetPosition()
    {
        return _currentBoard.GetPosition();
    }

    public IEnumerable<PieceMove> GetValidMoves()
    {
        return _moveGeneration.CalculateAllMoves(_currentBoard);
    }

    /// <summary>
    /// Resets various flags to their defaults
    ///
    /// i.e. move count half move count
    /// allow castling
    /// </summary>
    public void ResetFlags()
    {
        _currentBoard.ResetFlags();
    }

    private void MakeMove(PieceMove move)
    {
        if (UseOpeningBook && _openingBook is not null)
        {
            // TODO: Move the UCI translator into the opening book. Game should only deal with PieceMove
            _openingBook.RegisterMadeMove(UciMoveTranslator.ToUciMove(move));
        }

        var moveString = PgnTranslator.ToPgnMove(_currentBoard, move.Position, move.Moves, move.Type, move.SpecialMove);

        if (_currentBoard.WhiteToMove)
        {
            _gameTurns.Add(new GameTurn { WhiteMove = moveString });
        }
        else
        {
            if (_gameTurns.Count == 0) // We don't always start a game from the beginning. It might start with a black move
            {
                _gameTurns.Add(new GameTurn { BlackMove = moveString });
            }
            else
            {
                _gameTurns[_gameTurns.Count-1].BlackMove = moveString;
            }
        }

        _pieceMover.MakeMove(move);
    }

    public List<GameTurn> GetGameMoves()
    {
        return _gameTurns.ToList();
    }
}

