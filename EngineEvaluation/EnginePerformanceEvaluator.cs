﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        List<TestPosition> bratkoKopecPositions;
        List<TestPosition> kaufmanPositions;

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();
        
        public EnginePerformanceEvaluator(string logFolder, string highlightsLogFile, IEnumerable<IEvaluator> evaluators)
        {
            foreach (var evaluator in evaluators)
            {
                evaluator.Evaluate();
            }
            
            bratkoKopecPositions = m_ResourceLoader.LoadBratkoKopecPositions();
            kaufmanPositions = m_ResourceLoader.LoadKaufmanTestPositions();
            
            //LogLine("Performance Evaluator");
            //LogLine("");
            //LogLine(string.Format("Logging started at {0}", DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")));
            //LogLine("");
        }

        public void RunFullPerformanceEvaluation(int maxDepth)
        {
            throw new NotImplementedException();
        }

        public void EvaluateTestPositions(int depth)
        {
            //LogLine("Evaluating Test position performance");
            //LogLine("");
             
            //LogLine("Bratko Kopec Positions");
            //LogLine("");
            //EvaluatePositions(bratkoKopecPositions, depth);
             
            //LogLine("Kaufman Positions");
            //LogLine(""); 
            //EvaluatePositions(kaufmanPositions, depth);

             //foreach (TestPosition testPos in testPositions)
             //{
             //    LogLine("--------------------------------------------");
             //    LogLine(testPos.Name);
             //    LogLine(testPos.FenPosition);

             //    for (int i = minDepth; i <= maxDepth; i++)
             //    {
             //        //LogLine("--------------------------------------------");
             //        //LogLine("");
             //        //LogLine(string.Format("Depth:{0}", i));
             //        //LogLine("");
             //        EvaluatePosition(testPos, i);

             //    }
             //}                
        }

        //private void EvaluatePositions(List<TestPosition> testPositions, int depth)
        //{
        //    var passed = 0;

        //    foreach (var testPos in testPositions)
        //    {
        //        LogLine("--------------------------------------------");
        //        LogLine(testPos.Name);
        //        LogLine(testPos.FenPosition);

        //        if (EvaluatePosition(testPos, depth))
        //            passed++;
        //    }

        //    LogLine(string.Format("Passed:{0}/{1}", passed, testPositions.Count));
            
        //}

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
            //    string move = UCIMoveTranslator.ToUCIMove(idInfo[i].Move);

            //    LogLine(string.Format("Depth {0}", i + 1));
            //    LogLine(string.Format("Nodes evaluated:{0}, Accumulated time:{1}, Best move:{2}, Score:{3}", idInfo[i].NodesVisited, idInfo[i].AccumulatedTime, move, idInfo[i].Score));
            //}

            //LogLine("");
            //LogLine(string.Format("Total time: {0}", speed.ToString()));
            //LogLine("");

            //string bestMove = UCIMoveTranslator.ToUCIMove(idInfo[idInfo.Count-1].Move);
            //var bestMove = PgnTranslator.ToPgnMove(board, idInfo[idInfo.Count - 1].Move.Position, idInfo[idInfo.Count - 1].Move.Moves, idInfo[idInfo.Count - 1].Move.Type);
            
            var expectedMove = testPos.bestMoveFEN;

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
