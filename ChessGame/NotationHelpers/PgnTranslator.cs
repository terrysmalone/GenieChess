using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.BoardSearching;

namespace ChessGame.NotationHelpers
{
    /// <summary>
    /// Translates between Board and a PGN string
    /// </summary>
    public static class PgnTranslator
    {
        /// <summary>
        /// Creates a board from an entire PGN list
        /// </summary>
        /// <param name="board"></param>
        /// <param name="pgnString"></param>
        public static void ToBoard(Board board, string pgnString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an entire PGN string from a board 
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static string ToPgnMovesList(Board board)
        {
            string pgnString = string.Empty;
                        
            if (!DidGameStartFromInitialPosition(board))
            {
                string fenNotation = FenTranslator.ToFENString(board.History[0]);

                pgnString += "[FEN \"" + fenNotation + "\"]\n";
            }

            pgnString += "\n";

            List<BoardState> boardStates = board.History;

            bool whiteStarted = true;

            if (boardStates[0].HalfMoveClock % 2 != 0)
                whiteStarted = false;


                        
            //for (int i = 1; i < boardStates.Count; i++)
            //{
            //    string line = boardStates[i].FullMoveClock + ".";

            //    if (whiteStarted == false)
            //    {
            //        //draw 3 ellipses as move
            //        line += "..";
            //        whiteStarted = true;    //We only need the ellipses once
            //    }


            //    throw new NotImplementedException();
            //}

            return pgnString;
        }

        private static bool DidGameStartFromInitialPosition(Board board)
        {
            BoardState initialPosition = board.History[0];

            Board startBoard = new Board();
            startBoard.InitaliseStartingPosition();
            BoardState startState = startBoard.History[0];

            if (initialPosition.Equals(startState))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a PGN move string from a board move
        /// </summary>
        /// <param name="moveFromBoard"></param>
        /// <param name="moveToBoard"></param>
        /// <param name="pieceToMove"></param>
        /// <returns></returns>
        public static string ToPgnMove(Board board, ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
            bool moveIsCapture = false;

            string move = string.Empty;

            PieceType movedPiece = BoardChecking.GetPieceTypeOnSquare(board, moveFromBoard);    //Can#t trust pieceToMove since it may have changed (i.e. promotion)

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

                PieceType capturedPiece = PieceType.None; 

                //Is it a capture, if so use 'x'
                if (BoardChecking.IsEnemyPieceOnSquare(board, moveToBoard))
                {
                    moveIsCapture = true;
                    move += "x";

                    capturedPiece = BoardChecking.GetPieceTypeOnSquare(board, moveToBoard); 
                }

                //Get string of position moved to
                string moveTo = TranslationHelper.SquareBitboardToSquareString(moveToBoard);

#warning ToDo
                //If another piece can move here, specify which it was

                if (movedPiece == PieceType.Pawn && !moveIsCapture)
                {
                    //We need to trim the first letter since it is a pawn
                    moveTo = moveTo.Substring(1);
                }

                move += moveTo;

                //Is it a promotion, if so add "=Q"
                if (movedPiece != pieceToMove)
                    move += "=" + TranslationHelper.GetPieceLetter(pieceToMove, moveToBoard);

                //Check for check and mate "+"
                PieceColour movingColour;
                PieceColour colour;

                if (board.WhiteToMove)
                {
                    movingColour = PieceColour.White;
                    colour = PieceColour.Black;
                }
                else
                {
                    movingColour = PieceColour.Black;                    
                    colour = PieceColour.White;
                }
                
                //Move piece to test for check                
                
                //board.RemovePiece(moveFromBoard);
                //board.PlacePiece(pieceToMove, movingColour, moveToBoard); 
                board.MakeMove(moveFromBoard, moveToBoard, pieceToMove, PossibleMoves.SpecialMoveType.Normal, false);
                if (BoardChecking.IsKingInCheckFast(board, colour))
                    move += "+";
                
                board.UnMakeLastMove();
                
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
