using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBoardTests;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.Debugging;
using ChessGame.MoveSearching;
using ChessGame.NotationHelpers;
using ChessGame.PossibleMoves;
using ChessGame.ResourceLoading;
using ChessGame.ScoreCalculation;

namespace EngineEvaluation
{
    class NodeCountEvaluator
    {
        string logLocation = Environment.CurrentDirectory;
        string logFile;
               
        const int REPEAT_COUNT_DEFAULT = 1;
        List<PerfTPosition> positions;
        
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
            LogLine(string.Format("Logging started at {0}", DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")));
        }        

        #endregion constructor


        internal void EvaluateNodes(int minDepth, int maxDepth)
        {          
            positions = ResourceLoader.LoadPerfTPositions();
              
            LogLine("");
            
            foreach (PerfTPosition perfTPosition in positions)
            {
                LogLine("--------------------------------------------");
                LogLine(perfTPosition.Name);
                LogLine(perfTPosition.FenPosition);

                if (minDepth < 1)
                    minDepth = 1;
                
                int searchDepth = maxDepth;

                if (perfTPosition.Results.Count < maxDepth)
                    searchDepth = perfTPosition.Results.Count;

                for (int i = minDepth; i <= searchDepth; i++)
                {
                    LogLine("--------------------------------------------");
                    LogLine("");
                    LogLine(string.Format("Depth:{0}", i));
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

            Board board = new Board();
            board.SetFenPosition(startingPosition);

            Stopwatch timer = new Stopwatch();
            timer.Start();
            
            PerfT perft = new PerfT();

            ulong result = perft.Perft(board, depth);

            if (result != expectedResult)
                LogLine(string.Format("PERFT VERIFICATION FAILED: Expected-{0}, Found-{1}", expectedResult, result));
            else if(expectedResult != (ulong)CountDebugger.Nodes)
                LogLine(string.Format("PERFT COUNTDEBUGGER VERIFICATION FAILED: Expected-{0}, Found-{1}", expectedResult, CountDebugger.Nodes));
            else
            {
                timer.Stop();

                TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

                LogLine(string.Format("Verification passed: Expected-{0}, Found-{1}, NodesCounted-{2}", expectedResult, result, CountDebugger.Nodes));
                LogLine(string.Format("Time: {0}", speed.ToString()));
                LogLine("");
            }
        }

        private void CountMiniMaxNodes(string startingPosition, int depth, ulong nodeCount)
        {
            CountDebugger.ClearAll();
            LogLine(string.Format("MiniMax Nodes"));
            LogLine("");

            Board board = new Board();
            board.SetFenPosition(startingPosition);

            ScoreCalculator scoreCalc = ResourceLoader.LoadScoreValues("ScoreValues.xml");

            PieceColour colour = FenTranslator.GetPlayerColour(startingPosition);


            Stopwatch timer = new Stopwatch();
            timer.Start();
           
            MiniMax miniMax = new MiniMax(board, scoreCalc);
            PieceMoves currentMove = miniMax.MoveCalculate(depth);

            timer.Stop();

            TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

            LogLine(string.Format("Minimax nodes evaluated: {0}/{1}", CountDebugger.Evaluations, nodeCount));
            LogLine(string.Format("Time: {0}", speed.ToString()));
            LogLine("");
            
        }

        private void CountNegaMaxNodes(string startingPosition, int depth, ulong nodeCount)
        {
            CountDebugger.ClearAll();
            LogLine(string.Format("NegaMax Nodes"));
            LogLine("");

            Board board = new Board();
            board.SetFenPosition(startingPosition);

            ScoreCalculator scoreCalc = ResourceLoader.LoadScoreValues("ScoreValues.xml");

            PieceColour colour = FenTranslator.GetPlayerColour(startingPosition);


            Stopwatch timer = new Stopwatch();
            timer.Start();

            NegaMax negaMax = new NegaMax(board, scoreCalc);
            PieceMoves currentMove = negaMax.MoveCalculate(depth);

            timer.Stop();

            TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

            LogLine(string.Format("NegaMax nodes evaluated: {0}/{1}", CountDebugger.Evaluations, nodeCount));
            LogLine(string.Format("Time: {0}", speed.ToString()));
            LogLine("");
        }

        private void CountAlphaBetaNodes(string startingPosition, int depth, ulong nodeCount)
        {
            CountDebugger.ClearAll(); 
            //LogLine(string.Format("AlphaBeta Nodes"));
            //LogLine("");

            Board board = new Board();
            board.SetFenPosition(startingPosition);

            ScoreCalculator scoreCalc = ResourceLoader.LoadScoreValues("ScoreValues.xml"); 

            PieceColour colour = FenTranslator.GetPlayerColour(startingPosition);

            TranspositionTable.Restart();

            Stopwatch timer = new Stopwatch();

            timer.Start();

            AlphaBetaSearch alphaBeta = new AlphaBetaSearch(board, scoreCalc);
            PieceMoves currentMove = alphaBeta.MoveCalculate(depth);

            timer.Stop();

            TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

            LogLine(string.Format("AlphaBeta nodes evaluated: {0}/{1}", CountDebugger.Evaluations, nodeCount));
            LogLine(string.Format("Time: {0}", speed.ToString()));
            LogLine(string.Format("Best move: {0}", UCIMoveTranslator.ToUCIMove(currentMove)));
            LogLine("");
        }

        private void CountIterativeAlphaBetaNodes(string startingPosition, int depth, List<ulong> nodeCounts)
        {
            CountDebugger.ClearAll();
            //LogLine(string.Format("AlphaBeta Nodes"));
            //LogLine("");

            Board board = new Board();
            board.SetFenPosition(startingPosition);

            ScoreCalculator scoreCalc = ResourceLoader.LoadScoreValues("ScoreValues.xml");

            PieceColour colour = FenTranslator.GetPlayerColour(startingPosition);

            Stopwatch timer = new Stopwatch();

            TranspositionTable.Restart();
            CountDebugger.ClearAll();

            timer.Start();

            AlphaBetaSearch alphaBeta = new AlphaBetaSearch(board, scoreCalc);
            PieceMoves currentMove = alphaBeta.StartSearch(depth);

            timer.Stop();

            TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);
                
            LogLine(string.Format("AlphaBeta with iterative deepening"));
            
            List<PVInfo> idInfo = alphaBeta.IdMoves;

            ulong totalNodes = 0;
            for (int i = 0; i < depth; i++)
			{                
                string move = UCIMoveTranslator.ToUCIMove(idInfo[i].Move);
                
			    LogLine(string.Format("Depth {0}", i+1));

                ulong nodes = idInfo[i].NodesVisited;
                totalNodes += nodes;
                LogLine(string.Format("Nodes evaluated:{0}/{1}, Accumulated time:{2}, Best move:{3}, Score:{4}", nodes.ToString("N0"), nodeCounts[i].ToString("N0"), idInfo[i].AccumulatedTime, move, idInfo[i].Score));
			}

            LogLine("");
            LogLine(string.Format("Total nodes: {0}", totalNodes.ToString("N0")));    
            LogLine(string.Format("Total time: {0}", speed.ToString()));            
            LogLine("");
        }

        #region logging

        private void CreateLogFile()
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\NodeCountLogging.txt";
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
