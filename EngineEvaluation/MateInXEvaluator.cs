using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    internal sealed class MateInXEvaluator : IEvaluator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<Tuple<string, List<MateInXTestPosition>>> _testPositionSuites;

        private readonly string _fullLogFile;
        private readonly string _highlightsLogFile;

        private readonly ExcelHandler _excelHandler;

        public MateInXEvaluator(List<Tuple<string, List<MateInXTestPosition>>> testPositionSuites,
                                string highlightsLogFile,
                                string fullLogFile,
                                string testExcelLogFile)
        {
            if (testPositionSuites == null)
            {
                Log.Error("No Mate in X positions were passed to MateInXEvaluator");
                throw new ArgumentNullException(nameof(testPositionSuites));
            }

            _testPositionSuites = testPositionSuites;

            if (highlightsLogFile == null)
            {
                Log.Error("No highlightsLogFile was passed to MateInXEvaluator");
                throw new ArgumentNullException(nameof(highlightsLogFile));
            }

            _highlightsLogFile = highlightsLogFile;

            if (fullLogFile == null)
            {
                Log.Error("No fullLogFile was passed to MateInXEvaluator");
                throw new ArgumentNullException(nameof(fullLogFile));
            }

            _fullLogFile = fullLogFile;

            if (testExcelLogFile == null)
            {
                Log.Error("No testExcelLogFile was passed to MateInXEvaluator");
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
            LogLine("Mate in X evaluator");

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

                Log.Info($"Beginning evaluation of Test suite: {testSuiteName}");

                var totalTestSuiteTime = TimeSpan.Zero;

                float totalTestSuiteNodeCount = 0;

                var passedTestSuitePositions = 0;

                var currentMove = new PieceMove();

                foreach (var position in testPositionSuite.Item2)
                {
                    var chosenMoves = new string[position.MovesList.Length];

                    Log.Info($"Beginning evaluation of test: {position.Name} - FEN: {position.FenPosition}");

                    var board = new Board();
                    board.SetPosition(position.FenPosition);

                    var moveGeneration = new MoveGeneration();
                    var scoreCalculator = ScoreCalculatorFactory.Create();

                    TranspositionTable.Restart();

                    // Serialise the board since the search might leave it in a bad state
                    IFormatter formatter = new BinaryFormatter();
                    Board boardCopy;

                    // Make a copy of the board since the search might mess up it's state
                    using (var memStream = new MemoryStream())
                    {
                        formatter.Serialize(memStream, board);

                        memStream.Position = 0;
                        boardCopy = (Board)formatter.Deserialize(memStream);
                    }

                    var alphaBeta = new AlphaBetaSearch(moveGeneration, boardCopy, scoreCalculator);

                    var positionTimer = new Stopwatch();
                    positionTimer.Start();

                    var passed = false;

                    ulong totalNodes = 0;

                    for (var i=0; i<position.MovesList.Length; i++)
                    {
                        var move = position.MovesList[i];

                        if (maxThinkingSeconds > 0)
                        {
                            currentMove = alphaBeta.CalculateBestMove(evaluationDepth, maxThinkingSeconds);
                        }
                        else
                        {
                            currentMove = alphaBeta.CalculateBestMove(evaluationDepth);
                        }

                        var chosenMove = PgnTranslator.ToPgnMove(boardCopy, currentMove.Position, currentMove.Moves, currentMove.Type, currentMove.SpecialMove);

                        chosenMoves[i] = chosenMove;

                        totalNodes = alphaBeta.TotalSearchNodes;
                        totalTestSuiteNodeCount += totalNodes;

                        if (move == chosenMove)
                        {
                            passed = true;

                            var pieceMover = new PieceMover(boardCopy);
                            pieceMover.MakeMove(currentMove);
                            alphaBeta = new AlphaBetaSearch(moveGeneration, boardCopy, scoreCalculator);
                        }
                        else
                        {
                            passed = false;
                            positionTimer.Stop();
                            break;
                        }
                    }

                    if (passed)
                    {
                        passedTestSuitePositions++;
                    }

                    positionTimer.Stop();

                    totalTestSuiteTime = totalTestSuiteTime.Add(positionTimer.Elapsed);

                    LogTestPositionResults(
                        position.Name,
                        position.FenPosition,
                        chosenMoves,
                        position.MovesList,
                        passed,
                        positionTimer.Elapsed,
                        totalNodes);

                    _excelHandler.AddDataToSheet("Tests", new[]
                                                           {
                                                               testSuiteName,
                                                               position.Name,
                                                               passed.ToString(),
                                                               $"{totalNodes:n0}",
                                                               $"'{positionTimer.Elapsed.ToString()}"
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

            LogLine("--------------------------------------------------------------");

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

        private void LogTestPositionResults(string name, string position, string[] chosenMoves, string[] bestMoves, bool passed, TimeSpan elapsedTime, float totalNodes)
        {
            var result = passed ? "Passed" : "FAILED";

            LogLineAsDetailed($"{name}({position}) - " +
                              $"Winning moves:{string.Join(",", bestMoves)} - " +
                              $"Chosen moves:{string.Join(",", chosenMoves)} - " +
                              $"Total time:{elapsedTime} - " +
                              $"Total nodes visited:{totalNodes:n0} - " +
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
