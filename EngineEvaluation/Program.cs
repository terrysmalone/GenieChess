using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardSearching;
using log4net;
using ChessGame.ResourceLoading;
using ChessGame.PossibleMoves;
using ChessGame.BoardRepresentation;

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
            List<PerfTPosition> perftPositions = ResourceLoader.LoadPerfTPositions();

            foreach (PerfTPosition pos in perftPositions)
            {
                Board board = new Board();
                board.SetFENPosition(pos.FenPosition);

                List<PieceMoves> moves = MoveGeneration.CalculateAllPseudoLegalMoves(board);  
                
                foreach (PieceMoves move in moves)
	            {
                    board.MakeMove(move, false);

                    MoveGeneration.ValidateMove(board);

                    board.UnMakeLastMove();
	            }                
            }
        }
       
        private static void ArrayTest()
        {
            ulong[] smallArray = new ulong[1000000];
            ulong[] largeArray = new ulong[10000000];

            Random rand = new Random();

            Stopwatch timer = new Stopwatch();

            timer.Start();

            for (int i = 0; i < 10000000; i++)
            {
                int pos = rand.Next(1000000);

                ulong result = smallArray[pos];
            }

            timer.Stop();

            TimeSpan smallSpeed = new TimeSpan(timer.Elapsed.Ticks);

            timer.Restart();

            timer.Start();

            for (int i = 0; i < 10000000; i++)
            {
                int pos = rand.Next(10000000);

                ulong result = largeArray[pos];
            }

            timer.Stop();

            TimeSpan largeSpeed = new TimeSpan(timer.Elapsed.Ticks);

        }

        private static void EvaluatePerfTSpeeds()
        {
            PerfTEvaluator peftEvaluator = new PerfTEvaluator();

            int minDepth = 1;
            int maxDepth = 9;
            int repeatCount = 1;
            bool useHashing = true;

            peftEvaluator.EvaluatePerft(minDepth, maxDepth, repeatCount, useHashing);
        }
        
        private static void EvaluateNodeCounts()
        {
            NodeCountEvaluator nodeEval = new NodeCountEvaluator();

            int minDepth = 8;
            int maxDepth = 8;

            nodeEval.EvaluateNodes(minDepth, maxDepth);
        }

        private static void EvaluateMoveTimes()
        {
            int depth = 7;
            int numOfMoves = 20;

            MoveTimeEvaluator eval = new MoveTimeEvaluator();
            eval.EvaluateMoveTime(depth, numOfMoves);
        }


        private static void EvaluateEngine()
        {
            EnginePerformanceEvaluator eval = new EnginePerformanceEvaluator();
            //eval.EvaluateMoveSpeed(6, 10);

            int depth = 7;
            eval.EvaluateTestPositions(depth);
        }
    }
}
