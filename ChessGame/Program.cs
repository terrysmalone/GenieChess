using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Debugging;
using ChessGame.Enums;
using ChessGame.ResourceLoading;
using log4net;

namespace ChessGame
{
    class Program
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            log.Info("==============================================================");
            
#if UCI
            UCI uci = new UCI();
            uci.UCICommunication();
#else
            PlayOnCommandLine();
#endif
        }

        private static void PlayOnCommandLine()
        {
            Game game = new Game();
            
            game.SetSearchType(SearchStrategy.AlphaBeta);
            game.ThinkingDepth = 7;

            game.InitaliseStartingPosition();

            game.LoadDefaultOpeningBook();
            //game.SetFENPosition("4k3/2p5/4P3/8/8/1B4B1/8/4K3 w - - 0 1");
            CountDebugger.ClearAll();

            while (true)
            {
                Console.WriteLine("?:");
                string move = Console.ReadLine();

                if(move == "help")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("print - displays the board");
                    Console.WriteLine("genie - makes computer take next move");
                    Console.WriteLine("[UCI move] - plays a move");

                }
                if (move == "print")
                {
                    game.CurrentBoard.WriteBoardToConsole();
                }
                else if (move == "genie" || move == "g")
                {
                    string bestMove = game.FindBestMove_UCI();
                    Console.WriteLine(string.Format("Computer move: {0}", bestMove));
                    game.ReceiveUCIMove(bestMove);
                }
                else
                {
                    game.ReceiveUCIMove(move);
                    Console.WriteLine("Made move");                    
                }

            }
        }
    }
}
