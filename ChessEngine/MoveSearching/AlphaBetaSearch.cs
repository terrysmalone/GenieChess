using System.Diagnostics;
using System.Globalization;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.Debugging;
using ChessEngine.Exceptions;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using Logging;

namespace ChessEngine.MoveSearching;

// The basic algorithm performs a negamax alpha-beta pruning
// using iterative deepening
public sealed class AlphaBetaSearch
{
    private readonly ILog _log;

    private readonly MoveGeneration _moveGeneration;

    private readonly Board _boardPosition;
    private readonly IScoreCalculator _scoreCalculator;

    private readonly PieceMover _pieceMover;

    private List<MoveValueInfo> _initialMoves;
    private List<Tuple<int, PieceMove>> _initialMovesIterativeDeepeningShuffleOrder;

    private int _killerMovesToStore = 2;
    private PieceMove[,] _killerMoves;

    private int _evaluationDepth;

    private PieceMove? _bestMoveSoFar;

    private const int _nullMoveR = 1;

    private const int _maxCheckExtension = 10;

    public AlphaBetaSearch(MoveGeneration moveGeneration, Board boardPosition, IScoreCalculator scoreCalculator, ILog log)
    {
        _moveGeneration = moveGeneration ?? throw new ArgumentNullException(nameof(moveGeneration));
        _boardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));
        _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        _log = log;

        _pieceMover = new PieceMover(_boardPosition);
    }

    private PieceMove _completedSearchBestMove;
    private PieceMove _latestDepthBestMove;
    private PieceMove _inDepthBestMove;
    public ulong TotalSearchNodes;

    // See: https://stackoverflow.com/questions/2265412/set-timeout-to-an-operation
    // Task might be better
    public PieceMove CalculateBestMove(int maxDepth, int maxThinkingSeconds)
    {
        _completedSearchBestMove = new PieceMove();
        _latestDepthBestMove = new PieceMove();
        _inDepthBestMove = new PieceMove();

        TotalSearchNodes = 0;

        var threadTimeout = new TimeSpan(0, 0, maxThinkingSeconds);

        var moveTimer = new Stopwatch();
        moveTimer.Start();

        var moveThread = new Thread(() => CalculateBestMove(maxDepth, moveTimer));

        _log.Info($"Thread Starting: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
        moveThread.Start();
        _log.Info($"Thread Started: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");

        _log.Info($"Thread joining: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
        var finished = moveThread.Join(threadTimeout);
        _log.Info($"Thread joined: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");


        if (!finished)
        {
            _log.Info($"Thread aborting: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
            moveThread.Abort();
            _log.Info($"Thread aborted: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");

            _log.Info("Maximum time limit reached");

            TotalSearchNodes += CountDebugger.Evaluations;
        }

        moveTimer.Stop();

        _log.Info($"Move Time: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");

        // Temporary
        _log.Info(_completedSearchBestMove.Type == PieceType.None
                       ? "Completed search best move: -"
                       : $"Completed search best move: {UciMoveTranslator.ToUciMove(_completedSearchBestMove)}");

        _log.Info(_latestDepthBestMove.Type == PieceType.None
                       ? "Highest depth best move: -"
                        :$"Highest depth best move: {UciMoveTranslator.ToUciMove(_latestDepthBestMove)}");

        _log.Info(_inDepthBestMove.Type == PieceType.None
                       ? "In latest depth best move: -"
                       : $"In latest depth best move: {UciMoveTranslator.ToUciMove(_inDepthBestMove)}");

        // Temporary end

        if (_completedSearchBestMove.Type != PieceType.None)
        {
            _log.Info($"Found move: {UciMoveTranslator.ToUciMove(_completedSearchBestMove)}");

            return _completedSearchBestMove;
        }

        if (_latestDepthBestMove.Type != PieceType.None)
        {
            _log.Info($"Found move: {UciMoveTranslator.ToUciMove(_latestDepthBestMove)}");

            return _latestDepthBestMove;
        }

        if (_inDepthBestMove.Type != PieceType.None)
        {
            _log.Info($"Found move: {UciMoveTranslator.ToUciMove(_inDepthBestMove)}");

            return _inDepthBestMove;
        }

        throw new ChessBoardException("No move was found in given time");
    }

    public PieceMove CalculateBestMove(int maxDepth, Stopwatch moveTimer = null)
    {
        var toMove = _boardPosition.WhiteToMove ? "white" : "black";

        _log.Info($"Calculating move for {toMove}");
        _log.Info(FenTranslator.ToFenString(_boardPosition.GetCurrentBoardState()));

        _killerMoves = new PieceMove[maxDepth, _killerMovesToStore]; // Try to a depth of maxDepth with 5 saved each round

        _initialMoves = new List<MoveValueInfo>();
        _initialMovesIterativeDeepeningShuffleOrder = new List<Tuple<int, PieceMove>>();

        TranspositionTable.ResetAncients();

        var bestMove = new PieceMove();

        CountDebugger.ClearTranspositionValues();

        // Calculate scores for each move, starting at a
        // depth of one and working to max
        for (var depth = 1; depth <= maxDepth; depth++)
        {
#if UCI
            Console.WriteLine($"info depth {depth}");
#endif
            _log.Info("=========================================================================================");
            _log.Info($"Depth {depth}");

            CountDebugger.ClearNodesAndEvaluations();

            var timer = new Stopwatch();

            timer.Start();

            _evaluationDepth = depth;

            // Calculate the best move at the current depth
            bestMove = CalculateBestMove(depth, out var bestScore);

            _latestDepthBestMove = bestMove;

            // Reset this after each depth to make sure any move is from the current depth
            _inDepthBestMove = new PieceMove();

            timer.Stop();

            var speed = new TimeSpan(timer.Elapsed.Ticks);

            var moveValueInfo = new MoveValueInfo
            {
                Move = bestMove,
                Score = bestScore,
                DepthTime = speed,
                AccumulatedTime = _initialMoves.Count > 0
                    ? _initialMoves[_initialMoves.Count - 1].AccumulatedTime.Add(speed)
                    : speed,
                NodesVisited = CountDebugger.Evaluations
            };

            if (moveTimer != null)
            {
                _log.Info($"Depth {depth} found: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
            }


            _initialMoves.Add(moveValueInfo);

#if FullNodeCountDebug
            LogTranspositionCounts();
#endif
            _log.Info($"Move info: {UciMoveTranslator.ToUciMove(moveValueInfo.Move)} - " +
                       $"score: {GetScoreString(moveValueInfo.Score, _boardPosition.WhiteToMove)} - " +
                       $"nodes: {moveValueInfo.NodesVisited} - " +
                       $"time at depth: {moveValueInfo.DepthTime:mm\':\'ss\':\'ffff} - " +
                       $"Accumulated move time: {moveValueInfo.AccumulatedTime:mm\':\'ss\':\'ffff}");

#if UCI
            var foundMove = UciMoveTranslator.ToUciMove(bestMove);
            Console.WriteLine($"Best move at depth {depth}: {foundMove}");

            Console.WriteLine($"info currmove {bestMove} " +
                              $"depth {depth} " +
                              $"nodes {moveValueInfo.NodesVisited} ");

            Console.WriteLine($"info score cp 0 {bestMove} " +
                              $"depth {depth} " +
                              $"nodes {moveValueInfo.NodesVisited} " +
                              $"time {moveValueInfo.DepthTime:mm\':\'ss\':\'ffff}");

            Console.WriteLine($"info score cp {moveValueInfo.Score} " +
                              $"depth {depth} " +
                              $"nodes {moveValueInfo.NodesVisited} pv {bestMove} ");
#endif

            TotalSearchNodes += CountDebugger.Evaluations;
        }

        _log.Info($"Found move: {UciMoveTranslator.ToUciMove(bestMove)}");

        _completedSearchBestMove = bestMove;

        return bestMove;
    }

    private PieceMove CalculateBestMove(int depth, out int bestScore)
    {
        var alpha = int.MinValue / 2 - 1;
        var beta = int.MaxValue / 2 + 1;

        bestScore = alpha;

        var bestMove = new PieceMove();

        List<PieceMove> moveList;

        // Order the initial moves by their scores from the last depth, if any.
        // Otherwise order them based on....
        if (_initialMovesIterativeDeepeningShuffleOrder.Count > 0)
        {
            moveList = OrderFromIterativeDeepeningMoves();
        }
        else
        {
            moveList = new List<PieceMove>(_moveGeneration.CalculateAllMoves(_boardPosition));

            //OrderMovesInPlaceByEvaluation(moveList);
            OrderMovesInPlace(moveList, depth);
        }

        _log.Info("---------------------------------------------------------");
        _log.Info($"Moves to Check: {moveList.Count}");
        _log.Info("Shown in order checked");

        for (var i = 0; i < moveList.Count; i++)
        {
            var move = moveList[i];
#if UCI
            Console.WriteLine($"info currmove {UciMoveTranslator.ToUciMove(move)} currmovenumber {i + 1}");
#endif

            var movePath = new List<PieceMove>();

            _pieceMover.MakeMove(move);

            // Since we're swapping colours at the next depth invert alpha and beta
            var score = -AlphaBeta(-beta, -alpha, depth - 1, true, false, movePath, 0);

            if (score > bestScore)
            {
                bestMove = move;
                bestScore = score;

                _inDepthBestMove = bestMove;
            }

            _initialMovesIterativeDeepeningShuffleOrder.Add(new Tuple<int, PieceMove>(score, move));

            _pieceMover.UnMakeLastMove();

            //Print move path
            _log.Info($"Move: {UciMoveTranslator.ToUciMove(move)} - " +
                       $"Score: {GetScoreString(score, _boardPosition.WhiteToMove)} - " +
                       $"Best path: { string.Join(", ", movePath.Select(UciMoveTranslator.ToUciMove)) }");
        }

        return bestMove;
    }

    // Breakdown of steps
    // 1. Check the transposition table for the same position
    // 2. If we are at depth 0 move to a quiescence search, or if in check, extend the depth
    // 3. Decide whether to do a null move check or not
    // 4. Generate moves
    // 5. If we think it's suitable, do an internal iterative deepening search for a best move,
    //    otherwise order moves by best hash move, killer moves and MVV/LVA
    // 6. Loop through the moves recursively calling AlphaBeta
    // 7. Record alpha, beta or evaluation to the transposition table
    private int AlphaBeta(int alpha, int beta, int depthLeft, bool allowNull, bool isNullSearch, List<PieceMove> pvPath, int extensionDepth)
    {
        var pvPosition = pvPath.Count;

        var positionValue = int.MinValue / 2 + 1;

        PieceMove? bestHashMove = null;

        // Check transposition table
        var hash = TranspositionTable.ProbeTable(_boardPosition.Zobrist, depthLeft, alpha, beta);

        if (hash.Key !=0)
        {
            var transpositionScore = hash.Score;

            if (hash.BestMove.Type != PieceType.None)
            {
                bestHashMove = hash.BestMove;
            }

            // We use the best move even if the depth is shallower.
            if (hash.Depth >= depthLeft)
            {
                switch (hash.NodeType)
                {
                    case HashNodeType.Exact:
                        return transpositionScore;
                    case HashNodeType.LowerBound:
                        alpha = Math.Max(alpha, transpositionScore);
                        break;
                    case HashNodeType.UpperBound:
                        beta = Math.Min(beta, transpositionScore);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (alpha >= beta)
                {
                    return transpositionScore;
                }
            }
        }

        // If in check do a check extension otherwise perform quiescence search
        if (depthLeft == 0)
        {
            if (extensionDepth < _maxCheckExtension && BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove))
            {
                depthLeft++;
                extensionDepth++;
            }
            else
            {
                return QuiescenceEvaluate(alpha, beta, pvPath);
            }
        }

        // Null move check
        if (allowNull && depthLeft > _nullMoveR && !BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove))
        {
            _boardPosition.SwitchSides();
            // We don't care about the PV path. Maybe we should implement this differently
            // Set extensionDepth to _maxCheckExtension because we don't want it to get
            // extended after the null move check
            var eval = -AlphaBeta(-beta,
                                  -beta + 1,
                                  depthLeft - _nullMoveR - 1,
                                  false,
                                  true,
                                  new List<PieceMove>(),
                                  _maxCheckExtension);

            _boardPosition.SwitchSides();

            if (eval >= beta)
            {
                return eval; // Cutoff
            }
        }

        var moveList = new List<PieceMove>(_moveGeneration.CalculateAllPseudoLegalMoves(_boardPosition));

        // There are no possible moves. It's either check mate or stale mate
        if (moveList.Count == 0)
        {
            return EvaluateEndGame(depthLeft);
        }

        // Internal iterative deepening
        // If we're near the last node of this search and we have no best move from
        // the transposition table perform a limited iterative deepening search
        if (!isNullSearch && bestHashMove == null && depthLeft > 3)
        {
            _bestMoveSoFar = null;

            // Call this just to get the best move
            // We don't care about the PV path. Maybe we should implement this differently
            // Set extensionDepth to m_MaxCheckExtension because we don't want any further deepening
            AlphaBeta(alpha,
                      beta,
                      depthLeft - 1,
                      false,
                      false,
                      new List<PieceMove>(),
                      _maxCheckExtension);

            if (_bestMoveSoFar != null)
            {
                OrderMovesInPlace(moveList, depthLeft, _bestMoveSoFar);
            }
            else
            {
                //This never seems to be hit. I'm not sure if it's needed
                OrderMovesInPlace(moveList, depthLeft, null);
            }
        }
        else
        {
            OrderMovesInPlace(moveList, depthLeft, bestHashMove);
        }

        PieceMove? bestMoveSoFar = null;
        var bestScoreSoFar = positionValue;

        // Check the colour before moving since it's this colour king we have to check for
        // legal move generation.
        // Plus, we only have to do it once for all moves.
        var colourToMove = _boardPosition.WhiteToMove;

        var noMovesAnalysed = true;

        var isInitialMove = true;

        foreach (var move in moveList)
        {
            if (!IsMoveValid(move, colourToMove))
            {
                continue;
            }

            _pieceMover.MakeMove(move);

            // Futility pruning
            // if (depthLeft == 1 &&
            //     move.SpecialMove != SpecialMoveType.Capture &&
            //     move.SpecialMove != SpecialMoveType.ENPassantCapture)
            // {
            //     if (Evaluate(_boardPosition) + 1.25m < alpha)
            //     {
            //         _pieceMover.UnMakeLastMove();
            //         continue;
            //     }
            // }

            // Since we do pseudo legal move generation we need to check if this move is legal
            // otherwise skip to the next iteration of the loop
            if (BoardChecking.IsKingInCheck(_boardPosition, colourToMove))
            {
                _pieceMover.UnMakeLastMove();
                continue;
            }

            var bestPath = new List<PieceMove>();

            int score;

            // if this is the first (pv move) then do a full search
            if (isInitialMove)
            {
                score = -AlphaBeta(-beta, -alpha, depthLeft - 1, true, false, bestPath, extensionDepth);
                isInitialMove = false;
            }
            else
            {
                // Principal variation search - Do a search with a narrow aspiration window
                score = -AlphaBeta(-alpha - 1,
                                   -alpha,
                                   depthLeft - 1,
                                   true,
                                   false,
                                   bestPath,
                                   extensionDepth);

                // If it fails high do a full search
                if (score > alpha && score < beta)
                {
                    score = -AlphaBeta(-beta,
                                       -alpha,
                                       depthLeft - 1,
                                       true,
                                       false,
                                       new List<PieceMove>(),
                                       extensionDepth);
                }
            }

            noMovesAnalysed = false;

            _pieceMover.UnMakeLastMove();

            // Save the best move so far for the transposition table
            if (score > bestScoreSoFar)
            {
                bestMoveSoFar = move;
                bestScoreSoFar = score;

                _bestMoveSoFar = move;

                pvPath.RemoveRange(pvPosition, pvPath.Count - pvPosition);
                pvPath.Add(move);
                pvPath.AddRange(bestPath);
            }

            positionValue = Math.Max(alpha, score);

            alpha = Math.Max(alpha, positionValue);

            if (alpha >= beta)
            {
                RecordHash(depthLeft, score, HashNodeType.LowerBound, bestMoveSoFar);

                // Check if the current move is already in the killer move list
                var duplicate = false;

                for (var i = 0; i < _killerMovesToStore; i++)
                {
                    if (move == _killerMoves[depthLeft-1,i])
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate == false)
                {
                    //Shift killer moves to the right
                    for (var i = _killerMovesToStore - 2; i >= 0; i--)
                    {
                        _killerMoves[depthLeft - 1, i + 1] = _killerMoves[depthLeft - 1, i];
                    }

                    //Insert killer move at index 0
                    _killerMoves[depthLeft - 1, 0] = move;
                }

                return score;
            }
        }

        if (noMovesAnalysed)
        {
            return EvaluateEndGame(depthLeft);
        }

        // transposition table store
        var hashNodeType = positionValue <= alpha ? HashNodeType.UpperBound
                                                  : HashNodeType.Exact;

        RecordHash(depthLeft, positionValue, hashNodeType, bestMoveSoFar);

        return positionValue;
    }

    private bool IsMoveValid(PieceMove move, bool colourToMove)
    {
        // Since we do pseudo legal move generation we need to validate
        // any castling moves, otherwise skip to the next iteration of the loop
        if (move.SpecialMove == SpecialMoveType.KingCastle)
        {
            if (BoardChecking.IsKingInCheck(_boardPosition, colourToMove))
            {
                return false;
            }

            if (!_moveGeneration.ValidateKingsideCastlingMove(_boardPosition))
            {
                return false;
            }
        }
        else if (move.SpecialMove == SpecialMoveType.QueenCastle)
        {
            if (BoardChecking.IsKingInCheck(_boardPosition, colourToMove))
            {
                return false;
            }

            if (!_moveGeneration.ValidateQueensideCastlingMove(_boardPosition))
            {
                return false;
            }
        }

        return true;
    }

    private void RecordHash(int depth, int score, HashNodeType hashNodeType, PieceMove? bestMove = null)
    {
        var hash = new Hash
                   {
                       Key      = _boardPosition.Zobrist,
                       Depth    = depth,
                       NodeType = hashNodeType,
                       Score    = score
                   };

        if (bestMove != null)
        {
            hash.BestMove = (PieceMove)bestMove;
        }

        TranspositionTable.Add(hash);
    }

    // Evaluates the score relative to the current player
    // i.e. A high score means the position is better for the current player
    private int Evaluate(Board boardPosition)
    {
        int score;

        if (_boardPosition.WhiteToMove)
        {
            score = _scoreCalculator.CalculateScore(boardPosition);
        }
        else
        {
            score = -_scoreCalculator.CalculateScore(boardPosition);
        }

        return score;
    }

    // Keep examining down the tree until all moves are quiet.
    // Quiet moves are ones without captures
    private int QuiescenceEvaluate(int alpha, int beta, List<PieceMove> pvPath)
    {
        var pvPosition = pvPath.Count;

        // Check transposition table
        var hash = TranspositionTable.ProbeQuiescenceTable(_boardPosition.Zobrist, alpha, beta);

        PieceMove? bestHashMove = null;

        if (hash.Key != 0)
        {
            var transpositionScore = hash.Score;

            if (hash.BestMove.Type != PieceType.None)
            {
                bestHashMove = hash.BestMove;
            }

            switch (hash.NodeType)
            {
                case HashNodeType.Exact:
                    return transpositionScore;
                case HashNodeType.LowerBound:
                    alpha = Math.Max(alpha, transpositionScore);
                    break;
                case HashNodeType.UpperBound:
                    beta = Math.Min(beta, transpositionScore);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (alpha >= beta)
            {
                return transpositionScore;
            }
        }

        var evaluationScore = Evaluate(_boardPosition);

        if (evaluationScore >= beta)
        {
            RecordQuiescenceHash(beta, HashNodeType.LowerBound);

            return beta;
        }

        if (evaluationScore >= alpha)
        {
            alpha = evaluationScore;
        }

        var moves = _moveGeneration.CalculateAllCapturingMoves(_boardPosition);

        if (moves.Count == 0)
        {
            // We are effectively returning the max of evaluationScore and alpha
            return alpha;
        }

        MoveOrdering.OrderMovesByMvvLva(_boardPosition, moves);

        if (bestHashMove != null)
        {
            BringBestHashMoveToTheFront(moves, (PieceMove)bestHashMove);
        }

        PieceMove? bestMoveSoFar = null;

        foreach (var move in moves)
        {
            var currentPath = new List<PieceMove> { move };

            _pieceMover.MakeMove(move);

            evaluationScore = -QuiescenceEvaluate(-beta, -alpha, currentPath);

            _pieceMover.UnMakeLastMove();

            if (evaluationScore >= beta)
            {
                RecordQuiescenceHash(beta, HashNodeType.LowerBound);

                return beta;
            }

            if (evaluationScore > alpha)
            {
                alpha = evaluationScore;

                bestMoveSoFar = move;

                pvPath.RemoveRange(pvPosition, pvPath.Count - pvPosition);
                pvPath.AddRange(currentPath);
            }
        }

        // transposition table store
        var hashNodeType = evaluationScore <= alpha ? HashNodeType.UpperBound
                                                    : HashNodeType.Exact;

        RecordQuiescenceHash(alpha, hashNodeType, bestMoveSoFar);

        return alpha;
    }

    private void RecordQuiescenceHash(int evaluationScore, HashNodeType hashNodeType, PieceMove? bestMove = null)
    {
        var hash = new Hash
        {
            Key      = _boardPosition.Zobrist,
            NodeType = hashNodeType,
            Score    = evaluationScore
        };

        if (bestMove != null)
        {
            hash.BestMove = (PieceMove)bestMove;
        }

        TranspositionTable.AddQuiescenceHash(hash);
    }

    private List<PieceMove> OrderFromIterativeDeepeningMoves()
    {
        var moveList = new List<PieceMove>();

        _initialMovesIterativeDeepeningShuffleOrder =
            _initialMovesIterativeDeepeningShuffleOrder.OrderByDescending(i => i.Item1).ToList();

        foreach (var move in _initialMovesIterativeDeepeningShuffleOrder)
        {
            moveList.Add(move.Item2);
        }

        _initialMovesIterativeDeepeningShuffleOrder.Clear();

        return moveList;
    }

    // Order all moves by MVV/LVA
    private void OrderMovesInPlace(IList<PieceMove> moveList, int depth)
    {
        MoveOrdering.OrderMovesByMvvLva(_boardPosition, moveList);

        BringKillerMovesToTheFront(moveList, depth);
    }

    private void OrderMovesInPlace(IList<PieceMove> moveList, int depth, PieceMove? bestHashMove)
    {
        MoveOrdering.OrderMovesByMvvLva(_boardPosition, moveList);

        BringKillerMovesToTheFront(moveList, depth);

        if (bestHashMove != null)
        {
            BringBestHashMoveToTheFront(moveList, (PieceMove)bestHashMove);
        }
    }

    private void BringKillerMovesToTheFront(IList<PieceMove> moveList, int depth)
    {
        for (var slot = 0; slot < _killerMovesToStore; slot++)
        {
            var killerMove = _killerMoves[depth-1, slot];

            // There are no more killer moves at this depth
            if (killerMove.Type == PieceType.None)
            {
                break;
            }

            for (var i = 0; i < moveList.Count; i++)
            {
                var move = moveList[i];

                if (move == killerMove)
                {
                    var toMove = moveList[i];
                    moveList.RemoveAt(i);
                    moveList.Insert(0, toMove);

                    break;
                }
            }
        }
    }

    private static void BringBestHashMoveToTheFront(IList<PieceMove> moveList, PieceMove bestHashMove)
    {
        if (moveList.Remove(bestHashMove))
        {
            moveList.Insert(0, bestHashMove);
        }
    }

    // Evaluates the end game relative to the current player
    // (i.e. A low score if the current player loses)
    private int EvaluateEndGame(int depth)
    {
        var movesToEnd = (_evaluationDepth - depth) + 1;  //Since we want lower depth mates to score lower

        var isInCheck = BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove);

        if (isInCheck)
        {
            return int.MinValue / 2 + movesToEnd; // Player is in checkmate
        }
        else
        {
            return 0; //stalemate
        }
    }

    private void LogTranspositionCounts()
    {
        _log.Info("----------------------------------------------------------------------------------");
        _log.Info("TRANSPOSITION DEBUGGING");
        _log.Info($"Times hash added:              {CountDebugger.Transposition_HashAdded}");
        _log.Info($"Times hash replaced:           {CountDebugger.Transposition_HashReplaced}");
        _log.Info($"Times probed:                  {CountDebugger.Transposition_Searches}");
        _log.Info($"Times hash returned:           {CountDebugger.Transposition_HashFound}");
        _log.Info($"Times keys matched:            {CountDebugger.Transposition_MatchCount}");
        _log.Info($"Times keys and depths matched: {CountDebugger.Transposition_MatchAndUsed}");
        _log.Info($"Times not matched:             {CountDebugger.Transposition_Searches - CountDebugger.Transposition_MatchAndUsed}");
        _log.Info("----------------------------------------------------------------------------------");
    }

    private void LogKillerMoves(PieceMove[,] killerMoves)
    {
        _log.Info("----------------------------------------------------------------------------------");
        _log.Info("KILLER MOVES");

        for (var depth = 0; depth < killerMoves.GetLength(0); depth++)
        {
            var killerMovesList = new List<string>();

            for (var move = 0; move < killerMoves.GetLength(1); move++)
            {
                var killerMove = killerMoves[depth, move];

                if (killerMove.Type != PieceType.None)
                {
                    killerMovesList.Add(UciMoveTranslator.ToUciMove(killerMoves[depth, move]));
                }
            }

            if (killerMovesList.Any())
            {
                _log.Info($"Depth: {depth + 1} - {string.Join(",", killerMovesList)}");
            }
        }

        _log.Info("----------------------------------------------------------------------------------");
    }

    private static string GetScoreString(int score, bool whiteToMove)
    {
        //We try to maximise the scores so for display purposes negate them for black
        if (!whiteToMove)
        {
            score = -score;
        }

        var scoreString = score.ToString(CultureInfo.InvariantCulture);

        if (score > int.MaxValue / 3)
        {
            var movesToMate = ((int.MaxValue / 2) - score) / 2;

            scoreString = $"+M{movesToMate}";
        }
        else if (score < int.MinValue / 3)
        {
            var movesToMate = Math.Abs((int.MinValue / 2 - score) / 2);

            scoreString = $"-M{movesToMate}";
        }

        return scoreString;
    }

    public List<MoveValueInfo> GetMoveValueInfo()
    {
        return _initialMoves;
    }
}

