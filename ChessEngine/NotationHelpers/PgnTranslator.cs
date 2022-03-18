using System.Text;
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
        var movedPiece = PieceChecking.GetPieceTypeOnSquare(board, moveFromBoard);    //Can't trust pieceToMove since it may have changed (i.e. promotion)

        var castling = CheckForCastling(board, moveToBoard, movedPiece);

        if (!string.IsNullOrEmpty(castling))
        {
            return castling;
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(TranslationHelper.GetPieceLetter(movedPiece, moveFromBoard));

        var captureString = GetCaptureString(board, moveToBoard);

        //Get string of position moved to
        var moveTo = TranslationHelper.GetSquareNotation(moveToBoard);

        if (movedPiece == PieceType.Pawn && string.IsNullOrEmpty(captureString))
        {
            //We need to trim the first letter since it is a pawn
            moveTo = moveTo[1..];
        }

        var moveGeneration = new MoveGeneration();
        var allMoves = moveGeneration.CalculateAllMoves(board).ToList();

        var matchingMoves = allMoves.Where(m => m.Moves == moveToBoard
                                                                  && m.Type == movedPiece
                                                                  && !IsPromotionMove(m.SpecialMove)).ToList();

        var positionLetter = string.Empty;

        if (matchingMoves.Count > 1)
        {
            var moveFromPosition = TranslationHelper.GetColumnAndRow(moveFromBoard);

            var otherPiece = matchingMoves.Single(m => m.Position != moveFromBoard);
            var otherPiecePosition = TranslationHelper.GetColumnAndRow(otherPiece.Position);

            positionLetter = moveFromPosition.Item1 == otherPiecePosition.Item1 ? moveFromPosition.Item2.ToString()
                                                                                : moveFromPosition.Item1;
        }

        stringBuilder.Append(positionLetter);
        stringBuilder.Append(captureString);
        stringBuilder.Append(moveTo);

        //Is it a promotion
        if (IsPromotionMove(specialMoveType))
        {
            stringBuilder.Append("=");
            
            var pieceLetter = specialMoveType switch
            {
                SpecialMoveType.KnightPromotion or SpecialMoveType.KnightPromotionCapture => "N",
                SpecialMoveType.BishopPromotion or SpecialMoveType.BishopPromotionCapture => "B",
                SpecialMoveType.RookPromotion or SpecialMoveType.RookPromotionCapture => "R",
                SpecialMoveType.QueenPromotion or SpecialMoveType.QueenPromotionCapture => "Q",
                _ => throw new ArgumentOutOfRangeException($"Invalid move type {specialMoveType}")
            };

            stringBuilder.Append(pieceLetter);
        }

        var pieceMover = new PieceMover(board);

        pieceMover.MakeMove(moveFromBoard, moveToBoard, pieceToMove, specialMoveType);

        if (PieceChecking.IsKingInCheck(board, board.WhiteToMove))
        {
            if (moveGeneration.CalculateAllMoves(board).Count > 0 || PieceChecking.CanKingMove(board, board.WhiteToMove))
            {
                stringBuilder.Append("+");
            }
            else
            {
                stringBuilder.Append("#");
            }
        }

        pieceMover.UnMakeLastMove();

        return stringBuilder.ToString();
    }
    private static bool IsPromotionMove(SpecialMoveType specialMoveType)
    {
        return specialMoveType is SpecialMoveType.BishopPromotion
                               or SpecialMoveType.KnightPromotion
                               or SpecialMoveType.RookPromotion
                               or SpecialMoveType.QueenPromotion
                               or SpecialMoveType.BishopPromotionCapture
                               or SpecialMoveType.KnightPromotionCapture
                               or SpecialMoveType.RookPromotionCapture
                               or SpecialMoveType.QueenPromotionCapture;
    }
    private static string GetCaptureString(Board board, ulong moveToBoard)
    {
        return PieceChecking.IsEnemyPieceOnSquare(board, moveToBoard) ? "x" : string.Empty;
    }

    private static string CheckForCastling(Board board, ulong moveToBoard, PieceType pieceToMove)
    {
        if (board.WhiteToMove)
        {
            if (board.WhiteCanCastleKingside && pieceToMove == PieceType.King && moveToBoard == LookupTables.G1)
            {
                return "0-0";
            }

            if (board.WhiteCanCastleQueenside && pieceToMove == PieceType.King && moveToBoard == LookupTables.C1)
            {
                return "0-0-0";
            }
        }
        else
        {
            if (board.BlackCanCastleKingside && pieceToMove == PieceType.King && moveToBoard == LookupTables.G8)
            {
                return "0-0";
            }

            if (board.BlackCanCastleQueenside && pieceToMove == PieceType.King && moveToBoard == LookupTables.C8)
            {
                return "0-0-0";
            }
        }

        return string.Empty;
    }
}

