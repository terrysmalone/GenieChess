using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.Debugging;
using log4net;

namespace Genie_CommandLine
{
    internal sealed class Genie
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Game game;

        public Genie()
        {

            log4net.Config.XmlConfigurator.Configure();

            Log.Info("==============================================================");
            Log.Info("");
            Log.Info("Running Genie - Command-line version");
            Log.Info("");

            game = new Game();

            game.SetSearchType(SearchStrategy.AlphaBeta);
            game.ThinkingDepth = 7;

            game.InitaliseStartingPosition();

            game.LoadDefaultOpeningBook();
            //game.SetFENPosition("4k3/2p5/4P3/8/8/1B4B1/8/4K3 w - - 0 1");
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
                        game.CurrentBoard.WriteBoardToConsole();
                    }
                    else if (moveText == "genie" || moveText == "g")
                    {
                        var bestMove = game.FindBestMove_UCI();
                        Console.WriteLine($"Computer move: {bestMove}");
                        game.ReceiveUciMove(bestMove);
                    }
                    else if (moveText == "info")
                    {
                        DisplayGameInfo();
                    }
                    else
                    {
                        game.ReceiveUciMove(moveText);
                        Console.WriteLine("Made move");
                    }
                }
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("------------");
            Console.WriteLine("print - displays the board");
            Console.WriteLine("info - displays game setup info");
            Console.WriteLine("genie - makes computer take next move");
            Console.WriteLine("[Smith notation move] - plays a move (i.e 'e2e4')");
        }

        private static void DisplayGameInfo()
        {
            throw new NotImplementedException();
        }
    }
}

