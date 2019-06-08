using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using ResourceLoading;

namespace EngineEvaluation
{
    internal class MoveTimeEvaluator
    {
        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        string logLocation = Environment.CurrentDirectory;
        string logFile;

        #region properties

        public string LogFile
        {
            get { return logFile; }
        }

        #endregion properties

        #region constructor

        public MoveTimeEvaluator()
        {
            CreateLogFile();

            LogLine("Move time evaluator");
            LogLine("");
            LogLine($"Logging started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");
        }        

        #endregion constructor

        internal void EvaluateMoveTime(int depth, int numOfMoves)
        {
            //var position = m_ResourceLoader.LoadPerfTPositions();

            //LogLine("--------------------------------------------");
            //LogLine(position.Name);
            //LogLine(position.FenPosition);
            //LogLine("");
            //LogLine($"Depth:{depth}");
            //LogLine("");

            //EvaluateMoveSpeed(position, depth, numOfMoves);
        }

        private void EvaluateMoveSpeed(PerfTPosition pos, int depth, int movesToMake)
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(pos.FenPosition));
            
            var scoreCalc = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            TranspositionTable.Restart();
            CountDebugger.ClearAll();

            var totalTime = new TimeSpan();
            var totalWhite = new TimeSpan();
            var totalBlack = new TimeSpan();

            ulong totalNodes = 0;
            ulong totalWhiteNodes = 0;
            ulong totalBlackNodes = 0;

            var whiteMoves = 0;
            var blackMoves = 0;

            for (var i = 0; i < movesToMake; i++)
            {
                var timer = new Stopwatch();
                timer.Start();

                var alphaBeta = new AlphaBetaSearchOld(board, scoreCalc);
                var currentMove = alphaBeta.StartSearch(depth);

                timer.Stop();

                var speed = new TimeSpan(timer.Elapsed.Ticks);

                var idInfo = alphaBeta.IdMoves;

                var moveNodes = CountNodes(idInfo);
                totalNodes += moveNodes;

                totalTime = totalTime.Add(speed);

                if (board.WhiteToMove)
                {
                    totalWhite = totalWhite.Add(speed);
                    totalWhiteNodes += moveNodes;
                    whiteMoves++;
                }
                else
                {
                    totalBlack = totalBlack.Add(speed);
                    totalBlackNodes += moveNodes;
                    blackMoves++;
                }

                LogLine($"{i}-Move nodes:{moveNodes:N0}, " +
                        $"Move time:{speed:ss'.'fff}, " +
                        $"Move made:{UCIMoveTranslator.ToUCIMove(currentMove)}, " +
                        $"Move score:{idInfo[idInfo.Count - 1].Score}");

                board.MakeMove(currentMove, true);
            }

            var movesMade = movesToMake;

            var averageTime = new TimeSpan(totalTime.Ticks / movesMade);
            var averageWhiteTime = new TimeSpan(totalWhite.Ticks / whiteMoves);
            var averageBlackTime = new TimeSpan(totalBlack.Ticks / blackMoves);
            
            LogLine("");
            LogLine($"Moves made:{movesMade}, " +
                    $"Total nodes:{totalNodes:N0}, " +
                    $"Average nodes:{(totalNodes / (ulong) movesMade):N0}, " +
                    $"Total time:{totalTime:mm'.'ss'.'fff}, " +
                    $"Average time:{averageTime:ss'.'fff}");
            LogLine("");
            LogLine($"White - Moves made:{whiteMoves}, " +
                    $"Total nodes:{totalWhiteNodes:N0}, " +
                    $"Average nodes:{(totalWhiteNodes / (ulong) whiteMoves):N0}, " +
                    $"Total time:{totalWhite:mm'.'ss'.'fff}, " +
                    $"Average time:{averageWhiteTime:ss'.'fff}");

            LogLine($"Black - Moves made:{blackMoves}, " +
                    $"Total nodes:{totalBlackNodes:N0}, " +
                    $"Average nodes:{(totalBlackNodes / (ulong) blackMoves):N0}, " +
                    $"Total time:{totalBlack:mm'.'ss'.'fff}, " +
                    $"Average time:{averageBlackTime:ss'.'fff}");

            LogLine(board.BoardToString());

            //Log moves to PGN

        }

        private static ulong CountNodes(IEnumerable<MoveValueInfo> idInfo)
        {
            ulong nodes = 0;

            foreach (var info in idInfo)
            {
                nodes += info.NodesVisited;
            }

            return nodes;
        } 

        #region logging

        private void CreateLogFile()
        {
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\MoveTimeLogging.txt";
            //File.Create(logFile);
        }

        private void LogLine(string text)
        {
            using (var stream = System.IO.File.AppendText(logFile))
            {
                stream.WriteLine(text);
            }
        }

        private void Log(string text)
        {
            using (var stream = System.IO.File.AppendText(logFile))
            {
                stream.Write(text);
            }
        }

        #endregion logging
    }
}
