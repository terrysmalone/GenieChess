﻿using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.Debugging;
using ChessEngine.PossibleMoves;

namespace ChessEngine.ScoreCalculation;

    // Calculates the score of a particular board position
public class ScoreCalculator : IScoreCalculator
{
    private readonly List<IScoreCalculation> _scoreCalculations;
    private readonly ScoreValues _scoreValues;
    private Board _currentBoard;

    private const byte _endGameCount = 3;

    private bool _isEndGame;

    public ScoreCalculator(List<IScoreCalculation> scoreCalculations, ScoreValues scoreValues)
    {
        _scoreCalculations = scoreCalculations;
        _scoreValues = scoreValues;
    }

    // Calculates the score with black advantage as negative and white as positive
    public int CalculateScore(Board currentBoard)
    {
        CountDebugger.Evaluations++;

        _currentBoard = currentBoard;

        //StaticBoardChecks.Calculate(currentBoard);

        DetectEndGame();

        var score = 0;

        foreach (var scoreCalculation in _scoreCalculations)
        {
            score += scoreCalculation.Calculate(currentBoard);
        }

        score +=  CalculatePawnStructureScore();
        score += CalculateCentralPieceScore();

        score += CalculateCastlingScores();
        score += CalculateCanCastleScores();

        //Add king pawn protection scores
        //Add king position scores (stay away from the middle early/mid game)

        score += CalculateDevelopmentScore();

        score += CalculateSquareTableScores();

        score += CalculateCoverageAndAttackScores();

        return score;
    }

    private void DetectEndGame()
    {

        if(   BitboardOperations.GetPopCount(_currentBoard.WhiteNonEndGamePieces) < _endGameCount
           || BitboardOperations.GetPopCount(_currentBoard.BlackNonEndGamePieces) < _endGameCount)
        {
            _isEndGame = true;
        }
        else
        {
            _isEndGame = false;
        }

    }

    private int CalculatePawnStructureScore()
    {
        var pawnStructureScore = 0;

        pawnStructureScore += CalculateDoubledPawnScore();
        pawnStructureScore += CalculateProtectedPawnScore();
        pawnStructureScore += CalculatePassedPawnScore();

        return pawnStructureScore;
    }

    private int CalculateDoubledPawnScore()
    {
        var doubledPawnScore = 0;

        var whiteDoubleCount = 0;
        var blackDoubleCount = 0;

        for (var i = 0; i < 8; i++)
        {
            var mask = LookupTables.ColumnMaskByColumn[i];

            if (BitboardOperations.GetPopCount(mask & _currentBoard.WhitePawns) > 1)
            {
                whiteDoubleCount++;
            }

            if (BitboardOperations.GetPopCount(mask & _currentBoard.BlackPawns) > 1)
            {
                blackDoubleCount++;
            }
        }

        doubledPawnScore += whiteDoubleCount * _scoreValues.DoubledPawnScore;
        doubledPawnScore -= blackDoubleCount * _scoreValues.DoubledPawnScore;

        return doubledPawnScore;
    }

    private int CalculateProtectedPawnScore()
    {

        var protectedPawnScore = 0;

        //Pawn chain
        var notA = 18374403900871474942;
        ulong notH = 9187201950435737471;

        //White pawns
        var wPawnAttackSquares = ((_currentBoard.WhitePawns << 9) & notA) | (_currentBoard.WhitePawns << 7) & notH;
        var wProtectedPawns = wPawnAttackSquares & _currentBoard.WhitePawns;

        protectedPawnScore += BitboardOperations.GetPopCount(wProtectedPawns) * _scoreValues.ProtectedPawnScore;

        //Black pawns
        var bPawnAttackSquares = ((_currentBoard.BlackPawns >> 9) & notH) | (_currentBoard.BlackPawns >> 7) & notA;
        var bProtectedPawns = bPawnAttackSquares & _currentBoard.BlackPawns;

        protectedPawnScore -= BitboardOperations.GetPopCount(bProtectedPawns) * _scoreValues.ProtectedPawnScore;

        return protectedPawnScore;
    }

    private int CalculatePassedPawnScore()
    {
        var passedPawnScore = 0;

        foreach (var whitePawnBoard in BitboardOperations.SplitBoardToArray(_currentBoard.WhitePawns))
        {
            // Pawn is on 7th rank so it can promote
            if ((whitePawnBoard & LookupTables.RowMask7) != 0)
            {
                passedPawnScore += _scoreValues.PassedPawnScore;
                passedPawnScore += _scoreValues.PassedPawnAdvancementScore * 5;

                continue;
            }

            var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(whitePawnBoard);

            var pawnFrontSpan = LookupTables.WhitePawnFrontSpan[pawnIndex];

            if ((pawnFrontSpan & _currentBoard.BlackPawns) == 0)
            {
                passedPawnScore += _scoreValues.PassedPawnScore;
                passedPawnScore += _scoreValues.PassedPawnAdvancementScore * ((pawnIndex / 8) - 1);
            }
        }

        foreach (var blackPawnBoard in BitboardOperations.SplitBoardToArray(_currentBoard.BlackPawns))
        {
            //Pawn is on 2nd rank so it can promote
            if ((blackPawnBoard & LookupTables.RowMask2) != 0)
            {
                passedPawnScore -= _scoreValues.PassedPawnScore;
                passedPawnScore -= _scoreValues.PassedPawnAdvancementScore * 5;
                continue;
            }

            var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(blackPawnBoard);

            var pawnFrontSpan = LookupTables.BlackPawnFrontSpan[pawnIndex];

            if ((pawnFrontSpan & _currentBoard.WhitePawns) == 0)
            {
                passedPawnScore -= _scoreValues.PassedPawnScore;
                passedPawnScore -= _scoreValues.PassedPawnAdvancementScore * (8 - (pawnIndex / 8) - 2);
            }
        }

        return passedPawnScore;
    }

    // Points for placing pieces near to the centre
    private int CalculateCentralPieceScore()
    {
        var innerCentralSquares = (ulong)103481868288;
        var outerCentralSquares = (ulong)66125924401152;

        var piecePositionScore = 0;

        //Pawns
        var whitePawnBoard = _currentBoard.WhitePawns;

        piecePositionScore += CalculatePositionScores(whitePawnBoard, innerCentralSquares) * _scoreValues.InnerCentralPawnScore;
        piecePositionScore += CalculatePositionScores(whitePawnBoard, outerCentralSquares) * _scoreValues.OuterCentralPawnScore;

        var blackPawnBoard = _currentBoard.BlackPawns;

        piecePositionScore -= CalculatePositionScores(blackPawnBoard, innerCentralSquares) * _scoreValues.InnerCentralPawnScore;
        piecePositionScore -= CalculatePositionScores(blackPawnBoard, outerCentralSquares) * _scoreValues.OuterCentralPawnScore;

        //Knights
        var whiteKnightBoard = _currentBoard.WhiteKnights;

        piecePositionScore += CalculatePositionScores(whiteKnightBoard, innerCentralSquares) * _scoreValues.InnerCentralKnightScore;
        piecePositionScore += CalculatePositionScores(whiteKnightBoard, outerCentralSquares) * _scoreValues.OuterCentralKnightScore;

        var blackKnightBoard = _currentBoard.BlackKnights;

        piecePositionScore -= CalculatePositionScores(blackKnightBoard, innerCentralSquares) * _scoreValues.InnerCentralKnightScore;
        piecePositionScore -= CalculatePositionScores(blackKnightBoard, outerCentralSquares) * _scoreValues.OuterCentralKnightScore;

        //Bishops
        var whiteBishopBoard = _currentBoard.WhiteBishops;

        piecePositionScore += CalculatePositionScores(whiteBishopBoard, innerCentralSquares) * _scoreValues.InnerCentralBishopScore;
        piecePositionScore += CalculatePositionScores(whiteBishopBoard, outerCentralSquares) * _scoreValues.OuterCentralBishopScore;

        var blackBishopBoard = _currentBoard.BlackBishops;

        piecePositionScore -= CalculatePositionScores(blackBishopBoard, innerCentralSquares) * _scoreValues.InnerCentralBishopScore;
        piecePositionScore -= CalculatePositionScores(blackBishopBoard, outerCentralSquares) * _scoreValues.OuterCentralBishopScore;

        //Rooks
        var whiteRookBoard = _currentBoard.WhiteRooks;

        piecePositionScore += CalculatePositionScores(whiteRookBoard, innerCentralSquares) * _scoreValues.InnerCentralRookScore;
        piecePositionScore += CalculatePositionScores(whiteRookBoard, outerCentralSquares) * _scoreValues.OuterCentralRookScore;

        var blackRookBoard = _currentBoard.BlackRooks;

        piecePositionScore -= CalculatePositionScores(blackRookBoard, innerCentralSquares) * _scoreValues.InnerCentralRookScore;
        piecePositionScore -= CalculatePositionScores(blackRookBoard, outerCentralSquares) * _scoreValues.OuterCentralRookScore;

        //Queens
        var whiteQueenBoard = _currentBoard.WhiteQueen;

        piecePositionScore += CalculatePositionScores(whiteQueenBoard, innerCentralSquares) * _scoreValues.InnerCentralQueenScore;
        piecePositionScore += CalculatePositionScores(whiteQueenBoard, outerCentralSquares) * _scoreValues.OuterCentralQueenScore;

        var blackQueenBoard = _currentBoard.BlackQueen;

        piecePositionScore -= CalculatePositionScores(blackQueenBoard, innerCentralSquares) * _scoreValues.InnerCentralQueenScore;
        piecePositionScore -= CalculatePositionScores(blackQueenBoard, outerCentralSquares) * _scoreValues.OuterCentralQueenScore;

        return piecePositionScore;
    }

    private static int CalculatePositionScores(ulong pieces, ulong positions)
    {
        var inPosition = pieces & positions;

        return inPosition > 0 ? BitboardOperations.GetPopCount(inPosition) : 0;
    }

    private int CalculateCastlingScores()
    {
        if (_isEndGame)
        {
            return 0;
        }

        var castlingScore = 0;

        if (_currentBoard.WhiteKing == LookupTables.G1 && (_currentBoard.WhiteRooks & LookupTables.F1) != 0)
        {
            castlingScore += _scoreValues.CastlingKingSideScore;
        }
        else if (_currentBoard.WhiteKing == LookupTables.C1 && (_currentBoard.WhiteRooks & LookupTables.D1) != 0)
        {
            castlingScore += _scoreValues.CastlingQueenSideScore;
        }

        if (_currentBoard.BlackKing == LookupTables.G8 && (_currentBoard.BlackRooks & LookupTables.F8) != 0)
        {
            castlingScore -= _scoreValues.CastlingKingSideScore;
        }
        else if (_currentBoard.BlackKing == LookupTables.C8 && (_currentBoard.BlackRooks & LookupTables.D8) != 0)
        {
            castlingScore -= _scoreValues.CastlingQueenSideScore;
        }

        return castlingScore;
    }

    private int CalculateCanCastleScores()
    {
        var canCastleScore = 0;

        var state = _currentBoard.GetCurrentBoardState();

        canCastleScore += Convert.ToInt32(state.WhiteCanCastleKingside) * _scoreValues.CanCastleKingsideScore;
        canCastleScore += Convert.ToInt32(state.WhiteCanCastleQueenside) * _scoreValues.CanCastleQueensideScore;

        canCastleScore -= Convert.ToInt32(state.BlackCanCastleKingside) * _scoreValues.CanCastleKingsideScore;
        canCastleScore -= Convert.ToInt32(state.BlackCanCastleQueenside) * _scoreValues.CanCastleQueensideScore;

        return canCastleScore;
    }

    private int CalculateSquareTableScores()
    {
        var squareTableScores = 0;

        squareTableScores += CalculatePawnSquareTableScores();
        squareTableScores += CalculateKnightSquareTableScores();
        squareTableScores += CalculateBishopSquareTableScores();

        if(!_isEndGame)
            squareTableScores += CalculateKingSquareTableScores();
        else
            squareTableScores += CalculateKingEndGameSquareTableScores();

        return squareTableScores;
    }

    private int CalculatePawnSquareTableScores()
    {
        var pawnSquareTableScore = 0;

        pawnSquareTableScore += CalculateTableScores(_currentBoard.WhitePawns, _scoreValues.PawnSquareTable, true);

        pawnSquareTableScore -= CalculateTableScores(_currentBoard.BlackPawns, _scoreValues.PawnSquareTable, false);

        return pawnSquareTableScore;
    }

    private int CalculateKnightSquareTableScores()
    {
        var knightSquareTableScore = 0;

        knightSquareTableScore += CalculateTableScores(_currentBoard.WhiteKnights, _scoreValues.KnightSquareTable, true);

        knightSquareTableScore -= CalculateTableScores(_currentBoard.BlackKnights, _scoreValues.KnightSquareTable, false);

        return knightSquareTableScore;
    }

    private int CalculateBishopSquareTableScores()
    {
        var bishopSquareTableScore = 0;

        bishopSquareTableScore += CalculateTableScores(_currentBoard.WhiteBishops, _scoreValues.BishopSquareTable, true);

        bishopSquareTableScore -= CalculateTableScores(_currentBoard.BlackBishops, _scoreValues.BishopSquareTable, false);

        return bishopSquareTableScore;
    }

    private int CalculateKingSquareTableScores()
    {
        var kingSquareTableScore = 0;

        kingSquareTableScore += CalculateTableScores(_currentBoard.WhiteKing, _scoreValues.KingSquareTable, true);

        kingSquareTableScore -= CalculateTableScores(_currentBoard.BlackKing, _scoreValues.KingSquareTable, false);

        return kingSquareTableScore;
    }

    private int CalculateKingEndGameSquareTableScores()
    {
        var kingEndGameSquareTableScore = 0;

        kingEndGameSquareTableScore += CalculateTableScores(_currentBoard.WhiteKing, _scoreValues.KingEndGameSquareTable, true);

        kingEndGameSquareTableScore -= CalculateTableScores(_currentBoard.BlackKing, _scoreValues.KingEndGameSquareTable, false);

        return kingEndGameSquareTableScore;
    }

    private static int CalculateTableScores(ulong board, int[] squareTableValues, bool isWhite)
    {
        var positions =
            BitboardOperations.GetSquareIndexesFromBoardValue(isWhite ? board : BitboardOperations.FlipVertical(board));

        return positions.Sum(squareIndex => squareTableValues[squareIndex]);
    }

    private int CalculateDevelopmentScore()
    {
        var developedPieceScore = 0;

        developedPieceScore += CalculateDevelopedPieceScore();
        developedPieceScore += CalculateConnectedRookScore();

        developedPieceScore += CalculateEarlyQueenScore();

        return developedPieceScore;
    }

    // Points for pieces not on the back rank (bishops & knights)
    private int CalculateDevelopedPieceScore()
    {
        var developedPiecesScore = 0;

        ulong whiteBack = 255;
        var blackBack = 18374686479671623680;

        var whiteDevelopedPieces = (_currentBoard.WhiteBishops ^ _currentBoard.WhiteKnights ^ _currentBoard.WhiteQueen) & ~whiteBack;
        var developedWhitePieceCount = BitboardOperations.GetPopCount(whiteDevelopedPieces);
        developedPiecesScore += developedWhitePieceCount * _scoreValues.DevelopedPieceScore;

        var blackDevelopedPieces = (_currentBoard.BlackBishops ^ _currentBoard.BlackKnights ^ _currentBoard.BlackQueen) & ~blackBack;
        var developedBlackPieceCount = BitboardOperations.GetPopCount(blackDevelopedPieces);
        developedPiecesScore -= developedBlackPieceCount * _scoreValues.DevelopedPieceScore;

        return developedPiecesScore;
    }

    private int CalculateConnectedRookScore()
    {
        var connectedRookScore = 0;

        if (BitboardOperations.GetPopCount(_currentBoard.WhiteRooks) > 1)
        {
            var rooks = BitboardOperations.SplitBoardToArray(_currentBoard.WhiteRooks);

            var firstRook = rooks[0];
            var secondRook = rooks[1];

            if ((BoardChecking.FindRightBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
            else if ((BoardChecking.FindLeftBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
            else if ((BoardChecking.FindUpBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
            else if ((BoardChecking.FindDownBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
        }

        if (BitboardOperations.GetPopCount(_currentBoard.BlackRooks) > 1)
        {
            var rooks = BitboardOperations.SplitBoardToArray(_currentBoard.BlackRooks);

            var firstRook = rooks[0];
            var secondRook = rooks[1];

            if ((BoardChecking.FindRightBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
            else if ((BoardChecking.FindLeftBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
            else if ((BoardChecking.FindUpBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
            else if ((BoardChecking.FindDownBlockingPosition(_currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
        }

        return connectedRookScore;
    }

    private int CalculateEarlyQueenScore()
    {
        var earlyQueenScoreScore = 0;

        ulong whiteUndevelopedPieceBoard = 0;

        whiteUndevelopedPieceBoard |= _currentBoard.WhiteBishops & 36;  //Any white bishops on C1 or F1
        whiteUndevelopedPieceBoard |= _currentBoard.WhiteKnights & 66;  //Any white knights on B1 orG1

        var whiteUndevelopedPieceCount = BitboardOperations.GetPopCount(whiteUndevelopedPieceBoard);

        // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
        if (whiteUndevelopedPieceCount >= 2 && (_currentBoard.WhiteQueen & ~8UL) != 0)
        {
            earlyQueenScoreScore -= _scoreValues.EarlyQueenMoveScore;
        }

        ulong blackUndevelopedPieceBoard = 0;

        //Any black bishops on C8 or F8
        blackUndevelopedPieceBoard |= _currentBoard.BlackBishops & 2594073385365405696;
        //Any black knights on B8 or G8
        blackUndevelopedPieceBoard |= _currentBoard.BlackKnights & 66;

        int blackUndevelopedPieceCount = BitboardOperations.GetPopCount(blackUndevelopedPieceBoard);

        // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
        if (blackUndevelopedPieceCount >= 2 && (_currentBoard.BlackQueen & ~576460752303423488UL) != 0)
        {
            earlyQueenScoreScore += _scoreValues.EarlyQueenMoveScore;
        }

        return earlyQueenScoreScore;
    }

    // BoardCoverageScore
    //
    // QueenCoverageScore
    // RookCoverageScore
    // BishopCoverageScore
    //
    // AttackScore
    private int CalculateCoverageAndAttackScores()
    {
        var attackScore = 0;

        ulong whiteCoverageBoard = 0;   // All the empty squares white can move to next turn

        ulong whiteAttackBoard = 0;     // All squares with black pieces white can move to next turn

        // White
        // Score for bishop
        var whiteBishopPossibleMoves = BoardChecking.CalculateAllowedBishopMoves(_currentBoard, _currentBoard.WhiteBishops, true);                                       ;

        if (whiteBishopPossibleMoves > 0)
        {
            var whiteBishopCoverageBoard = whiteBishopPossibleMoves & ~_currentBoard.AllBlackOccupiedSquares;
            attackScore += BitboardOperations.GetPopCount(whiteBishopCoverageBoard)
                           * _scoreValues.BishopCoverageScore; // Add points for white bishop coverage

            whiteCoverageBoard |= whiteBishopCoverageBoard;

            // Add points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whiteBishopPossibleMoves
                & (_currentBoard.BlackQueen | _currentBoard.BlackRooks))
                * _scoreValues.MoreValuablePieceAttackScore;

            whiteAttackBoard |= whiteBishopPossibleMoves & _currentBoard.AllBlackOccupiedSquares;
        }

        // Score for rook coverage
        var whiteRookPossibleMoves = BoardChecking.CalculateAllowedRookMoves(_currentBoard,
                                                                             _currentBoard.WhiteRooks,
                                                                             true);

        if (whiteRookPossibleMoves > 0)
        {
            var whiteRookCoverageBoard =
                whiteRookPossibleMoves
                & ~_currentBoard.AllBlackOccupiedSquares;

            attackScore += BitboardOperations.GetPopCount(whiteRookCoverageBoard) * _scoreValues.RookCoverageScore;

            whiteCoverageBoard |= whiteRookCoverageBoard;

            // Add points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whiteRookPossibleMoves & _currentBoard.BlackQueen) * _scoreValues.MoreValuablePieceAttackScore;

            whiteAttackBoard |= whiteRookPossibleMoves & _currentBoard.AllBlackOccupiedSquares;
        }

        // Score for queen coverage
        var whiteQueenPossibleMoves = BoardChecking.CalculateAllowedQueenMoves(_currentBoard,
                                                                               _currentBoard.WhiteQueen,
                                                                               true);
        if (whiteQueenPossibleMoves > 0)
        {
            var whiteQueenCoverageBoard = whiteQueenPossibleMoves & ~_currentBoard.AllBlackOccupiedSquares;
            attackScore += BitboardOperations.GetPopCount(whiteQueenCoverageBoard) * _scoreValues.QueenCoverageScore;

            whiteCoverageBoard |= whiteQueenCoverageBoard;

            whiteAttackBoard |= whiteQueenPossibleMoves & _currentBoard.AllBlackOccupiedSquares;
        }

        // Score for knight coverage
        ulong whiteKnightPossibleMoves = 0;

        var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteKnights);

        foreach (var knightPos in whiteKnightPositions)
	    {
            var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

            possibleMoves = possibleMoves & ~_currentBoard.AllWhiteOccupiedSquares;
            whiteKnightPossibleMoves |= possibleMoves;
	    }

        whiteCoverageBoard |= whiteKnightPossibleMoves & ~_currentBoard.AllBlackOccupiedSquares;

        whiteAttackBoard |= whiteKnightPossibleMoves & _currentBoard.AllBlackOccupiedSquares;

        // Points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(
            whiteKnightPossibleMoves & (_currentBoard.BlackQueen | _currentBoard.BlackRooks)) * _scoreValues.MoreValuablePieceAttackScore;

        //Pawns
        const ulong notA = 18374403900871474942;
        const ulong notH = 9187201950435737471;

        var whitePawnPossibleAttackMoves = ((_currentBoard.WhitePawns << 9) & notA)
                                           | ((_currentBoard.WhitePawns << 7) & notH);

        if (whitePawnPossibleAttackMoves > 0)
        {
            // There is no Score for pawn coverage, just pawn attacks

            // Add points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whitePawnPossibleAttackMoves
                    & (_currentBoard.BlackQueen
                       | _currentBoard.BlackRooks
                       | _currentBoard.BlackBishops
                       | _currentBoard.BlackKnights))
            * _scoreValues.MoreValuablePieceAttackScore;

            whiteAttackBoard |= whitePawnPossibleAttackMoves & _currentBoard.AllBlackOccupiedSquares;
        }

        // Points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(
            whiteKnightPossibleMoves
            & (_currentBoard.BlackQueen | _currentBoard.BlackRooks))
            * _scoreValues.MoreValuablePieceAttackScore;

        // Points for overall board coverage
        attackScore += BitboardOperations.GetPopCount(whiteCoverageBoard) * _scoreValues.BoardCoverageScore;

        // Points for board attacks
        attackScore += BitboardOperations.GetPopCount(whiteAttackBoard) * _scoreValues.AttackScore;

        //Black
        ulong blackCoverageBoard = 0;   // All the empty squares black can move to next turn
        ulong blackAttackBoard = 0;    // All white squares black can move to next turn

        // Score for bishop coverage
        var blackBishopPossibleMoves = BoardChecking.CalculateAllowedBishopMoves(_currentBoard,
                                                                                 _currentBoard.BlackBishops,
                                                                                 false);

        if (blackBishopPossibleMoves > 0)
        {
            var blackBishopCoverageBoard = blackBishopPossibleMoves & ~_currentBoard.AllWhiteOccupiedSquares;

            // Add points for black bishop coverage
            attackScore -= BitboardOperations.GetPopCount(blackBishopCoverageBoard)
                           * _scoreValues.BishopCoverageScore;

            blackCoverageBoard |= blackBishopCoverageBoard;

            // Points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(
                blackBishopPossibleMoves & (_currentBoard.WhiteQueen | _currentBoard.WhiteRooks))
                * _scoreValues.MoreValuablePieceAttackScore;

            blackAttackBoard |= blackBishopPossibleMoves & _currentBoard.AllWhiteOccupiedSquares;
        }

        // Score for rook coverage
        var blackRookPossibleMoves = BoardChecking.CalculateAllowedRookMoves(_currentBoard,
                                                                             _currentBoard.BlackRooks,
                                                                             false);

        if (blackRookPossibleMoves > 0)
        {
            var blackRookCoverageBoard = blackRookPossibleMoves & ~_currentBoard.AllWhiteOccupiedSquares;

            // Add points for black rook coverage
            attackScore -= BitboardOperations.GetPopCount(blackRookCoverageBoard) * _scoreValues.RookCoverageScore;

            blackCoverageBoard |= blackRookCoverageBoard;

            // Points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(
                blackRookPossibleMoves & _currentBoard.WhiteQueen) * _scoreValues.MoreValuablePieceAttackScore;

            blackAttackBoard |= blackRookPossibleMoves & _currentBoard.AllWhiteOccupiedSquares;
        }

        // Score for queen coverage
        var blackQueenPossibleMoves = BoardChecking.CalculateAllowedQueenMoves(_currentBoard,
                                                                               _currentBoard.BlackQueen,
                                                                               false);

        if (blackQueenPossibleMoves > 0)
        {
            var blackQueenCoverageBoard = blackQueenPossibleMoves & ~_currentBoard.AllWhiteOccupiedSquares;
            attackScore -= BitboardOperations.GetPopCount(blackQueenCoverageBoard) * _scoreValues.QueenCoverageScore;

            blackCoverageBoard |= blackQueenCoverageBoard;

            blackAttackBoard |= blackQueenPossibleMoves & _currentBoard.AllWhiteOccupiedSquares;
        }

        //// Score for knight coverage
        ulong blackKnightPossibleMoves = 0;

        var blackKnightPositions =
            BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackKnights);

        foreach (var knightPos in blackKnightPositions)
        {
            var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

            possibleMoves = possibleMoves & ~_currentBoard.AllBlackOccupiedSquares;
            blackKnightPossibleMoves |= possibleMoves;
        }

        blackCoverageBoard |= blackKnightPossibleMoves & ~_currentBoard.AllWhiteOccupiedSquares;

        blackAttackBoard |= blackKnightPossibleMoves & _currentBoard.AllWhiteOccupiedSquares;

        // Points for every attack on a more valuable piece
        attackScore -= BitboardOperations.GetPopCount(
            blackKnightPossibleMoves & (_currentBoard.WhiteQueen | _currentBoard.WhiteRooks))
            * _scoreValues.MoreValuablePieceAttackScore;

        //Pawns
        var blackPawnPossibleAttackMoves = ((_currentBoard.BlackPawns >> 7) & notA)
                                           | ((_currentBoard.BlackPawns >> 7) & notH);

        if (blackPawnPossibleAttackMoves > 0)
        {
            // There is no Score for pawn coverage, just pawn attacks
            // Add points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(
                blackPawnPossibleAttackMoves & (_currentBoard.WhiteQueen
                                                | _currentBoard.WhiteRooks
                                                | _currentBoard.WhiteBishops
                                                | _currentBoard.WhiteKnights))
                * _scoreValues.MoreValuablePieceAttackScore;

            blackAttackBoard |= blackPawnPossibleAttackMoves & _currentBoard.AllWhiteOccupiedSquares;
        }

        // Points for overall board coverage
        attackScore -= BitboardOperations.GetPopCount(blackCoverageBoard) * _scoreValues.BoardCoverageScore;

        // Points for board attacks
        attackScore -= BitboardOperations.GetPopCount(blackAttackBoard) * _scoreValues.AttackScore;

        return attackScore;
    }
}

