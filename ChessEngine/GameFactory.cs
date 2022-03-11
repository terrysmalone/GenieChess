using ChessEngine.BoardRepresentation;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using Logging;
using ResourceLoading;

namespace ChessEngine;

public sealed class GameFactory : IChessGameFactory
{
    private readonly ILog _log;

    public GameFactory(ILog log)
    {
        _log = log;
    }

    public Game CreateChessGame(bool useOpeningBook)
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create(_log);
        var resourceLoader = new ResourceLoader();

        IOpeningBook openingBook = null;

        if (useOpeningBook)
        {
            openingBook = GetOpeningBook(resourceLoader, "book.txt");
        }

        return new Game(moveGeneration, scoreCalculator, new Board(), openingBook, _log);
    }

    private IOpeningBook GetOpeningBook(IResourceLoader resourceLoader, string bookName)
    {
        try
        {
            var openingBook = new OpeningBook(resourceLoader.GetGameResourcePath(bookName));

            _log.Info($"Opening book {openingBook.FilePath} loaded");

#if UCI
            Console.WriteLine($"Opening book {openingBook.FilePath} loaded");
#endif
            return openingBook;
        }
        catch (Exception exc)
        {
            _log.Error($"Error loading opening book {bookName}", exc);

#if UCI
            Console.WriteLine($"Error loading opening book {bookName}. Exception:{exc}");
#endif
        }

        return null;
    }
}
