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

            m_Game.ThinkingDepth = 2;

            //m_Game.SetFENPosition("5r1k/8/8/R4b2/3N3P/6BK/6RR/8 w - - 0 1");
            //m_Game.SetFENPosition("r1bqkb1r/pppp1ppp/2n1pn2/8/2B5/4PN2/PPPP1PPP/RNBQK2R w KQkq - 4 4");
            //m_Game.SetFENPosition("4k3/4q3/4r3/4r3/4Q3/4R3/4R3/4K3 w - - 0 1"); //with depth 3 it should use check extensions to make the capture
           // m_Game.SetFENPosition("3k4/3r4/3r4/8/3Q4/3R4/8/3K4 w - - 0 1"); //with depth 2 it should use check extensions to make the capture
            m_Game.SetFENPosition("8/1n2k3/5q2/4N3/4p3/1QN5/3KR3/8 b - - 0 1"); 

            //m_Game.InitaliseStartingPosition();

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

