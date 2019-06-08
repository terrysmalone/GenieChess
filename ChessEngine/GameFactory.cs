using System;
using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace ChessEngine
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
            var resourceLoader = new ResourceLoader();
            
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            IOpeningBook openingBook = null;

            if (useOpeningBook)
            {
                openingBook = GetOpeningBook(resourceLoader, "book.txt");
            }

            return new Game(scoreCalculator, new Board(), openingBook);
        }

        private IOpeningBook GetOpeningBook(IResourceLoader resourceLoader, string bookName)
        {
            try
            {
                var openingBook = new OpeningBook(resourceLoader.GetGameResourcePath(bookName));

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
