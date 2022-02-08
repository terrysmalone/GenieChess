using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    public sealed class TestPositionsEvaluator : IEvaluator
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Tuple<string, List<TestPosition>>> _testPositionSuites;

        private readonly string _fullLogFile;
        private readonly string _highlightsLogFile;

        private readonly ExcelHandler _excelHandler;

        private readonly IResourceLoader _resourceLoader = new ResourceLoader();
        
        public TestPositionsEvaluator(List<Tuple<string, List<TestPosition>>> testPositionSuites, 
                                      string highlightsLogFile, 
                                      string fullLogFile,
                                      string testExcelLogFile)
        {
            if (testPositionSuites == null)
            {
                _log.Error("No testPositionSuites were passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(testPositionSuites));
            }

            _testPositionSuites = testPositionSuites;

            if (highlightsLogFile == null)
            {
                _log.Error("No highlightsLogFile was passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(highlightsLogFile));
            }

            _highlightsLogFile = highlightsLogFile;

            if (fullLogFile == null)
            {
                _log.Error("No fullLogFile was passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(fullLogFile));
            }

            this._fullLogFile = fullLogFile;

            if (testExcelLogFile == null)
            {
                _log.Error("No testExcelLogFile was passed to TestPositionsEvaluator");
                throw new ArgumentNullException(nameof(testExcelLogFile));
            }

            _excelHandler = new ExcelHandler(testExcelLogFile);

            _excelHandler.CreateHeaders("Test suites", new [] { "Test suite", "Result", "Test passed", "Number of tests", "Nodes visited", "Total time (hh:mm:ss.ms)"});
            _excelHandler.CreateHeaders("Tests", new[] { "Test suite", "Test name", "Passed", "Nodes visited", "Total time (hh:mm:ss.ms)" });
        }

        public void Evaluate(int evaluationDepth)
        {
            Evaluate(evaluationDepth, 0);
        }

        public void Evaluate(int evaluationDepth, int maxThinkingSeconds)
        {
            LogLine("====================================================================");
            LogLine("Test positions evaluator");

            LogLineAsDetailed($"Evaluation started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");

            if (maxThinkingSeconds > 0)
            {
                LogLineAsDetailed($"Logging Test positions with a max search depth of {evaluationDepth} " +
                                  $"and a max thinking time of {maxThinkingSeconds} seconds");
            }
            else
            {
                LogLineAsDetailed($"Logging Test positions with a max search depth of {evaluationDepth}");
            }

            var overallTestPositions = 0;
            var overallPassedTestPositions = 0;

            var overallTestSuiteTime = TimeSpan.Zero;
            float overallTestSuiteNodes = 0;

            foreach (var testPositionSuite in _testPositionSuites)
            {
                var testSuiteName = testPositionSuite.Item1;

                LogLine("--------------------------------------------------------------");
                LogLine($"Test set: {testSuiteName}");

                _log.Info($"Beginning test evaluation of Test suite: {testSuiteName}");
                
                var totalTestSuiteTime = TimeSpan.Zero;
                float totalTestSuiteNodeCount = 0;

                var passedTestSuitePositions = 0;

                foreach (var position in testPositionSuite.Item2)
                {
                    _log.Info($"Beginning test evaluation of test: {position.Name} - FEN: {position.FenPosition}");
                    
                    var board = new Board();
                    board.SetPosition(position.FenPosition);

                    var scoreCalculator = ScoreCalculatorFactory.Create();

                    TranspositionTable.Restart();

                    // Serialise the board since the search might leave it in a bad state
                    IFormatter formatter = new BinaryFormatter();
                    Board boardCopy;
                    
                    // Make a copy of the board since the search might mess up it's state
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        formatter.Serialize(memStream, board);

                        memStream.Position = 0;
                        boardCopy = (Board)formatter.Deserialize(memStream);
                    }

                    var alphaBeta = new AlphaBetaSearch(boardCopy, scoreCalculator);

                    var timer = new Stopwatch();
                    timer.Start();

                    var currentMove = new PieceMove();

                    if (maxThinkingSeconds > 0)
                    {
                        currentMove = alphaBeta.CalculateBestMove(evaluationDepth, maxThinkingSeconds);
                    }
                    else
                    {
                        currentMove = alphaBeta.CalculateBestMove(evaluationDepth);
                    }

                    timer.Stop();

                    totalTestSuiteTime = totalTestSuiteTime.Add(timer.Elapsed);

                    var totalNodes = alphaBeta.TotalSearchNodes;
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

                    _excelHandler.AddDataToSheet("Tests", new[]
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

                _excelHandler.AddDataToSheet("Test suites", new[]
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

            _excelHandler.AddDataToSheet("Test suites", new[]
                                                         {
                                                             string.Empty,
                                                             string.Empty,
                                                             overallPassedTestPositions.ToString(CultureInfo.InvariantCulture),
                                                             overallTestPositions.ToString(CultureInfo.InvariantCulture),
                                                             $"{overallTestSuiteNodes:n0}",
                                                             $"'{overallTestSuiteTime.ToString()}"
                                                         });

            _excelHandler.AddDataToSheet("Tests", new[]
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
            using (var stream = File.AppendText(_highlightsLogFile))
            {
                stream.WriteLine(text);
            }
        }

        private void LogLineAsDetailed(string text)
        {
            using (var stream = File.AppendText(_fullLogFile))
            {
                stream.WriteLine(text);
            }
        }
    }
}
