using System;
using ChessEngine;
using ChessEngine.Debugging;
using ChessEngine.NotationHelpers;
using log4net;

namespace Genie_CommandLine
{
    internal sealed class Genie
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Game m_Game;

        public Genie()
        {
            log4net.Config.XmlConfigurator.Configure();

            Log.Info("==============================================================");
            Log.Info("");
            Log.Info("Running Genie - Command-line version");
            Log.Info("");

            var chessGameFactory = new GameFactory(null); //TODO: pass in the logger

            var useOpeningBook = false;

            Log.Info($"useOpeningBook: {useOpeningBook}");

            m_Game = chessGameFactory.CreateChessGame(useOpeningBook);

            m_Game.ThinkingDepth = 6;

            //m_Game.SetFENPosition("rnbqkb1r/pppp1ppp/4pn2/8/3P4/2N5/PPP1PPPP/R1BQKBNR w KQkq - 1 3");   //It seems to play a weird move
            //m_Game.SetFENPosition("r1bqk2r/ppNp1ppp/2nbp3/8/3PnB2/7N/PPP1PPPP/R2QKB1R b KQkq - 0 6");

            m_Game.InitaliseStartingPosition();

            CountDebugger.ClearAll();
            
            while (true)
            {
                Console.Write("?:");
                var moveText = Console.ReadLine();

                if (!string.IsNullOrEmpty(moveText))
                {
                    if (moveText == "-help" || moveText == "-?" || moveText == "help" || moveText == "?")
                    {
                        DisplayHelp();
                    }
                    else if (moveText == "print")
                    {
                        m_Game.WriteBoardToConsole();
                    }
                    else if (moveText == "genie" || moveText == "g")
                    {
                        var bestMove = UciMoveTranslator.ToUciMove(m_Game.GetBestMove());

                        Console.WriteLine($"Computer move: {bestMove}");

                        m_Game.ReceiveUciMove(bestMove);
                    }
                    else if (moveText.StartsWith("info"))
                    {
                        DisplayGameInfo(moveText.Substring(5, moveText.Length-5));
                    }
                    else
                    {
                        m_Game.ReceiveUciMove(moveText);

                        Console.WriteLine("Made move");
                    }
                }
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("------------");
            Console.WriteLine("print - displays the board");
            Console.WriteLine("info [subject] - displays game setup info. Current subjects: book");
            Console.WriteLine("genie - makes computer take next move");
            Console.WriteLine("[Smith notation move] - plays a move (i.e 'e2e4')");
            Console.WriteLine("------------");
        }

        private void DisplayGameInfo(string toDisplay)
        {
            switch (toDisplay)
            {
                case "book":
                    Console.WriteLine($"Opening book file:{m_Game.OpeningBookFile}");
                    break;
            }
        }
    }
}

