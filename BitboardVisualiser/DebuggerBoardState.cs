using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.DebuggerVisualizers;
using ChessGame.BoardRepresentation;
using ChessGame.Enums;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(BitboardVisualiser.DebuggerBoardState),
typeof(VisualizerObjectSource),
Target = typeof(BoardState),
Description = "Displays the contents of a BoardPosition on an 8x8 grid")]

namespace BitboardVisualiser
{
    public class DebuggerBoardState : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            BoardState board = (BoardState)objectProvider.GetObject();
            
            ChessBoardView chessBoardView = new ChessBoardView();

            AddPieces(board, chessBoardView);

            windowService.ShowDialog(chessBoardView);


            //string board = BoardToString(chessBoard);

            //MessageBox.Show(board);

            // MessageBox.Show(objectProvider.GetObject().ToString());
        }

        private void AddPieces(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            AddPawns(chessBoard, chessBoardView);
            AddKnights(chessBoard, chessBoardView);
            AddBishops(chessBoard, chessBoardView);
            AddRooks(chessBoard, chessBoardView);
            AddQueens(chessBoard, chessBoardView);
            AddKings(chessBoard, chessBoardView);
        }

        private void AddPawns(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            List<byte> locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.WhitePawns);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Pawn, PieceColour.White, location);
            }

            locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.BlackPawns);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Pawn, PieceColour.Black, location);
            }
        }

        private void AddKnights(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            List<byte> locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.WhiteKnights);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Knight, PieceColour.White, location);
            }

            locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.BlackKnights);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Knight, PieceColour.Black, location);
            }
        }

        private void AddBishops(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            List<byte> locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.WhiteBishops);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Bishop, PieceColour.White, location);
            }

            locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.BlackBishops);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Bishop, PieceColour.Black, location);
            }
        }

        private void AddRooks(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            List<byte> locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.WhiteRooks);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Rook, PieceColour.White, location);
            }

            locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.BlackRooks);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Rook, PieceColour.Black, location);
            }
        }

        private void AddQueens(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            List<byte> locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.WhiteQueen);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Queen, PieceColour.White, location);
            }

            locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.BlackQueen);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.Queen, PieceColour.Black, location);
            }
        }

        private void AddKings(BoardState chessBoard, ChessBoardView chessBoardView)
        {
            List<byte> locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.WhiteKing);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.King, PieceColour.White, location);
            }

            locations = BitboardOperations.GetSquareIndexesFromBoardValue(chessBoard.BlackKing);
            foreach (byte location in locations)
            {
                chessBoardView.AddPiece(PieceType.King, PieceColour.Black, location);
            }
        }

        public string BoardToString(BoardState chessBoard)
        {
            string board = "";

            char[] squares = new char[64];

            AddPieceLetterToSquares(squares, chessBoard.WhitePawns, 'p');
            AddPieceLetterToSquares(squares, chessBoard.BlackPawns, 'P');

            AddPieceLetterToSquares(squares, chessBoard.WhiteKnights, 'n');
            AddPieceLetterToSquares(squares, chessBoard.BlackKnights, 'N');

            AddPieceLetterToSquares(squares, chessBoard.WhiteBishops, 'b');
            AddPieceLetterToSquares(squares, chessBoard.BlackBishops, 'B');

            AddPieceLetterToSquares(squares, chessBoard.WhiteRooks, 'r');
            AddPieceLetterToSquares(squares, chessBoard.BlackRooks, 'R');

            AddPieceLetterToSquares(squares, chessBoard.WhiteQueen, 'q');
            AddPieceLetterToSquares(squares, chessBoard.BlackQueen, 'Q');

            AddPieceLetterToSquares(squares, chessBoard.WhiteKing, 'k');
            AddPieceLetterToSquares(squares, chessBoard.BlackKing, 'K');

            for (int rank = 7; rank >= 0; rank--)
            {
                board += "\n";
                board += "|";

                for (int file = 0; file < 8; file++)
                {
                    int index = rank * 8 + file;

                    if (char.IsLetter(squares[index]))
                    {
                        board += " ";
                        board += squares[index];
                        board += " ";
                    }
                    else
                        board += " _ ";
                    
                    board += "|";
                }
            }

            board += "";
            board += "\n";
            //board += "-------------------------";

            return board;
        }

        private void AddPieceLetterToSquares(char[] squares, UInt64 piecePosition, char letterToAdd)
        {
            List<byte> pieceSquares = BitboardOperations.GetSquareIndexesFromBoardValue(piecePosition);

            for (int i = 0; i < pieceSquares.Count; i++)
            {
                squares[pieceSquares[i]] = letterToAdd;
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(DebuggerChessBoard));
            visualizerHost.ShowVisualizer();
        }
    }
}
