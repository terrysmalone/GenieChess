using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Properties;
using ChessGame.ResourceLoading;
using ChessGame.ScoreCalculation;
using ChessGame.NotationHelpers;

namespace ChessGame
{
    /// <summary>
    /// Deals with the UCICommunication for interfacing with GUI's
    /// </summary>
    public class UCI
    {
        private static string ENGINE_NAME = "Genie v0.01";

        private Game game;

        public UCI()
        {

        }

        public void UCICommunication()
        {
            while (true)
            {
                String input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {

                    if (input.Equals("uci"))
                    {
                        InputUCI();
                    }
                    else if (input.StartsWith("ucinewgame"))
                    {
                        InputUCINewGame();
                    }
                    else if (input.StartsWith("setoption"))
                    {
                        InputSetOption(input);
                    }
                    else if (input.Equals("isready"))
                    {
                        InputIsReady();
                    }
                    else if (input.StartsWith("position"))
                    {
                        InputPosition(input);
                    }
                    else if (input.StartsWith("go"))
                    {
                        InputGo(input);
                    }
                }
            }
        }

        private void InputUCI()
        {
            Console.WriteLine("id name " + ENGINE_NAME);
            Console.WriteLine("id author Terry Malone");

            //Options go here

            Console.WriteLine("uciok");

            InputUCINewGame();
        }

        private void InputSetOption(String inputString)
        {
            //Add option setup here
        }  

        /// <summary>
        /// UCI command: "isready"
        /// GUI is checking if we're ready. Tell it we are
        /// </summary>
        private void InputIsReady()
        {
            Console.WriteLine("readyok");
        }

        /// <summary>
        /// uci command "ucinewgame"
        /// New game has started reset 
        /// May want top clear/reset things here
        /// </summary>
        private void InputUCINewGame()
        {
            var scoreCalculator = new ScoreCalculator(ResourceLoader.GetResourcePath("ScoreValues.xml"));

            game = new Game(scoreCalculator);
            game.LoadDefaultOpeningBook();
            game.UseOpeningBook = true;
        }

        /// <summary>
        /// UCI command starts with "position"
        /// </summary>
        /// <param name="input"></param>
        private void InputPosition(string input)
        {
            game.ResetFlags();

            //Console.WriteLine(string.Format("DEBUGGING-position received: INPUT={0}", input));
            
            //remove position text
            input = input.Substring(9);

            //Console.WriteLine(string.Format("DEBUGGING-remove position: INPUT={0}", input));
            
            if (input.StartsWith("startpos"))
            {
                //input = input.Substring(9);

                //Console.WriteLine(string.Format("DEBUGGING-startpos detected: INPUT={0}", input));
            
                game.InitaliseStartingPosition();
                game.LoadDefaultOpeningBook();

                if (input.Contains("moves"))
                {
                    int moveStart = input.IndexOf("moves");

                    string moves = input.Substring(moveStart + 6);

                    //Console.WriteLine(string.Format("DEBUGGING-MAKING MOVE: INPUT={0}", moves));

                    game.ReceiveUciMoves(moves);
                }
            }
            else if (input.Contains("fen"))
            {                
                int fenStart = input.IndexOf("fen");
                string fen = input.Substring(fenStart + 4);

                //Console.WriteLine(string.Format("DEBUGGING-fen detected: INPUT={0}", fen));
            
                if(input.Contains("moves"))      
                {
                    //Console.WriteLine(string.Format("DEBUGGING-MAKING MOVE: INPUT={0}", input));

                    int moveStart = input.IndexOf("moves");

                    string fenPos = input.Substring(0, moveStart - 1);
                    string moves = input.Substring(moveStart+6);

                    //Console.WriteLine(string.Format("DEBUGGING-fenPos: INPUT={0}", fenPos));
                    //Console.WriteLine(string.Format("DEBUGGING-moves: INPUT={0}", moves));
            
                    game.SetFENPosition(fenPos);
                    game.ReceiveUciMoves(moves);
                }
                else
                {
                    game.SetFENPosition(input);
                }
            }            
        }

        /// <summary>
        /// UCI command starts with "go" 
        /// </summary>
        /// <param name="input"></param>
        private void InputGo(string input)
        {
            //make search on new thread so we can accept stop command
            string bestMove = game.FindBestMove_UCI();

            //PrintFEN();

            Console.WriteLine(string.Format("bestmove {0}", bestMove));

        }

        private void PrintFEN()
        {
            string fen = FenTranslator.ToFENString(game.CurrentBoard.GetCurrentBoardState());           
        }
    }
}
