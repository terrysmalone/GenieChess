using System;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    class Program
    {
        private static readonly IResourceLoader m_ResourceLoader = new ResourceLoader();
        
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var performanceEvaluatorFactory = new PerformanceEvaluatorFactory();

            var engineEvaluation = performanceEvaluatorFactory.CreatePerformanceEvaluator();
            
            engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 6);

            //eval.EvaluateMoveSpeed(6, 10);

            //eval.EvaluateTestPositions(depth);

            //EvaluatePerfTSpeeds();

            //EvaluateNodeCounts();

            //EvaluateMoveTimes();

            //EvaluateEngine();

            //ArrayTest();

            //ProfileMoveGeneration();
        }

        private static void ProfileMoveGeneration()
        {
            var perftPositions = m_ResourceLoader.LoadPerfTPositions();

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

        //private static void EvaluatePerfTSpeeds()
        //{
        //    var peftEvaluator = new PerfTEvaluator();

        //    var minDepth = 1;
        //    var maxDepth = 9;
        //    var repeatCount = 1;
        //    var useHashing = true;

        //    peftEvaluator.EvaluatePerft(minDepth, maxDepth, repeatCount, useHashing);
        //}
        
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
    }
}
