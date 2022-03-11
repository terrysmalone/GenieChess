using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngine.NotationHelpers;

//Translates between Board and a PGN string
public static class PgnTranslator
{
    // Creates a PGN move string from a board move
    public static string ToPgnMove(Board board, ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMoveType = SpecialMoveType.Normal)
    {
        var moveIsCapture = false;

        var movedPiece = BoardChecking.GetPieceTypeOnSquare(board, moveFromBoard);    //Can#t trust pieceToMove since it may have changed (i.e. promotion)

        var move = CheckForCastling(board, moveToBoard, movedPiece);

        if (!string.IsNullOrEmpty(move))
        {
            move = string.Copy(move);

            return move;
        }

        //Get string of piece to move
        move += TranslationHelper.GetPieceLetter(movedPiece, moveFromBoard);

        //Is it a capture, if so use 'x'
        if (BoardChecking.IsEnemyPieceOnSquare(board, moveToBoard))
        {
            moveIsCapture = true;
            move += "x";

            BoardChecking.GetPieceTypeOnSquare(board, moveToBoard);
        }

        //Get string of position moved to
        var moveTo = TranslationHelper.GetSquareNotation(moveToBoard);


        // TODO: If another piece can move here, specify which it was

        if (movedPiece == PieceType.Pawn && !moveIsCapture)
        {
            //We need to trim the first letter since it is a pawn
            moveTo = moveTo.Substring(1);
        }

        move += moveTo;

        //Is it a promotion
        if (specialMoveType is SpecialMoveType.BishopPromotion
                            or SpecialMoveType.KnightPromotion
                            or SpecialMoveType.RookPromotion
                            or SpecialMoveType.QueenPromotion
                            or SpecialMoveType.BishopPromotionCapture
                            or SpecialMoveType.KnightPromotionCapture
                            or SpecialMoveType.RookPromotionCapture
                            or SpecialMoveType.QueenPromotionCapture)
        {
            move += "=";
            
            var pieceLetter = specialMoveType switch
            {
                SpecialMoveType.KnightPromotion or SpecialMoveType.KnightPromotionCapture => "N",
                SpecialMoveType.BishopPromotion or SpecialMoveType.BishopPromotionCapture => "B",
                SpecialMoveType.RookPromotion or SpecialMoveType.RookPromotionCapture => "R",
                SpecialMoveType.QueenPromotion or SpecialMoveType.QueenPromotionCapture => "Q",
                _ => throw new ArgumentOutOfRangeException($"Invalid move type {specialMoveType}")
            };

            move += pieceLetter;
        }

        var pieceMover = new PieceMover(board);

        pieceMover.MakeMove(moveFromBoard, moveToBoard, pieceToMove, specialMoveType);

        if (BoardChecking.IsKingInCheck(board, board.WhiteToMove))
        {
            if (new MoveGeneration().CalculateAllMoves(board).Count > 0 || BoardChecking.CanKingMove(board, board.WhiteToMove))
            {
                move += "+";
            }
            else
            {
                move += "#";
            }
        }

        pieceMover.UnMakeLastMove();

        //If mate add score

        Debug.Assert(!string.IsNullOrEmpty(move));

        return move;
    }

    private static string CheckForCastling(Board board, ulong moveToBoard, PieceType pieceToMove)
    {
        //Castling flag checks
        if (board.WhiteToMove)
        {
            if (board.WhiteCanCastleKingside)
            {
                if (pieceToMove == PieceType.King)
                {
                    if (moveToBoard == LookupTables.G1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                    {
                        return "0-0";
                    }
                }
            }

            if (board.WhiteCanCastleQueenside)
            {
                if (pieceToMove == PieceType.King)
                {
                    if (moveToBoard == LookupTables.C1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                    {
                        return "0-0-0";
                    }
                }
            }

        }
        else
        {
            if (board.BlackCanCastleKingside)
            {
                if (pieceToMove == PieceType.King)
                {
                    if (moveToBoard == LookupTables.G8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                    {
                        return "0-0";
                    }
                }
            }

            if (board.BlackCanCastleQueenside)
            {
                if (pieceToMove == PieceType.King)
                {
                    if (moveToBoard == LookupTables.C8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                    {
                        return "0-0-0";
                    }
                }
            }
        }

        return string.Empty;
    }
}

