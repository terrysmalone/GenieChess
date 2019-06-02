using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.Debugging;
using ChessGame.NotationHelpers;
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

            m_Game.SetSearchType(SearchStrategy.AlphaBeta);
            m_Game.ThinkingDepth = 6;

            m_Game.InitaliseStartingPosition();
            
            CountDebugger.ClearAll();

            while (true)
            {
                Console.Write("?:");
                string moveText = Console.ReadLine();

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
                        var bestMove = UCIMoveTranslator.ToUCIMove(m_Game.GetBestMove());

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

