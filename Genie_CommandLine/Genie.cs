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
                    else if (moveText.StartsWith("set"))
                    {
                        var toSet = moveText.Substring(4, moveText.Length-4);

                        if (toSet.StartsWith("ply"))
                        {
                            var plyToSet = toSet.Substring(4, toSet.Length-4);

                            var ply = Convert.ToInt32(plyToSet);

                            m_Game.ThinkingDepth = ply;
                        }
                        else if (toSet.StartsWith("position"))
                        {
                            var positionToSet = toSet.Substring(9, toSet.Length-9);

                            m_Game.SetPosition(positionToSet);
                        }
                    }
                    else if (moveText.StartsWith("info"))
                    {
                        DisplayGameInfo();
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
            Console.WriteLine("info - displays game setup info");
            Console.WriteLine("set ply [x] - Sets the thinking depth");
            Console.WriteLine("set position [fen position] - Sets the game to the given fen position");
            Console.WriteLine("genie(g) - makes computer take next move");
            Console.WriteLine("[Smith notation move] - plays a move (i.e 'e2e4')");
            Console.WriteLine("------------");
        }

        private void DisplayGameInfo()
        {
            Console.WriteLine("------------");
            Console.WriteLine($"Search ply:{m_Game.ThinkingDepth}");
            Console.WriteLine($"Opening book file:{m_Game.OpeningBookFile}");
            Console.WriteLine("------------");
        }
    }
}

