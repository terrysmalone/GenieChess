using System;
using System.Diagnostics;
using log4net;
using ChessGame.ResourceLoading;
using ChessGame.PossibleMoves;
using ChessGame.BoardRepresentation;
using ChessGame.NotationHelpers;

namespace EngineEvaluation
{
    class Program
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            EvaluatePerfTSpeeds();
            
            //EvaluateNodeCounts();

            //EvaluateMoveTimes();

            //EvaluateEngine();

            //ArrayTest();

            //ProfileMoveGeneration();
        }

        private static void ProfileMoveGeneration()
        {
            var perftPositions = ResourceLoader.LoadPerfTPositions();

            foreach (var pos in perftPositions)
            {
                var board = new Board();
                board.SetPosition(FenTranslator.ToBoardState(pos.FenPosition));

                var moves = MoveGeneration.CalculateAllPseudoLegalMoves(board);  
                
                foreach (var move in moves)
	            {
                    board.MakeMove(move, false);

                    MoveGeneration.ValidateMove(board);

                    board.UnMakeLastMove();
	            }                
            }
        }
       
        private static void ArrayTest()
        {
            var smallArray = new ulong[1000000];
            var largeArray = new ulong[10000000];

            var rand = new Random();

            var timer = new Stopwatch();

            timer.Start();

            for (var i = 0; i < 10000000; i++)
            {
                var pos = rand.Next(1000000);

                var result = smallArray[pos];
            }

            timer.Stop();

            var smallSpeed = new TimeSpan(timer.Elapsed.Ticks);

            timer.Restart();

            timer.Start();

            for (var i = 0; i < 10000000; i++)
            {
                var pos = rand.Next(10000000);

                var result = largeArray[pos];
            }

            timer.Stop();

            var largeSpeed = new TimeSpan(timer.Elapsed.Ticks);

        }

        private static void EvaluatePerfTSpeeds()
        {
            var peftEvaluator = new PerfTEvaluator();

            var minDepth = 1;
            var maxDepth = 9;
            var repeatCount = 1;
            var useHashing = true;

            peftEvaluator.EvaluatePerft(minDepth, maxDepth, repeatCount, useHashing);
        }
        
        private static void EvaluateNodeCounts()
        {
            var nodeEval = new NodeCountEvaluator();

            var minDepth = 8;
            var maxDepth = 8;

            nodeEval.EvaluateNodes(minDepth, maxDepth);
        }

        private static void EvaluateMoveTimes()
        {
            var depth = 7;
            var numOfMoves = 20;

            var eval = new MoveTimeEvaluator();
            eval.EvaluateMoveTime(depth, numOfMoves);
        }


        private static void EvaluateEngine()
        {
            var eval = new EnginePerformanceEvaluator();
            //eval.EvaluateMoveSpeed(6, 10);

            var depth = 7;
            eval.EvaluateTestPositions(depth);
        }
    }
}
