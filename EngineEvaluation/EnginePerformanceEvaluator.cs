
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    // Runs and logs a full performance evaluation 
    public class EnginePerformanceEvaluator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<IEvaluator> m_Evaluators;
        
        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();
        
        public EnginePerformanceEvaluator(List<IEvaluator> evaluators)
        {
            if (evaluators == null)
            {
                Log.Error("Evaluators list passed into EnginePerformanceEvaluator was null");
                throw new ArgumentNullException(nameof(evaluators));
            }

            m_Evaluators = evaluators;
        }

        public void RunFullPerformanceEvaluation(int maxDepth)
        {
            foreach (var evaluator in m_Evaluators)
            {
                evaluator.Evaluate(maxDepth);
            }
        }

        private bool EvaluatePosition(TestPosition testPos, int depth)
        {
            CountDebugger.ClearAll();

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(testPos.FenPosition));

            var scoreCalc = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var timer = new Stopwatch();

            TranspositionTable.Restart();
            CountDebugger.ClearAll();

            timer.Start();

            var alphaBeta = new AlphaBetaSearch(board, scoreCalc);
            var currentMove = alphaBeta.CalculateBestMove(depth);

            timer.Stop();

            var speed = new TimeSpan(timer.Elapsed.Ticks);

            //var idInfo = alphaBeta.IdMoves;
            
            //for (int i = 0; i < depth; i++)
            //{
            //    string move = UciMoveTranslator.ToUciMove(idInfo[i].Move);

            //    LogLine(string.Format("Depth {0}", i + 1));
            //    LogLine(string.Format("Nodes evaluated:{0}, Accumulated time:{1}, Best move:{2}, Score:{3}", idInfo[i].NodesVisited, idInfo[i].AccumulatedTime, move, idInfo[i].Score));
            //}

            //LogLine("");
            //LogLine(string.Format("Total time: {0}", speed.ToString()));
            //LogLine("");

            //string bestMove = UciMoveTranslator.ToUciMove(idInfo[idInfo.Count-1].Move);
            //var bestMove = PgnTranslator.ToPgnMove(board, idInfo[idInfo.Count - 1].Move.Position, idInfo[idInfo.Count - 1].Move.Moves, idInfo[idInfo.Count - 1].Move.Type);
            
            var expectedMove = testPos.BestMovePgn;

            var pass = "FAIL";
            var passed = false;

            //if (bestMove.Equals(expectedMove))
            //{
            //    pass = "PASS";
            //    passed = true;
            //}

            //LogLine($("Depth:{depth} - Found move:{1}, Expected move:{2} - {3}", depth, , expectedMove, pass));
            return passed;        
        }
    }
}
