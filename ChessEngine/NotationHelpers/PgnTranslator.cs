using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;

namespace ChessEngine.NotationHelpers
{
    /// <summary>
    /// Translates between Board and a PGN string
    /// </summary>
    public static class PgnTranslator
    {
        // Creates a PGN move string from a board move
        public static string ToPgnMove(Board board, ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
            var moveIsCapture = false;

            var move = string.Empty;

            var movedPiece = BoardChecking.GetPieceTypeOnSquare(board, moveFromBoard);    //Can#t trust pieceToMove since it may have changed (i.e. promotion)

            move = CheckForCastling(board, moveFromBoard, moveToBoard, movedPiece);

            if (!string.IsNullOrEmpty(move))
            {
                move = string.Copy(move);

                return move;
            }
            else
            {
                //Get string of piece to move 
                move += TranslationHelper.GetPieceLetter(movedPiece, moveFromBoard);

                var capturedPiece = PieceType.None; 

                //Is it a capture, if so use 'x'
                if (BoardChecking.IsEnemyPieceOnSquare(board, moveToBoard))
                {
                    moveIsCapture = true;
                    move += "x";

                    capturedPiece = BoardChecking.GetPieceTypeOnSquare(board, moveToBoard); 
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

                //Is it a promotion, if so add "=Q"
                if (movedPiece != pieceToMove)
                    move += "=" + TranslationHelper.GetPieceLetter(pieceToMove, moveToBoard);

                //Move piece to test for check  

                var pieceMover = new PieceMover(board);
                
                //board.RemovePiece(moveFromBoard);
                //board.PlacePiece(pieceToMove, movingColour, moveToBoard); 
                pieceMover.MakeMove(moveFromBoard, moveToBoard, pieceToMove, PossibleMoves.SpecialMoveType.Normal, false);
                
                if (BoardChecking.IsKingInCheck(board, board.WhiteToMove))
                    move += "+";
                
                pieceMover.UnMakeLastMove();
                
                //board.PlacePiece(capturedPiece, colour, moveToBoard);
                //board.PlacePiece(movedPiece, movingColour, moveFromBoard); 
                
                //If mate add score

                Debug.Assert(!string.IsNullOrEmpty(move));

            }

            return move;
        }
        
        private static string CheckForCastling(Board board, ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
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

        private static void CheckForPawnPromotion(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
           //throw new NotImplementedException("Pawn promotion not impletemented");
        }
    }
}
