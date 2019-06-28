using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

using Excel = Microsoft.Office.Interop.Excel;

namespace EngineEvaluation
{
    public sealed class TestPositionsEvaluator : IEvaluator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Tuple<string, List<TestPosition>>> m_TestPositionSuites;

        private readonly string m_FullLogFile;
        private readonly string m_HighlightsLogFile;

        private readonly ExcelHandler m_ExcelHandler;


        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();
        
        public TestPositionsEvaluator(List<Tuple<string, List<TestPosition>>> testPositionSuites, 
                                      string highlightsLogFile, 
                                      string fullLogFile,
                                      string testExcelLogFile)
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
                throw new ArgumentNullException(nameof(fullLogFile));
            }

            m_FullLogFile = fullLogFile;

            if (testExcelLogFile == null)
            {
                Log.Error("No testExcelLogFile was passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(testExcelLogFile));
            }

            m_ExcelHandler = new ExcelHandler(testExcelLogFile);

            m_ExcelHandler.CreateHeaders("Test suites", new [] { "Test suite", "Result", "Test passed", "Number of tests", "Nodes visited", "Total time (hh:mm:ss.ms)"});
            m_ExcelHandler.CreateHeaders("Tests", new[] { "Test suite", "Test name", "Passed", "Nodes visited", "Total time (hh:mm:ss.ms)" });

            LogLine("====================================================================");
            LogLine("Test positions evaluator");
        }

        public void Evaluate(int evaluationDepth)
        {
            LogLineAsDetailed($"Evaluation started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");
            LogLineAsDetailed($"Logging Test positions with a search depth of {evaluationDepth}");

            var overallTestPositions = 0;
            var overallPassedTestPositions = 0;

            var overallTestSuiteTime = TimeSpan.Zero;
            float overallTestSuiteNodes = 0;

            foreach (var testPositionSuite in m_TestPositionSuites)
            {
                var testSuiteName = testPositionSuite.Item1;

                LogLine("--------------------------------------------------------------");
                LogLine($"Test set: {testSuiteName}");

                Log.Info($"Beginning test evaluation of Test suite: {testSuiteName}");
                
                var totalTestSuiteTime = TimeSpan.Zero;
                float totalTestSuiteNodeCount = 0;

                var passedTestSuitePositions = 0;

                foreach (var position in testPositionSuite.Item2)
                {
                    Log.Info($"Beginning test evaluation of test: {position.Name} - FEN: {position.FenPosition}");
                    
                    var board = new Board();
                    board.SetPosition(FenTranslator.ToBoardState(position.FenPosition));

                    var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

                    TranspositionTable.Restart();

                    var alphaBeta = new AlphaBetaSearch(board, scoreCalculator);

                    var timer = new Stopwatch();
                    timer.Start();

                    var currentMove = alphaBeta.CalculateBestMoveTimed(evaluationDepth, 2);

                    timer.Stop();

                    totalTestSuiteTime = totalTestSuiteTime.Add(timer.Elapsed);

                    var totalNodes = alphaBeta.GetMoveValueInfo().Sum(n => (float)n.NodesVisited);
                    totalTestSuiteNodeCount += totalNodes;

                    var chosenMove = PgnTranslator.ToPgnMove(board, currentMove.Position, currentMove.Moves, currentMove.Type);

                    var passed = false;
                    
                    if (position.BestMovePgn == chosenMove)
                    {
                        passedTestSuitePositions++;
                        passed = true;
                    }
                    
                    LogTestPositionResults(
                        position.Name,
                        chosenMove, 
                        position.BestMovePgn,
                        passed,
                        timer.Elapsed,
                        totalNodes);

                    m_ExcelHandler.AddDataToSheet("Tests", new[]
                                                           {
                                                               testSuiteName,
                                                               position.Name,
                                                               passed.ToString(),
                                                               $"{totalNodes:n0}",
                                                               $"'{timer.Elapsed.ToString()}"
                                                           });
                }

                var totalSuiteTestPositions = testPositionSuite.Item2.Count;

                var passedSuite = passedTestSuitePositions == totalSuiteTestPositions ? "Passed" : "FAILED";
                
                LogLine($"{passedSuite} - {passedTestSuitePositions}/{totalSuiteTestPositions} - " +
                        $"Total time: {totalTestSuiteTime} - " +
                        $"Total node visited: {totalTestSuiteNodeCount:n0}");

                m_ExcelHandler.AddDataToSheet("Test suites", new[]
                                                             {
                                                                 testSuiteName,
                                                                 passedSuite,
                                                                 passedTestSuitePositions.ToString(CultureInfo.InvariantCulture),
                                                                 totalSuiteTestPositions.ToString(CultureInfo.InvariantCulture),
                                                                 $"{totalTestSuiteNodeCount:n0}",
                                                                 $"'{totalTestSuiteTime.ToString()}"
                                                             });

                overallPassedTestPositions += passedTestSuitePositions;
                overallTestPositions += totalSuiteTestPositions;

                overallTestSuiteTime = overallTestSuiteTime.Add(totalTestSuiteTime);

                overallTestSuiteNodes += totalTestSuiteNodeCount;
            }

            LogLine($"Overall passed: {overallPassedTestPositions}/{ overallTestPositions}");
            LogLine($"Overall time: {overallTestSuiteTime}");
            LogLine($"Overall node count: {overallTestSuiteNodes:n0}");

            m_ExcelHandler.AddDataToSheet("Test suites", new[]
                                                         {
                                                             string.Empty,
                                                             string.Empty,
                                                             overallPassedTestPositions.ToString(CultureInfo.InvariantCulture),
                                                             overallTestPositions.ToString(CultureInfo.InvariantCulture),
                                                             $"{overallTestSuiteNodes:n0}",
                                                             $"'{overallTestSuiteTime.ToString()}"
                                                         });

            m_ExcelHandler.AddDataToSheet("Tests", new[]
                                                   {
                                                       string.Empty,
                                                       string.Empty,
                                                       overallPassedTestPositions.ToString(CultureInfo.InvariantCulture),
                                                       overallTestPositions.ToString(CultureInfo.InvariantCulture),
                                                       $"{overallTestSuiteNodes:n0}",
                                                       $"'{overallTestSuiteTime.ToString()}"
                                                   });
        }

        private void LogTestPositionResults(
            string positionName, string chosenMove, string bestMove, bool passed, TimeSpan elapsedTime, float totalNodes)
        {
            var result = passed ? "Passed" : "FAILED";

            LogLineAsDetailed($"Name:{ positionName } - " + 
                              $"Best move:{ bestMove } - " +
                              $"Selected move {chosenMove} - " +
                              $"Total time: {elapsedTime} - " +
                              $"Total nodes visited {totalNodes:n0} - " +
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
