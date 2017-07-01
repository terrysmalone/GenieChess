using ChessGame.BoardRepresentation;
using ChessGame.Debugging;
using ChessGame.MoveSearching;
using ChessGame.NotationHelpers;
using ChessGame.PossibleMoves;
using ChessGame.ResourceLoading;
using ChessGame.ScoreCalculation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EngineEvaluation
{
    internal class MoveTimeEvaluator
    {
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
            LogLine(string.Format("Logging started at {0}", DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")));
        }        

        #endregion constructor

        internal void EvaluateMoveTime(int depth, int numOfMoves)
        {
            PerfTPosition position = ResourceLoader.LoadPerfTPosition("PerfTInitial");

            LogLine("--------------------------------------------");
            LogLine(position.Name);
            LogLine(position.FenPosition);
            LogLine("");
            LogLine(string.Format("Depth:{0}", depth));
            LogLine("");

            EvaluateMoveSpeed(position, depth, numOfMoves);
        }

        private void EvaluateMoveSpeed(PerfTPosition pos, int depth, int movesToMake)
        {
            Board board = new Board();
            board.SetFENPosition(pos.FenPosition);

            ScoreCalculator scoreCalc = ResourceLoader.LoadScoreValues("ScoreValues.xml");

            TranspositionTable.Restart();
            CountDebugger.ClearAll();

            TimeSpan totalTime = new TimeSpan();
            TimeSpan totalWhite = new TimeSpan();
            TimeSpan totalBlack = new TimeSpan();

            ulong totalNodes = 0;
            ulong totalWhiteNodes = 0;
            ulong totalBlackNodes = 0;

            int whiteMoves = 0;
            int blackMoves = 0;

            for (int i = 0; i < movesToMake; i++)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                AlphaBetaSearch alphaBeta = new AlphaBetaSearch(board, scoreCalc);
                PieceMoves currentMove = alphaBeta.StartSearch(depth);

                timer.Stop();

                TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

                List<PVInfo> idInfo = alphaBeta.IdMoves;

                ulong moveNodes = CountNodes(idInfo);
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

                LogLine(string.Format("{0}-Move nodes:{1}, Move time:{2}, Move made:{3}, Move score:{4}", i, moveNodes.ToString("N0"), speed.ToString("ss'.'fff"), UCIMoveTranslator.ToUCIMove(currentMove), idInfo[idInfo.Count - 1].Score));

                board.MakeMove(currentMove, true);
            }

            int movesMade = movesToMake;

            TimeSpan averageTime = new TimeSpan(totalTime.Ticks / movesMade);
            TimeSpan averageWhiteTime = new TimeSpan(totalWhite.Ticks / whiteMoves);
            TimeSpan averageBlackTime = new TimeSpan(totalBlack.Ticks / blackMoves);
            
            LogLine("");
            LogLine(string.Format("Moves made:{0}, Total nodes:{1}, Average nodes:{2}, Total time:{3}, Average time:{4}", movesMade, totalNodes.ToString("N0"), (totalNodes / (ulong)movesMade).ToString("N0"), totalTime.ToString("mm'.'ss'.'fff"), averageTime.ToString("ss'.'fff")));
            LogLine("");
            LogLine(string.Format("White - Moves made:{0}, Total nodes:{1}, Average nodes:{2}, Total time:{3}, Average time:{4}", whiteMoves, totalWhiteNodes.ToString("N0"), (totalWhiteNodes / (ulong)whiteMoves).ToString("N0"), totalWhite.ToString("mm'.'ss'.'fff"), averageWhiteTime.ToString("ss'.'fff")));
            LogLine(string.Format("Black - Moves made:{0}, Total nodes:{1}, Average nodes:{2}, Total time:{3}, Average time:{4}", blackMoves, totalBlackNodes.ToString("N0"), (totalBlackNodes / (ulong)blackMoves).ToString("N0"), totalBlack.ToString("mm'.'ss'.'fff"), averageBlackTime.ToString("ss'.'fff")));

            LogLine(board.BoardToString());

            //Log moves to PGN

        }

        private ulong CountNodes(List<PVInfo> idInfo)
        {
            ulong nodes = 0;

            foreach (PVInfo info in idInfo)
            {
                nodes += info.NodesVisited;
            }

            return nodes;
        } 

        #region logging

        private void CreateLogFile()
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\MoveTimeLogging.txt";
            //File.Create(logFile);
        }

        private void LogLine(string text)
        {
            using (System.IO.StreamWriter stream = System.IO.File.AppendText(logFile))
            {
                stream.WriteLine(text);
            }
        }

        private void Log(string text)
        {
            using (System.IO.StreamWriter stream = System.IO.File.AppendText(logFile))
            {
                stream.Write(text);
            }
        }

        #endregion logging
    }
}
