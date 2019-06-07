using System;
using ChessGame.BoardRepresentation;
using ChessGame.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace ChessGame
{
    public sealed class GameFactory : IChessGameFactory
    {
        private readonly ILog m_Log;

        public GameFactory(ILog log)
        {
            m_Log = log;
        }

        public Game CreateChessGame(bool useOpeningBook)
        {
            var scoreCalculator = new ScoreCalculator(ResourceLoader.GetResourcePath("ScoreValues.xml"));

            IOpeningBook openingBook = null;

            if (useOpeningBook)
            {
                openingBook = GetOpeningBook("book.txt");
            }

            return new Game(scoreCalculator, new Board(), openingBook);
        }

        private IOpeningBook GetOpeningBook(string bookName)
        {
            try
            {
                var openingBook = new OpeningBook(ResourceLoader.GetResourcePath(bookName));

                m_Log.Info($"Opening book {openingBook.FilePath} loaded");

#if UCI
                Console.WriteLine($"Opening book {openingBook.FilePath} loaded");
#endif
                return openingBook;
            }
            catch (Exception exc)
            {
                m_Log.Error($"Error loading opening book {bookName}", exc);

#if UCI
                Console.WriteLine($"Error loading opening book {bookName}. Exception:{exc}");               
#endif
            }

            return null;
        }
    }
}
