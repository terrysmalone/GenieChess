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
    internal class Program
    {
        private static readonly ILog Log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            Log.Info("==============================================================");
            Log.Info("");
            Log.Info("Running Genie - Command-line version");
            Log.Info("");

            var game = new Game();
            
            game.SetSearchType(SearchStrategy.AlphaBeta);
            game.ThinkingDepth = 7;

            game.InitaliseStartingPosition();

            game.LoadDefaultOpeningBook();
            //game.SetFENPosition("4k3/2p5/4P3/8/8/1B4B1/8/4K3 w - - 0 1");
            CountDebugger.ClearAll();

            while (true)
            {
                Console.Write("?:");
                string move = Console.ReadLine();

                if (!string.IsNullOrEmpty(move))
                {
                    if (move == "-help" || move == "-?" || move == "help" || move == "?")
                    {
                        DisplayHelp();
                    }
                    else if (move == "print")
                    {
                        game.CurrentBoard.WriteBoardToConsole();
                    }
                    else if (move == "genie" || move == "g")
                    {
                        var bestMove = game.FindBestMove_UCI();
                        Console.WriteLine($"Computer move: {bestMove}");
                        game.ReceiveUciMove(bestMove);
                    }
                    else
                    {
                        game.ReceiveUciMove(move);
                        Console.WriteLine("Made move");
                    }
                }
            }

        }

        private static void DisplayHelp()
        {
            Console.WriteLine("------------");
            Console.WriteLine("print - displays the board");
            Console.WriteLine("genie - makes computer take next move");
            Console.WriteLine("[Smith notation move] - plays a move (i.e 'e2e4')");
        }
    }
}
