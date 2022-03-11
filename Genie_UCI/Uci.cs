using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using Logging;
using ResourceLoading;

namespace Genie_UCI;

// Deals with the UCICommunication for interfacing with GUI's
internal sealed class Uci
{
    private readonly SerilogLog _log;

    private const string ENGINE_NAME = "Genie v0.01";

    private Game _game;

    internal Uci(SerilogLog log)
    {
        _log = log;
    }

    internal void UciCommunication()
    {
        while (true)
        {
            var input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input))
            {

                if (input.Equals("uci"))
                {
                    InputUci();
                }
                else if (input.StartsWith("ucinewgame"))
                {
                    InputUciNewGame();
                }
                else if (input.StartsWith("setoption"))
                {
                    InputSetOption(input);
                }
                else if (input.Equals("isready"))
                {
                    InputIsReady();
                }
                else if (input.StartsWith("position"))
                {
                    InputPosition(input);
                }
                else if (input.StartsWith("go"))
                {
                    InputGo(input);
                }
            }
        }
    }

    private void InputUci()
    {
        Console.WriteLine("id name " + ENGINE_NAME);
        Console.WriteLine("id author Terry Malone");

        //Options go here

        Console.WriteLine("uciok");

        InputUciNewGame();
    }

    private void InputSetOption(string inputString)
    {
        //Add option setup here
    }

    // UCI command: "isready"
    // GUI is checking if we're ready. Tell it we are
    private void InputIsReady()
    {
        Console.WriteLine("readyok");
    }

    // uci command "ucinewgame"
    // New game has started reset
    // May want to clear/reset things here
    private void InputUciNewGame()
    {
        // TODO: Use game factory
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create(new NullLogger());

        var resourceLoader = new ResourceLoader();
        var openingBook = new OpeningBook(resourceLoader.GetGameResourcePath("book.txt"));

        _game = new Game(moveGeneration, scoreCalculator, new Board(), openingBook, null);
    }

    /// UCI command starts with "position"
    private void InputPosition(string input)
    {
        _game.ResetFlags();

        //remove position text
        input = input.Substring(9);

        if (input.StartsWith("startpos"))
        {
            _game.InitaliseStartingPosition();

            if (input.Contains("moves"))
            {
                var moveStart = input.IndexOf("moves");

                var moves = input.Substring(moveStart + 6);

                MakeMoves(moves);
            }
        }
        else if (input.Contains("fen"))
        {
            var fenStart = input.IndexOf("fen");
            var fen = input.Substring(fenStart + 4);

            if(input.Contains("moves"))
            {
                var moveStart = input.IndexOf("moves");

                var fenPos = input.Substring(0, moveStart - 1);
                var moves = input.Substring(moveStart+6);

                _game.SetPosition(fenPos);

                MakeMoves(moves);
            }
            else
            {
                _game.SetPosition(input);
            }
        }
    }

    private void MakeMoves(string moves)
    {
        var splitMoves = moves.Split();

        foreach (var move in splitMoves)
        {
            _game.ReceiveMove(UciMoveTranslator.ToGameMove(move, _game.GetCurrentBoard()));
        }
    }

    // UCI command starts with "go"
    private void InputGo(string input)
    {
        //make search on new thread so we can accept stop command

        var bestMove = _game.GetBestMove();

        var bestMoveUci =  UciMoveTranslator.ToUciMove(bestMove);

        Console.WriteLine($"bestmove {bestMoveUci}");
    }
}
