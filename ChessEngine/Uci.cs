﻿using System;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using ResourceLoading;

namespace ChessEngine
{
    /// <summary>
    /// Deals with the UCICommunication for interfacing with GUI's
    /// </summary>
    public class Uci
    {
        private static string ENGINE_NAME = "Genie v0.01";

        private Game game;

        public Uci()
        {

        }

        public void UciCommunication()
        {
            while (true)
            {
                var input = Console.ReadLine();

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
            var resourceLoader = new ResourceLoader(); 

            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var openingBook = new OpeningBook(resourceLoader.GetGameResourcePath("book.txt"));

            game = new Game(scoreCalculator, new Board(), openingBook);
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
                
                if (input.Contains("moves"))
                {
                    var moveStart = input.IndexOf("moves");

                    var moves = input.Substring(moveStart + 6);

                    //Console.WriteLine(string.Format("DEBUGGING-MAKING MOVE: INPUT={0}", moves));

                    game.ReceiveUciMoves(moves);
                }
            }
            else if (input.Contains("fen"))
            {                
                var fenStart = input.IndexOf("fen");
                var fen = input.Substring(fenStart + 4);

                //Console.WriteLine(string.Format("DEBUGGING-fen detected: INPUT={0}", fen));
            
                if(input.Contains("moves"))      
                {
                    //Console.WriteLine(string.Format("DEBUGGING-MAKING MOVE: INPUT={0}", input));

                    var moveStart = input.IndexOf("moves");

                    var fenPos = input.Substring(0, moveStart - 1);
                    var moves = input.Substring(moveStart+6);

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

            var bestMove = game.GetBestMove();

            var bestMoveUci =  UciMoveTranslator.ToUciMove(bestMove);
            
            Console.WriteLine($"bestmove {bestMoveUci}");
        }

        private void PrintFEN()
        {
            var fen = FenTranslator.ToFENString(game.GetCurrentBoardState());           
        }
    }
}