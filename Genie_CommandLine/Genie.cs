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

        private readonly Game _game;

        public Genie()
        {
            log4net.Config.XmlConfigurator.Configure();

            Log.Info("==============================================================");
            Log.Info("");
            Log.Info("Running Genie - Command-line version");
            Log.Info("");

            var chessGameFactory = new GameFactory(null); //TODO: pass in the logger

            Log.Info($"useOpeningBook: {false}");

            _game = chessGameFactory.CreateChessGame(false);

            _game.ThinkingDepth = 6;

            _game.InitaliseStartingPosition();

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
                        _game.WriteBoardToConsole();
                    }
                    else if (moveText == "genie" || moveText == "g")
                    {
                        var bestMove = UciMoveTranslator.ToUciMove(_game.GetBestMove());

                        Console.WriteLine($"Computer move: {bestMove}");

                        _game.ReceiveUciMove(bestMove);
                    }
                    else if (moveText.StartsWith("set"))
                    {
                        var toSet = moveText.Substring(4, moveText.Length-4);

                        if (toSet.StartsWith("ply"))
                        {
                            var plyToSet = toSet.Substring(4, toSet.Length-4);

                            var ply = Convert.ToInt32(plyToSet);

                            _game.ThinkingDepth = ply;
                        }
                        else if (toSet.StartsWith("position"))
                        {
                            var positionToSet = toSet.Substring(9, toSet.Length-9);

                            _game.SetPosition(positionToSet);
                        }
                    }
                    else if (moveText.StartsWith("info"))
                    {
                        DisplayGameInfo();
                    }
                    else
                    {
                        _game.ReceiveUciMove(moveText);

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
            Console.WriteLine($"Search ply:{_game.ThinkingDepth}");
            Console.WriteLine($"Opening book file:{_game.OpeningBookFile}");
            Console.WriteLine("------------");
        }
    }
}

