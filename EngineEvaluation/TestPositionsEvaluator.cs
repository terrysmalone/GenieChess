using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    public sealed class TestPositionsEvaluator : IEvaluator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Tuple<string, List<TestPosition>>> m_TestPositionSuites;

        private readonly string m_FullLogFile;
        private readonly string m_HighlightsLogFile;

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        public TestPositionsEvaluator(List<Tuple<string, List<TestPosition>>> testPositionSuites, 
                                      string highlightsLogFile, 
                                      string fullLogFile)
        {
            if (testPositionSuites == null)
            {
                Log.Error("No testPositionSuites were passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(testPositionSuites));
            }

            m_TestPositionSuites = testPositionSuites;

            if (highlightsLogFile == null)
            {
                Log.Error("No highlightsLogFile was passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(highlightsLogFile));
            }

            m_HighlightsLogFile = highlightsLogFile;

            if (fullLogFile == null)
            {
                Log.Error("No fullLogFile was passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(highlightsLogFile));
            }

            m_FullLogFile = fullLogFile;

            LogLine("====================================================================");
            LogLine("Test positions evaluator");
        }

        public void Evaluate(int evaluationDepth)
        {
            LogLineAsDetailed($"Evaluation started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");
            LogLineAsDetailed($"Logging Test positions with a search depth of {evaluationDepth}");
            
            foreach (var testPositionSuite in m_TestPositionSuites)
            {
                LogLine("--------------------------------------------------------------");
                LogLine($"Test set: {testPositionSuite.Item1}");
                

                var totalTime = TimeSpan.Zero;

                var passedTestPositions = 0;

                foreach (var position in testPositionSuite.Item2)
                {
                    var board = new Board();
                    board.SetPosition(FenTranslator.ToBoardState(position.FenPosition));

                    var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

                    TranspositionTable.Restart();

                    var alphaBeta = new AlphaBetaSearch(board, scoreCalculator);

                    var timer = new Stopwatch();
                    timer.Start();

                    var currentMove = alphaBeta.CalculateBestMove(evaluationDepth);

                    timer.Stop();

                    totalTime = totalTime.Add(timer.Elapsed);

                    var totalNodes = alphaBeta.GetMoveValueInfo().Sum(n => (decimal)n.NodesVisited);

                    var chosenMove = PgnTranslator.ToPgnMove(board, currentMove.Position, currentMove.Moves, currentMove.Type);

                    var passed = false;
                    
                    if (position.BestMovePgn == chosenMove)
                    {
                        passedTestPositions++;
                        passed = true;
                    }
                    
                    LogTestPositionResults(
                        chosenMove, 
                        position.BestMovePgn,
                        passed,
                        timer.Elapsed,
                        totalNodes);
                }

                var totalTestPositions = testPositionSuite.Item2.Count;

                var passedSuite = passedTestPositions == totalTestPositions ? "Passed" : "FAILED";
                
                LogLine($"{passedSuite} - {passedTestPositions}/{totalTestPositions} - Total time: {totalTime}");
            }
        }

        private void LogTestPositionResults(string chosenMove, string bestMove, bool passed, TimeSpan elapsedTime, decimal totalNodes)
        {
            var result = passed ? "Passed" : "FAILED";

            LogLineAsDetailed($"Best move:{ bestMove } - " +
                              $"Selected move {chosenMove} - " +
                              $"Total time: {elapsedTime} - " +
                              $"Total nodes visited { totalNodes } - " +
                              $"{result}");
        }

        private void LogLine(string text)
        {
            LogLineAsDetailed(text);
            LogLineAsHighlight(text);
        }
        
        private void LogLineAsHighlight(string text)
        {
            using (var stream = File.AppendText(m_HighlightsLogFile))
            {
                stream.WriteLine(text);
            }
        }

        private void LogLineAsDetailed(string text)
        {
            using (var stream = File.AppendText(m_FullLogFile))
            {
                stream.WriteLine(text);
            }
        }
    }
}
