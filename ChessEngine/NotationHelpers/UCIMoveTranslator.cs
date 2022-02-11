using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngine.NotationHelpers
{
    /// <summary>
    /// Translates to and from UCI moves
    /// </summary>
    public static class UciMoveTranslator
    {
        /// <summary>
        /// Converts a move into the appropriate UCI string
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static string ToUciMove(PieceMove move)
        {
            var moveString = string.Empty;

            var moveFromBoard = move.Position;
            var moveToBoard = move.Moves;

            moveString = TranslationHelper.GetSquareNotation(moveFromBoard);
            moveString += TranslationHelper.GetSquareNotation(moveToBoard);

            moveString += CheckForPromotionString(move.SpecialMove);

            return moveString;
        }

        private static string CheckForPromotionString(SpecialMoveType specialMoveType)
        {
            switch (specialMoveType)
            {
                case (SpecialMoveType.BishopPromotion):
                    return "b";
                case (SpecialMoveType.BishopPromotionCapture):
                    return "b";
                case (SpecialMoveType.KnightPromotion):
                    return "n";
                case (SpecialMoveType.KnightPromotionCapture):
                    return "n";
                case (SpecialMoveType.RookPromotion):
                    return "r";
                case (SpecialMoveType.RookPromotionCapture):
                    return "r";
                case (SpecialMoveType.QueenPromotion):
                    return "q";
                case (SpecialMoveType.QueenPromotionCapture):
                    return "q";
                default:
                    return string.Empty;
            }
        }

        // Translates a UCI string move to a game PieceMove
        public static PieceMove ToGameMove(string move, Board board)
        {            
            var pieceMove = new PieceMove();

            //split string into pairs
            var moveFrom = move.Substring(0, 2);
            var moveTo = move.Substring(2, 2);

            var MoveFromPosition = TranslationHelper.GetBitboard(moveFrom);
            var MoveToPosition = TranslationHelper.GetBitboard(moveTo);

            pieceMove.Position = MoveFromPosition;
            pieceMove.Moves = MoveToPosition;

            pieceMove.Type = BoardChecking.GetPieceTypeOnSquare(board, MoveFromPosition);

            pieceMove.SpecialMove = BoardChecking.GetSpecialMoveType(board, MoveFromPosition, MoveToPosition, move);

            return pieceMove;
        }
    }
}
