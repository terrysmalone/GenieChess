using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using ChessEngineTests;
using ResourceLoading;

namespace EngineEvaluation
{
    internal sealed class NodeCountEvaluator
    {
        private string logLocation = Environment.CurrentDirectory;
        private string logFile;

        private const int REPEAT_COUNT_DEFAULT = 1;
        private List<PerfTPosition> positions;

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        #region properties

        public string LogFile
        {
            get { return logFile; }
        }

        #endregion properties

        #region constructor

        public NodeCountEvaluator()
        {
            CreateLogFile();

            LogLine("Node count evaluator");
            LogLine("");
            LogLine($"Logging started at {DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")}");
        }        

        #endregion constructor


        internal void EvaluateNodes(int minDepth, int maxDepth)
        {          
            positions = m_ResourceLoader.LoadPerfTPositions();
              
            LogLine("");
            
            foreach (var perfTPosition in positions)
            {
                LogLine("--------------------------------------------");
                LogLine(perfTPosition.Name);
                LogLine(perfTPosition.FenPosition);

                if (minDepth < 1)
                    minDepth = 1;
                
                var searchDepth = maxDepth;

                if (perfTPosition.Results.Count < maxDepth)
                    searchDepth = perfTPosition.Results.Count;

                for (var i = minDepth; i <= searchDepth; i++)
                {
                    LogLine("--------------------------------------------");
                    LogLine("");
                    LogLine($"Depth:{i}");
                    LogLine("");

                    //CountPerftNodes(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1]);
                    //CountMiniMaxNodes(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1]);
                    //CountNegaMaxNodes(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1]);

                    //CountAlphaBetaNodes(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1]);                                     
                }

                LogLine("--------------------------------------------");
                CountIterativeAlphaBetaNodes(perfTPosition.FenPosition, searchDepth, perfTPosition.Results);   
            }
        }
        
        private void CountPerftNodes(string startingPosition, int depth, ulong expectedResult)
        {
            CountDebugger.ClearAll();
            LogLine(string.Format("PerfT verification"));
            LogLine("");

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(startingPosition));

            var timer = new Stopwatch();
            timer.Start();
            
            var perft = new PerfT();

            var result = perft.Perft(board, depth);

            if (result != expectedResult)
            {
                LogLine($"PERFT VERIFICATION FAILED: Expected-{expectedResult}, Found-{result}");
            }
            else if (expectedResult != (ulong) CountDebugger.Nodes)
            {
                LogLine(
                    $"PERFT COUNTDEBUGGER VERIFICATION FAILED: Expected-{expectedResult}, Found-{CountDebugger.Nodes}");
            }
            else
            {
                timer.Stop();

                var speed = new TimeSpan(timer.Elapsed.Ticks);

                LogLine($"Verification passed: Expected-{expectedResult}, Found-{result}, NodesCounted-{CountDebugger.Nodes}");
                LogLine($"Time: {speed.ToString()}");
                LogLine("");
            }
        }

        private void CountMiniMaxNodes(string startingPosition, int depth, ulong nodeCount)
        {
            CountDebugger.ClearAll();
            LogLine(string.Format("MiniMax Nodes"));
            LogLine("");

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(startingPosition));

            var scoreCalc = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var colour = FenTranslator.GetPlayerColour(startingPosition);


            var timer = new Stopwatch();
            timer.Start();
           
            var miniMax = new MiniMax(board, scoreCalc);
            var currentMove = miniMax.MoveCalculate(depth);

            timer.Stop();

            var speed = new TimeSpan(timer.Elapsed.Ticks);

            LogLine($"Minimax nodes evaluated: {CountDebugger.Evaluations}/{nodeCount}");
            LogLine($"Time: {speed.ToString()}");
            LogLine("");
            
        }

        private void CountNegaMaxNodes(string startingPosition, int depth, ulong nodeCount)
        {
            CountDebugger.ClearAll();
            LogLine(string.Format("NegaMax Nodes"));
            LogLine("");

            var board = new Board();
   
            board.SetPosition(FenTranslator.ToBoardState(startingPosition));

            var scoreCalc = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var colour = FenTranslator.GetPlayerColour(startingPosition);


            var timer = new Stopwatch();
            timer.Start();

            var negaMax = new NegaMax(board, scoreCalc);
            var currentMove = negaMax.MoveCalculate(depth);

            timer.Stop();

            var speed = new TimeSpan(timer.Elapsed.Ticks);

            LogLine($"NegaMax nodes evaluated: {CountDebugger.Evaluations}/{nodeCount}");
            LogLine($"Time: {speed.ToString()}");
            LogLine("");
        }

        private void CountAlphaBetaNodes(string startingPosition, int depth, ulong nodeCount)
        {
            CountDebugger.ClearAll(); 
            //LogLine(string.Format("AlphaBeta Nodes"));
            //LogLine("");

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(startingPosition));

            var scoreCalc = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var colour = FenTranslator.GetPlayerColour(startingPosition);

            TranspositionTable.Restart();

            var timer = new Stopwatch();

            timer.Start();

            var alphaBeta = new AlphaBetaSearchOld(board, scoreCalc);
            var currentMove = alphaBeta.MoveCalculate(depth);

            timer.Stop();

            var speed = new TimeSpan(timer.Elapsed.Ticks);

            LogLine($"AlphaBeta nodes evaluated: {CountDebugger.Evaluations}/{nodeCount}");
            LogLine($"Time: {speed.ToString()}");
            LogLine($"Best move: {UCIMoveTranslator.ToUCIMove(currentMove)}");
            LogLine("");
        }

        private void CountIterativeAlphaBetaNodes(string startingPosition, int depth, List<ulong> nodeCounts)
        {
            CountDebugger.ClearAll();
            //LogLine(string.Format("AlphaBeta Nodes"));
            //LogLine("");

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(startingPosition));

            var scoreCalc = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var colour = FenTranslator.GetPlayerColour(startingPosition);

            var timer = new Stopwatch();

            TranspositionTable.Restart();
            CountDebugger.ClearAll();

            timer.Start();

            var alphaBeta = new AlphaBetaSearchOld(board, scoreCalc);
            var currentMove = alphaBeta.StartSearch(depth);

            timer.Stop();

            var speed = new TimeSpan(timer.Elapsed.Ticks);
                
            LogLine(string.Format("AlphaBeta with iterative deepening"));
            
            var idInfo = alphaBeta.IdMoves;

            ulong totalNodes = 0;
            for (var i = 0; i < depth; i++)
			{                
                var move = UCIMoveTranslator.ToUCIMove(idInfo[i].Move);
                
			    LogLine($"Depth {i + 1}");

                var nodes = idInfo[i].NodesVisited;
                totalNodes += nodes;
                LogLine(
                    $"Nodes evaluated:{nodes:N0}/{nodeCounts[i]:N0}, Accumulated time:{idInfo[i].AccumulatedTime}, Best move:{move}, Score:{idInfo[i].Score}");
			}

            LogLine("");
            LogLine($"Total nodes: {totalNodes:N0}");    
            LogLine($"Total time: {speed.ToString()}");            
            LogLine("");
        }

        #region logging

        private void CreateLogFile()
        {
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\NodeCountLogging.txt";
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
