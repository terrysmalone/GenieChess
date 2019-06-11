﻿using System;
using System.CodeDom;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using ResourceLoading;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ChessEngineTests
{
    [TestClass]
    public class MoveGenerationTests
    {
        [Test]
        public void PseudoLegalMoveGenerationWhiteCantCastleWhileInCheck()
        {
            var board = new Board();
            board.PlacePiece(PieceType.King, PieceColour.White, 4, 0);
            board.PlacePiece(PieceType.King, PieceColour.Black, 4, 7);

            board.PlacePiece(PieceType.Rook, PieceColour.White, 0, 0);

            board.PlacePiece(PieceType.Rook, PieceColour.Black, 4, 6);

            board.WhiteCanCastleQueenside = true;

            var moveList = MoveGeneration.CalculateAllPseudoLegalMoves(board);

            //Remove checking moves
            for (var i = moveList.Count-1; i >= 0; i--)
            {
                var currentMove = moveList[i];
                board.MakeMove(currentMove, false);
                if (BoardChecking.IsKingInCheck(board, PieceColour.White))
                {
                    moveList.RemoveAt(i);
                }
                board.UnMakeLastMove();
            }

            // remove invalid castling moves
            for (var i = moveList.Count-1; i >= 0; i--)
            {
                var currentMove = moveList[i];
                if (currentMove.SpecialMove == SpecialMoveType.KingCastle || currentMove.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    // If king is in check he can't move
                    if (BoardChecking.IsKingInCheck(board, PieceColour.White))
                    {
                        moveList.RemoveAt(i);
                    }

                    if (!MoveGeneration.ValidateCastlingMove(board, currentMove))
                    {
                        moveList.RemoveAt(i);
                    }
                }
            }

            Assert.Fail();
        }

        #region move calculation tests
        
        /// <summary>
        /// Checks that pawns can't move when they are blocked
        /// </summary>
        [TestMethod]
        public void TestPawnMoves()
        {
            var board = new Board();
            board.PlacePiece(PieceType.Pawn, PieceColour.White, 1, 1);
            board.PlacePiece(PieceType.Pawn, PieceColour.Black, 1, 2);

            board.PlacePiece(PieceType.King, PieceColour.White, 1, 6);
            board.PlacePiece(PieceType.King, PieceColour.Black, 5, 6);
            
            board.WhiteCanCastleKingside = false;
            board.WhiteCanCastleQueenside = false;
            board.BlackCanCastleQueenside = false;
            board.BlackCanCastleKingside = false;

            var moveList = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(8, moveList.Count);

        }

        //[TestMethod]
        //public void TestKnightMoves()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestBishopMoves()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestRookMoves()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestQueenMoves()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestKingMoves()
        //{
        //    throw new NotImplementedException();
        //}

        [TestMethod]
        public void TestEnPassantCaptureBlack()
        {
            var board = new Board();

            board.PlacePiece(PieceType.King, PieceColour.White, 0, 0);
            board.PlacePiece(PieceType.King, PieceColour.Black, 0, 7);

            board.PlacePiece(PieceType.Pawn, PieceColour.White, 7, 1);
            board.PlacePiece(PieceType.Pawn, PieceColour.Black, 6, 3);

            //flags changed
            board.BlackCanCastleKingside = false;
            board.WhiteCanCastleKingside = false;
            board.BlackCanCastleQueenside = false;
            board.WhiteCanCastleQueenside = false;

            board.MakeMove(32768, 2147483648, PieceType.Pawn, SpecialMoveType.DoublePawnPush, false);       //Move h pawn 2 spaces

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(5, allMoves.Count);
        }

        [TestMethod]
        public void TestEnPassantCaptureWhite()
        {
            var board = new Board();

            board.PlacePiece(PieceType.King, PieceColour.White, 4, 3); 
            board.PlacePiece(PieceType.King, PieceColour.Black, 0, 7);

            board.PlacePiece(PieceType.Pawn, PieceColour.White, 6, 4);
            board.PlacePiece(PieceType.Pawn, PieceColour.Black, 5, 6);

            //flags changed
            board.BlackCanCastleKingside = false;
            board.WhiteCanCastleKingside = false;
            board.BlackCanCastleQueenside = false;
            board.WhiteCanCastleQueenside = false;

            board.SwitchSides();

            board.MakeMove(9007199254740992, 137438953472, PieceType.Pawn, SpecialMoveType.DoublePawnPush, false);       //Move black f-pawn 2 spaces

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(9, allMoves.Count);
        }

        //[TestMethod]
        //public void TestCastling()
        //{
        //    throw new NotImplementedException();

        //    //Test blocked pieces

        //    //Test attacked path

        //    //Test can castle
        //}

        #region Check tests

        [TestMethod]
        public void TestDoubleCheck()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/4r2q/8/2B4R/2N1KP2/r7/8 w - - 0 1"));

            var allMoves = MoveGeneration.CalculateAllMoves(board);
            
            Assert.AreEqual(2, allMoves.Count);

            var move1 = allMoves[0];
            Assert.AreEqual(PieceType.King, move1.Type);
            Assert.AreEqual((ulong)1048576, move1.Position);
            Assert.AreEqual((ulong)524288, move1.Moves);

            var move2 = allMoves[1];
            Assert.AreEqual(PieceType.King, move2.Type);
            Assert.AreEqual((ulong)1048576, move2.Position);
            Assert.AreEqual((ulong)134217728, move2.Moves); 
        }

        #region Test blocking checks

        //[TestMethod]
        //public void TestCheckBlockingWorks()
        //{
        //    throw new NotImplementedException();

        //    //Test that ray attacks that are blocked do not check king
        //}

        [TestMethod]
        public void TestBlockingPieceCantMove1()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("b4k2/8/8/8/8/8/6P1/7K w - - 0 1"));

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(2, allMoves.Count);

            var move1 = allMoves[0];
            Assert.AreEqual(PieceType.King, move1.Type);
            Assert.AreEqual((ulong)128, move1.Position);
            Assert.AreEqual((ulong)64, move1.Moves);

            var move2 = allMoves[1];
            Assert.AreEqual(PieceType.King, move2.Type);
            Assert.AreEqual((ulong)128, move2.Position);
            Assert.AreEqual((ulong)32768, move2.Moves);        
        }

        [TestMethod]
        public void TestBlockingPieceCantMove2()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("5k2/8/4q3/8/8/8/4N3/4K3 w - - 0 1"));

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(4, allMoves.Count);

            foreach (var pieceMove in allMoves)
            {
                Assert.AreEqual(PieceType.King, pieceMove.Type);
                Assert.AreEqual((ulong)16, pieceMove.Position);
            }
        }

        #endregion Test blocking checks

        #region Check wrap tests

        [TestMethod]
        public void TestCheckWrapping()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("5b2/8/8/8/1P5b/KP5r/1P5q/8 w - - 0 1"));

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(2, allMoves.Count);

            //PieceMoves move1 = allMoves[0];
            //Assert.AreEqual(PieceType.King, move1.Type);
            //Assert.AreEqual((ulong)1048576, move1.Position);
            //Assert.AreEqual((ulong)524288, move1.Moves);

            //PieceMoves move2 = allMoves[1];
            //Assert.AreEqual(PieceType.King, move2.Type);
            //Assert.AreEqual((ulong)1048576, move2.Position);
            //Assert.AreEqual((ulong)134217728, move2.Moves);
        }

        #endregion Check wrap tests

        #endregion Check tests

        #endregion move calculation tests

        #region speed tests

        /// <summary>
        /// Tests the move generator speed by running a number of perft tests
        /// </summary>
        [TestMethod]
        public void TestMoveGenerationSpeed()
        { 
            
            


        }

        private TimeSpan GetRunningTime(string boardPosition, int depth)
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(boardPosition));

            var startTime = DateTime.Now;
            var perft = new PerfT();

            perft.Perft(board, depth);

            var endTime = DateTime.Now;

            var time = endTime - startTime;

            return time;
        }

        #endregion speed tests

    }
}
