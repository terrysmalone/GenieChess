using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.BoardSearching;
using ChessGame.PossibleMoves;

namespace ChessGame.NotationHelpers
{
    /// <summary>
    /// Translates to and from UCI moves
    /// </summary>
    public static class UCIMoveTranslator
    {
        /// <summary>
        /// Converts a move into the appropriate UCI string
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static string ToUCIMove(PieceMoves move)
        {
            string moveString = string.Empty;

            ulong moveFromBoard = move.Position;
            ulong moveToBoard = move.Moves;

            moveString = TranslationHelper.SquareBitboardToSquareString(moveFromBoard);
            moveString += TranslationHelper.SquareBitboardToSquareString(moveToBoard);

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

        /// <summary>
        /// Translates a UCI string move to a game PieceMove
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static PieceMoves ToGameMove(string move, Board board)
        {            
            PieceMoves pieceMove = new PieceMoves();

            //split string into pairs
            string moveFrom = move.Substring(0, 2);
            string moveTo = move.Substring(2, 2);

            ulong MoveFromPosition = TranslationHelper.BitboardFromSquareString(moveFrom);
            ulong MoveToPosition = TranslationHelper.BitboardFromSquareString(moveTo);

            pieceMove.Position = MoveFromPosition;
            pieceMove.Moves = MoveToPosition;

            pieceMove.Type = BoardChecking.GetPieceTypeOnSquare(board, MoveFromPosition);

            pieceMove.SpecialMove = BoardChecking.GetSpecialMoveType(board, MoveFromPosition, MoveToPosition, pieceMove.Type, move);

            return pieceMove;
        }
    }
}
