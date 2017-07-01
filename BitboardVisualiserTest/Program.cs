using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitboardVisualiser;
using ChessGame;
using ChessGame.BoardRepresentation;

namespace BitboardVisualiserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //UInt64 square = 768;
            //DebuggerBitboard.TestShowVisualizer(square);

            Board board = new Board();

            board.InitaliseStartingPosition();

            DebuggerChessBoard.TestShowVisualizer(board);
        }
    }
}
