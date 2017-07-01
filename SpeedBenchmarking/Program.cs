using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessBoardTests;
using ChessGame.BoardRepresentation;
using ChessGame.MoveSearching;
using ChessGame.ScoreCalculation;
using ChessBoardTests.Properties;
using ChessGame.Enums;
using System.Timers;
using System.Diagnostics;
using ChessGame.PossibleMoves;

namespace SpeedBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime timeStamp = DateTime.Now;
            string directory = "Speed benchmark - " + timeStamp.ToString("yy-MM-dd-HH-mm-ss");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (StreamWriter writer = new StreamWriter(directory + "\\speed.txt"))
            {
                Console.WriteLine("PerfT speeds - start");
                writer.WriteLine("PerfT speeds");
                writer.WriteLine("-----------------");
                writer.WriteLine("");

                LogPerftSpeeds(writer, 10);

                Console.WriteLine("AlphaBeta speeds - start");
               
                writer.WriteLine("AlphaBeta speeds speeds");
                writer.WriteLine("-----------------");
                writer.WriteLine("");

                LogAlphaBetaSpeeds(writer, 10);
            }
        }

        #region PerfT

        private static void LogPerftSpeeds(StreamWriter writer, int repeatCount)
        {
            //Initial position
            Console.WriteLine("LogInitial - start"); 
            string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            int depth = 4;
            ulong expectedResult = 197281;
            LogPerft(writer, startingPosition, depth, expectedResult, repeatCount);
            writer.WriteLine("-----------------");
            //Perft2
            Console.WriteLine("Perft2 - start");
            startingPosition = "8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -";
            depth = 6;
            expectedResult = 120995;
            LogPerft(writer, startingPosition, depth, expectedResult, repeatCount);
            writer.WriteLine("-----------------");
            //Perft12
            Console.WriteLine("Perft12 - start"); 
            
            startingPosition = "8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1";
            depth = 5;
            expectedResult = 206379;
            LogPerft(writer, startingPosition, depth, expectedResult, repeatCount);
            writer.WriteLine("-----------------");
            //Perft19
            Console.WriteLine("Peft19 - start");             
            startingPosition = "4k3/1P6/8/8/8/8/K7/8 w - - 0 1";
            depth = 6;
            expectedResult = 217342;
            LogPerft(writer, startingPosition, depth, expectedResult, repeatCount);
        }

        private static void LogPerft(StreamWriter writer, string startingPosition, int depth, ulong expectedResult, int repeatCount)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < repeatCount; i++)
            {
                RunPerfT(startingPosition, depth, expectedResult);
            }

            timer.Stop();

            TimeSpan averageSpeed = new TimeSpan(timer.ElapsedTicks / repeatCount);

            writer.WriteLine("Position - \t" + startingPosition);
            writer.WriteLine("Depth - \t" + depth);
            writer.WriteLine("Speed - \t" + averageSpeed.TotalSeconds + " seconds");
            writer.WriteLine();
            
        }

        private static  void RunPerfT(string boardPosition, int depth, ulong expectedResult)
        {
            Board board = new Board();
            board.SetFENPosition(boardPosition);

            PerfT perft = new PerfT();

            ulong result = perft.Perft(board, depth);

            //Confirm result
            if (expectedResult != result)
                throw new Exception();
        }

        #endregion PerfT

        private static void LogAlphaBetaSpeeds(StreamWriter writer, int repeatCount)
        {
            Console.WriteLine("LogAlphaBetaSpeed - start");
           
           //Initial position     
            Console.WriteLine("LogInitial - start");
            string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            int depth = 4;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 6;
            //LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            //writer.WriteLine("-----------------");
           
            //Perft2
            Console.WriteLine("Log Perft2 - start");
            startingPosition = "8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -";
            depth = 5;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 6;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 8;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");

            //Perft12
            Console.WriteLine("Log Perft12 - start");
            
            startingPosition = "8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1";
            depth = 5;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 6;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 8;
            //LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            //writer.WriteLine("-----------------");

            //Perft19
            Console.WriteLine("Log Perft19 - start");
            
            startingPosition = "4k3/1P6/8/8/8/8/K7/8 w - - 0 1";
            depth = 5;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 6;
            LogAlphaBeta(writer, startingPosition, depth, repeatCount);
            writer.WriteLine("-----------------");
            depth = 8;
           // LogAlphaBeta(writer, startingPosition, depth, repeatCount);
        }

        private static void LogAlphaBeta(StreamWriter writer, string startingPosition, int depth, int repeatCount)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < repeatCount; i++)
            {
                Console.WriteLine("Count: " + i);
            
                RunAlphaBeta(startingPosition, depth);
            }

            timer.Stop();

            TimeSpan averageSpeed = new TimeSpan(timer.ElapsedTicks / repeatCount);

            writer.WriteLine("Position - \t" + startingPosition);
            writer.WriteLine("Depth - \t" + depth);
            writer.WriteLine("Speed - \t" + averageSpeed.TotalSeconds + " seconds");
            writer.WriteLine();            
        }

        private static void RunAlphaBeta(string startingPosition, int depth)
        {
            //TranspositionDebugger.ClearAll();
            Board board = new Board();
            TranspositionTable.ClearAll();
            board.ClearBoard();
            board.SetFENPosition(startingPosition);

            ScoreCalculator calc = new ScoreCalculator(Resources.ScoreValues);
            
            AlphaBetaSearch search = new AlphaBetaSearch(board, calc);

            PieceMoves move = search.MoveCalculate(depth, board.MoveColour);
                        
            if (move.Type == PieceType.None)
                throw new Exception();
        }
    }
}
