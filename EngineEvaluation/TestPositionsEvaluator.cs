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

        private List<Tuple<string, List<TestPosition>>> m_TestPositions;

        private readonly string m_FullLogFile;
        private readonly string m_HighlightsLogFile;

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        public TestPositionsEvaluator(List<Tuple<string, List<TestPosition>>> testPositions, 
                                      string highlightsLogFile, 
                                      string fullLogFile)
        {
            if (testPositions == null)
            {
                Log.Error("No testPositions were passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(testPositions));
            }

            m_TestPositions = testPositions;

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
            
            foreach (var testPosition in m_TestPositions)
            {
                LogLine("--------------------------------------------------------------");
                LogLine($"Test set: {testPosition.Item1}");
                

                var totalTime = TimeSpan.Zero;

                var passedOverall = "PASSED";

                foreach (var position in testPosition.Item2)
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
                    
                    LogTestPositionResults(
                        chosenMove, 
                        position.BestMovePgn, 
                        timer.Elapsed,
                        totalNodes);

                    if (chosenMove != position.BestMovePgn)
                    {
                        passedOverall = "FAILED";
                    }
                }

                LogLine($"{passedOverall} - Total time: {totalTime}");
            }
        }

        private void LogTestPositionResults(string chosenMove, string bestMove, TimeSpan elapsedTime, decimal totalNodes)
        {
            var result = chosenMove == bestMove ? "Passed" : "FAILED";

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
