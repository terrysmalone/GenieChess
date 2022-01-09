using System;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessEngineTests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void TestWriteBoardToConsole()
        {
            var board = new Board();

            board.InitaliseStartingPosition();
            board.WriteBoardToConsole();
        }

        [STAThread]
        [TestMethod]
        public void TestInitaliseStartingPosition()
        {
            var board = new Board();
            
            board.InitaliseStartingPosition();
            board.CalculateUsefulBitboards();

            var allWhite = board.AllWhiteOccupiedSquares;
            var allBlack = board.AllBlackOccupiedSquares;
            var allPieces = board.AllOccupiedSquares;            

            //Random checks                        
            Assert.AreEqual((ulong)512, allPieces & LookupTables.B2);   //Checks that one of the pieces are on B1            
            Assert.AreEqual((ulong)512, allWhite & LookupTables.B2);   //Checks that one of the white pieces is on B1

            Assert.AreEqual((ulong)0, allBlack & LookupTables.B2);                       //Checks that one of the black pieces is not on B1
            Assert.AreNotEqual((ulong)512, allBlack & LookupTables.B2);            //...or 

            Assert.AreEqual((ulong)512, board.WhitePawns & LookupTables.B2);       //Checks that one of the white pawns pieces is not on B2    
        }

        /// <summary>
        /// Tests the values of all piece bitboards for a starting position using pre calculated values
        /// </summary>
        [TestMethod]
        public void TestPieceBitboardsInInitialPosition()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            Assert.AreEqual((ulong)65280, board.WhitePawns);
            Assert.AreEqual((ulong)66, board.WhiteKnights);
            Assert.AreEqual((ulong)36, board.WhiteBishops);
            Assert.AreEqual((ulong)129, board.WhiteRooks);
            Assert.AreEqual((ulong)8, board.WhiteQueen);
            Assert.AreEqual((ulong)16, board.WhiteKing);

            Assert.AreEqual((ulong)71776119061217280, board.BlackPawns);
            Assert.AreEqual((ulong)4755801206503243776, board.BlackKnights);
            Assert.AreEqual((ulong)2594073385365405696, board.BlackBishops);
            Assert.AreEqual((ulong)9295429630892703744, board.BlackRooks);
            Assert.AreEqual((ulong)576460752303423488, board.BlackQueen);
            Assert.AreEqual((ulong)1152921504606846976, board.BlackKing);
        }

        /// <summary>
        /// Tests the values of the useful bitboards for a starting position using pre calculated values
        /// </summary>
        [TestMethod]
        public void TestUsefulBitboardsInInitialPosition()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            board.CalculateUsefulBitboards();

            var allWhite = board.AllWhiteOccupiedSquares;
            const ulong expectedAllWhite = 65535;
            Assert.AreEqual(expectedAllWhite, allWhite);

            var allBlack = board.AllBlackOccupiedSquares;
            const ulong expectedAllBlack = 18446462598732840960;
            Assert.AreEqual(expectedAllBlack, allBlack);

            var allPieces = board.AllOccupiedSquares;
            const ulong expectedAll = 18446462598732906495;
            Assert.AreEqual(expectedAll, allPieces);

            var emptySquares = board.EmptySquares;
            const ulong expectedEmpty = 281474976645120;
            Assert.AreEqual(expectedEmpty, emptySquares);

            var whiteOrEmpty = board.WhiteOrEmpty;
            const ulong expectedWhiteOrEmpty = 281474976710655;
            Assert.AreEqual(expectedWhiteOrEmpty, whiteOrEmpty);

            var blackOrEmpty = board.BlackOrEmpty;
            const ulong expectedBlackOrEmpty = 18446744073709486080;
            Assert.AreEqual(expectedBlackOrEmpty, blackOrEmpty);
        }

        private void CheckAllPiecesArePresent(Board board, PieceType type, bool isWhite)
        {
            var compareBoard = new Board();
            compareBoard.InitaliseStartingPosition();

            ulong comparePieceBoard = 0;
            ulong pieceBoard = 0;

            if(isWhite)
            {
                switch(type)
                {
                    case PieceType.Pawn:
                        comparePieceBoard = compareBoard.WhitePawns;
                        pieceBoard = board.WhitePawns;
                        break;
                    case PieceType.Knight:
                        comparePieceBoard = compareBoard.WhiteKnights;
                        pieceBoard = board.WhiteKnights;
                        break;
                    case PieceType.Bishop:
                        comparePieceBoard = compareBoard.WhiteBishops;
                        pieceBoard = board.WhiteBishops;
                        break;
                    case PieceType.Rook:
                        comparePieceBoard = compareBoard.WhiteRooks;
                        pieceBoard = board.WhiteRooks;
                        break;
                    case PieceType.Queen:
                        comparePieceBoard = compareBoard.WhiteQueen;
                        pieceBoard = board.WhiteQueen;
                        break;
                    case PieceType.King:
                        comparePieceBoard = compareBoard.WhiteKing;
                        pieceBoard = board.WhiteKing;
                        break;
                }
            }
            else
            {
                switch(type)
                {
                    case PieceType.Pawn:
                        comparePieceBoard = compareBoard.BlackPawns;
                        pieceBoard = board.BlackPawns;
                        break;
                    case PieceType.Knight:
                        comparePieceBoard = compareBoard.BlackKnights;
                        pieceBoard = board.BlackKnights;
                        break;
                    case PieceType.Bishop:
                        comparePieceBoard = compareBoard.BlackBishops;
                        pieceBoard = board.BlackBishops;
                        break;
                    case PieceType.Rook:
                        comparePieceBoard = compareBoard.BlackRooks;
                        pieceBoard = board.BlackRooks;
                        break;
                    case PieceType.Queen:
                        comparePieceBoard = compareBoard.BlackQueen;
                        pieceBoard = board.BlackQueen;
                        break;
                    case PieceType.King:
                        comparePieceBoard = compareBoard.BlackKing;
                        pieceBoard = board.BlackKing;
                        break;
                }
            }

            Assert.AreEqual(comparePieceBoard, pieceBoard);
        }

        [TestMethod]
        public void TestSetFenSetFENPosition_Initial()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));
             
            //Check piece positions
            Assert.AreEqual((ulong)65280, board.WhitePawns);
            Assert.AreEqual((ulong)66, board.WhiteKnights);
            Assert.AreEqual((ulong)36, board.WhiteBishops);
            Assert.AreEqual((ulong)129, board.WhiteRooks);
            Assert.AreEqual((ulong)8, board.WhiteQueen);
            Assert.AreEqual((ulong)16, board.WhiteKing);

            Assert.AreEqual((ulong)71776119061217280, board.BlackPawns);
            Assert.AreEqual((ulong)4755801206503243776, board.BlackKnights);
            Assert.AreEqual((ulong)2594073385365405696, board.BlackBishops);
            Assert.AreEqual((ulong)9295429630892703744, board.BlackRooks);
            Assert.AreEqual((ulong)576460752303423488, board.BlackQueen);
            Assert.AreEqual((ulong)1152921504606846976, board.BlackKing);

            Assert.IsTrue(board.WhiteToMove);

            Assert.IsTrue(board.BlackCanCastleKingside);
            Assert.IsTrue(board.BlackCanCastleQueenside);
            Assert.IsTrue(board.WhiteCanCastleKingside);
            Assert.IsTrue(board.WhiteCanCastleQueenside);
        }

        [TestMethod]
        public void TestSetFenNotation_position_2()
        {
            //Board board = new Board();

            //board.PlacePiece(PieceType.Pawn, false, 0, 6);
            //board.PlacePiece(PieceType.Pawn, true, 1, 4);
            //board.PlacePiece(PieceType.King, true, 0, 3);
            //board.PlacePiece(PieceType.King, false, 2, 3);
            //board.PlacePiece(PieceType.Pawn, false, 6, 3);
            //board.PlacePiece(PieceType.Pawn, true, 6, 2);
            //board.PlacePiece(PieceType.Pawn, true, 7, 1);

            ////flags changed
            //board.BlackCanCastleKingside = false;
            //board.WhiteCanCastleKingside = false;
            //board.BlackCanCastleQueenside = false;
            //board.WhiteCanCastleQueenside = false;

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -"));

            Assert.AreEqual((ulong)8594161664, board.WhitePawns);
            Assert.AreEqual((ulong)0, board.WhiteKnights);
            Assert.AreEqual((ulong)0, board.WhiteBishops);
            Assert.AreEqual((ulong)0, board.WhiteRooks);
            Assert.AreEqual((ulong)0, board.WhiteQueen);
            Assert.AreEqual((ulong)16777216, board.WhiteKing);

            Assert.AreEqual((ulong)281476050452480, board.BlackPawns);
            Assert.AreEqual((ulong)0, board.BlackKnights);
            Assert.AreEqual((ulong)0, board.BlackBishops);
            Assert.AreEqual((ulong)0, board.BlackRooks);
            Assert.AreEqual((ulong)0, board.BlackQueen);
            Assert.AreEqual((ulong)67108864, board.BlackKing);

            Assert.IsTrue(board.WhiteToMove);

            Assert.IsFalse(board.WhiteCanCastleKingside);
            Assert.IsFalse(board.WhiteCanCastleQueenside);
            Assert.IsFalse(board.BlackCanCastleKingside);
            Assert.IsFalse(board.BlackCanCastleQueenside);

            Assert.AreEqual((ulong)0, board.EnPassantPosition);
        }

        [TestMethod]
        public void TestClearBoard()
        {
            var board = new Board();

            board.InitaliseStartingPosition();

            Assert.AreNotEqual((ulong)0, board.AllOccupiedSquares);
            Assert.AreNotEqual((ulong)0, board.AllWhiteOccupiedSquares);
            Assert.AreNotEqual((ulong)0, board.AllBlackOccupiedSquares);
            Assert.AreNotEqual((ulong)18446744073709551615, board.BlackOrEmpty);
            Assert.AreNotEqual((ulong)18446744073709551615, board.WhiteOrEmpty);
            Assert.AreNotEqual((ulong)18446744073709551615, board.EmptySquares);

            Assert.AreNotEqual((ulong)0, board.WhitePawns);
            Assert.AreNotEqual((ulong)0, board.WhiteQueen);
            Assert.AreNotEqual((ulong)0, board.WhiteKing);
            Assert.AreNotEqual((ulong)0, board.WhiteBishops);
            Assert.AreNotEqual((ulong)0, board.WhiteKnights);
            Assert.AreNotEqual((ulong)0, board.WhiteRooks);

            Assert.AreNotEqual((ulong)0, board.BlackPawns);
            Assert.AreNotEqual((ulong)0, board.BlackQueen);
            Assert.AreNotEqual((ulong)0, board.BlackKing);
            Assert.AreNotEqual((ulong)0, board.BlackBishops);
            Assert.AreNotEqual((ulong)0, board.BlackKnights);
            Assert.AreNotEqual((ulong)0, board.BlackRooks);
            
            board.ClearBoard();

            Assert.AreEqual((ulong)0, board.AllOccupiedSquares);
            Assert.AreEqual((ulong)0, board.AllWhiteOccupiedSquares);
            Assert.AreEqual((ulong)0, board.AllBlackOccupiedSquares);
            Assert.AreEqual((ulong)18446744073709551615, board.BlackOrEmpty);
            Assert.AreEqual((ulong)18446744073709551615, board.WhiteOrEmpty);
            Assert.AreEqual((ulong)18446744073709551615, board.EmptySquares);

            Assert.AreEqual((ulong)0, board.WhitePawns);
            Assert.AreEqual((ulong)0, board.WhiteQueen);
            Assert.AreEqual((ulong)0, board.WhiteKing);
            Assert.AreEqual((ulong)0, board.WhiteBishops);
            Assert.AreEqual((ulong)0, board.WhiteKnights);
            Assert.AreEqual((ulong)0, board.WhiteRooks);

            Assert.AreEqual((ulong)0, board.BlackPawns);
            Assert.AreEqual((ulong)0, board.BlackQueen);
            Assert.AreEqual((ulong)0, board.BlackKing);
            Assert.AreEqual((ulong)0, board.BlackBishops);
            Assert.AreEqual((ulong)0, board.BlackKnights);
            Assert.AreEqual((ulong)0, board.BlackRooks);

        }

        [TestMethod]
        public void TestMoveColour()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            Assert.AreEqual(true, board.WhiteToMove);

            board.SwitchSides();
            Assert.AreEqual(false, board.WhiteToMove);


        }

        [TestMethod]
        public void TestIsKingInCheckFlags()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        //#region Zobrist tests

        //[TestMethod]
        //public void TestZobrist_Move_Normal()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestZobrist_Castle()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestZobrist_Move_Capture()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestZobrist_Move_Promotion()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestZobrist_Move_PromotionCapture()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestZobrist_Move_EnPassant()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestZobrist_Move_DoublePawnPush()
        //{
        //    throw new NotImplementedException();
        //    //Game game = new Game(@"C:\Users\Terry\Documents\Programming\C#\ChessEngine\ChessBoardTests\Resources\ScoreValues.txt");
        //    //game.InitaliseStartingPosition();

        //    //ulong initialZobrist = game.CurrentBoard.Zobrist;

        //    ////e2-e4
        //    //game.CurrentBoard.MakeMove(new PieceMove { Position = LookupTables.E2, Moves = LookupTables.E4, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn }, true);
        //    ////game.ReceiveUCIMove("e2e4");

        //    //ulong move1Zobrist = game.CurrentBoard.Zobrist;

        //    //game.CurrentBoard.UnMakeLastMove();
            
        //    //ulong unMakeMove1Zobrist = game.CurrentBoard.Zobrist;

        //    //Assert.AreNotEqual(initialZobrist, move1Zobrist);
        //    //Assert.AreNotEqual(move1Zobrist, unMakeMove1Zobrist);
        //    //Assert.AreEqual(initialZobrist, unMakeMove1Zobrist);
        //}

        //#endregion Zobrist tests

    }
}
