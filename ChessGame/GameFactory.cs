﻿using System;
using ChessGame.BoardRepresentation;
using ChessGame.Books;
using ChessGame.ResourceLoading;
using ChessGame.ScoreCalculation;
using log4net;

namespace ChessGame
{
    public sealed class GameFactory : IChessGameFactory
    {
        private readonly ILog m_Log;

        public GameFactory(ILog log)
        {
            m_Log = log;
        }

        public Game CreateChessGame()
        {
            var scoreCalculator = new ScoreCalculator(ResourceLoader.GetResourcePath("ScoreValues.xml"));

            var openingBook = GetOpeningBook("book.txt");
            
            return new Game(scoreCalculator, new Board(), null);
        }

        private IOpeningBook GetOpeningBook(string bookName)
        {
            IOpeningBook openingBook = null;

            try
            {
                openingBook = new OpeningBook(ResourceLoader.GetResourcePath(bookName));

#if UCI
                Console.WriteLine($"Opening book {openingBook.FilePath} loaded");
#endif
            }
            catch (Exception exc)
            {
                m_Log.Error($"Error loading opening book {bookName}", exc);

#if UCI
                Console.WriteLine($"Error loading opening book {bookName}. Exception:{exc}");               
#endif
            }

            return openingBook;
        }
    }
}
