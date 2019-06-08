﻿using BitboardVisualiser;
using ChessEngine.BoardRepresentation;

namespace BitboardVisualiserTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var board = new Board();

            board.InitaliseStartingPosition();

            DebuggerChessBoard.TestShowVisualizer(board);
        }
    }
}
