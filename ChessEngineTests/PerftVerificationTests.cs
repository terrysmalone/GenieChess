using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessEngineTests
{
    [TestClass]
    public class PerftVerificationTests
    {
        /// <summary>
        /// https://sites.google.com/site/numptychess/perft/position-1
        /// </summary>
        [TestMethod]
        public void TestPerftInitial()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

            var perft = new PerfT();
            
            Assert.AreEqual(20, perft.Perft(board, 1));
            Assert.AreEqual(400, perft.Perft(board, 2));
            Assert.AreEqual(8902, perft.Perft(board, 3));
            Assert.AreEqual(197281, perft.Perft(board, 4));
            Assert.AreEqual(4865609, perft.Perft(board, 5));
            //Assert.AreEqual(119060324, perft.Perft(board, 6));
        }

        /// <summary>
        /// https://sites.google.com/site/numptychess/perft/position-1
        /// </summary>
        [TestMethod]
        public void TestPerftInitial_WithHashing()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

            var perft = new PerfT();
            perft.UseHashing = true;

            //List<Tuple<string, ulong>> divides = perft.Divides(board);

            Assert.AreEqual(20, perft.Perft(board, 1));
            Assert.AreEqual(400, perft.Perft(board, 2));
            Assert.AreEqual(8902, perft.Perft(board, 3));
            Assert.AreEqual(197281, perft.Perft(board, 4));
            Assert.AreEqual(4865609, perft.Perft(board, 5));
            //Assert.AreEqual(119060324, perft.Perft(board, 6));
        }

        /// <summary>
        /// https://sites.google.com/site/numptychess/perft/position-2
        /// </summary>
        [TestMethod]
        public void TestPerft2()
        {
            var board = new Board();
            var perft = new PerfT();

            board.SetPosition(FenTranslator.ToBoardState("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -"));

            Assert.AreEqual(5, perft.Perft(board, 1));
            Assert.AreEqual(39, perft.Perft(board, 2));
            Assert.AreEqual(237, perft.Perft(board, 3));
            Assert.AreEqual(2002, perft.Perft(board, 4));
            Assert.AreEqual(14062, perft.Perft(board, 5));
            Assert.AreEqual(120995, perft.Perft(board, 6));
            Assert.AreEqual(966152, perft.Perft(board, 7));
            Assert.AreEqual(8103790, perft.Perft(board, 8));
        }
        
        /// <summary>
        /// https://sites.google.com/site/numptychess/perft/position-3
        /// </summary>
        [TestMethod]
        public void TestPerft3()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq -"));

            var perft = new PerfT();

            //var divides = perft.Divides(board);

            Assert.AreEqual(17, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual(341, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual(6666, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual(150072, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual(3186478, perft.Perft(board, 5), "Failed at 5");
            //Assert.AreEqual(77054993, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerft4()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -"));

            var perft = new PerfT();

            //var divides = perft.Divides(board);

            Assert.AreEqual(9, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual(85, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual(795, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual(7658, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual(72120, perft.Perft(board, 5), "Failed at 5");
            Assert.AreEqual(703851, perft.Perft(board, 6), "Failed at 6");
            //Assert.AreEqual(6627106, perft.Perft(board, 7));
        }

        /// <summary>
        /// https://sites.google.com/site/numptychess/perft/position-5
        /// </summary>
        [TestMethod]
        public void TestPerft5()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pb3pp1/5n1p/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R b KQkq -"));

            var perft = new PerfT();

            Assert.AreEqual(30, perft.Perft(board, 1));
            Assert.AreEqual(986, perft.Perft(board, 2));
            Assert.AreEqual(29777, perft.Perft(board, 3));
            Assert.AreEqual(967198, perft.Perft(board, 4));
            //Assert.AreEqual(29345534, perft.Perft(board, 5));
            //Assert.AreEqual(943244129, perft.Perft(board, 6));
        }

        /// <summary>
        /// "kiwipete"
        /// http://chessprogramming.wikispaces.com/Perft+Results
        /// </summary>
        [TestMethod]
        public void TestPerft6()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -"));

            var perft = new PerfT();

            Assert.AreEqual(48, perft.Perft(board, 1));
            Assert.AreEqual(2039, perft.Perft(board, 2));
            Assert.AreEqual(97862, perft.Perft(board, 3));
            Assert.AreEqual(4085603, perft.Perft(board, 4));
            //Assert.AreEqual(193690690, perft.Perft(board, 5));
        }

        /// <summary>
        /// "kiwipete" (above) failed at higher depth than I do in unit tests. This test plays 3 moves
        /// in from that, closer to the failed position to make sure it doesn't go unnoticed again
        /// It failed because of a bug in the en-passant zobrist hashing
        /// </summary>
        [TestMethod]
        public void TestPerft6Fail()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N1BQ1p/PPP1BPPP/R3K2R w KQkq c6 0 2"));

            //board.MakeMove(UciMoveTranslator.ToGameMove("d5c6", board), false);
            var perft = new PerfT();

            //var perftScore = perft.Perft(board, 3);

            Assert.AreEqual(50,        perft.Perft(board, 1));
            Assert.AreEqual(2028,      perft.Perft(board, 2));
            Assert.AreEqual(98109,     perft.Perft(board, 3));
            Assert.AreEqual(3901513,   perft.Perft(board, 4));
        }

        /// <summary>
        /// http://www.rocechess.ch/perft.html
        /// Good for checking promotions
        /// </summary>
        [TestMethod]
        public void TestPerft7()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(24, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual(496, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual(9483, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual(182838, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual(3605103, perft.Perft(board, 5), "Failed at 5");
            //Assert.AreEqual(71179139, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerft8()
        {
            var board = new Board();

            
            board.SetPosition(FenTranslator.ToBoardState("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1"));
                        
            var perft = new PerfT();
            
            Assert.AreEqual(14, perft.Perft(board, 1));
            Assert.AreEqual(191, perft.Perft(board, 2));
            Assert.AreEqual(2812, perft.Perft(board, 3));
            Assert.AreEqual(43238, perft.Perft(board, 4));
            Assert.AreEqual(674624, perft.Perft(board, 5));
            //Assert.AreEqual(11030083, perft.Perft(board, 6));
        }

        /// <summary>
        /// A contrived position where the kings are in the middle of the board and can be checked lots next move
        /// </summary>
        [TestMethod]
        public void TestPerft9()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("q7/2pp3b/8/3n1k1r/R1K1N3/8/B4PP1/7Q b - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(37, perft.Perft(board, 1));
            Assert.AreEqual(1109, perft.Perft(board, 2));
            Assert.AreEqual(35507, perft.Perft(board, 3));
            Assert.AreEqual(1064191, perft.Perft(board, 4));
            //Assert.AreEqual(33673031, perft.Perft(board, 5));       //116.87 seconds - 08/02/2014
        }

        /// <summary>
        /// http://www.chessprogramming.net/philosophy/perfect-perft/
        /// //--Illegal ep move #1
        /// </summary>
         [TestMethod]
        public void TestPerft10()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(18, perft.Perft(board, 1));
            Assert.AreEqual(92, perft.Perft(board, 2));
            Assert.AreEqual(1670, perft.Perft(board, 3));
            Assert.AreEqual(10138, perft.Perft(board, 4));
            Assert.AreEqual(185429, perft.Perft(board, 5));
            Assert.AreEqual(1134888, perft.Perft(board, 6));
        }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         /// //--Illegal ep move #2
         /// </summary>
         [TestMethod]
         public void TestPerft11()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("8/8/4k3/8/2p5/8/B2P2K1/8 w - - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(13, perft.Perft(board, 1));
             Assert.AreEqual(102, perft.Perft(board, 2));
             Assert.AreEqual(1266, perft.Perft(board, 3));
             Assert.AreEqual(10276, perft.Perft(board, 4));
             Assert.AreEqual(135655, perft.Perft(board, 5));
             Assert.AreEqual(1015133, perft.Perft(board, 6));
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--EP Capture Checks Opponent
         /// </summary>
         [TestMethod]
         public void TestPerft12()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(15, perft.Perft(board, 1));
             Assert.AreEqual(126, perft.Perft(board, 2));
             Assert.AreEqual(1928, perft.Perft(board, 3));
             Assert.AreEqual(13931, perft.Perft(board, 4));
             Assert.AreEqual(206379, perft.Perft(board, 5));
             Assert.AreEqual(1440467, perft.Perft(board, 6));
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Short Castling Gives Check
         /// </summary>
         [TestMethod]
         public void TestPerft13()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("5k2/8/8/8/8/8/8/4K2R w K - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(15, perft.Perft(board, 1));
             Assert.AreEqual(66, perft.Perft(board, 2));
             Assert.AreEqual(1198, perft.Perft(board, 3));
             Assert.AreEqual(6399, perft.Perft(board, 4));
             Assert.AreEqual(120330, perft.Perft(board, 5));
             Assert.AreEqual(661072, perft.Perft(board, 6));
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Long Castling Gives Check
         /// </summary>
         [TestMethod]
         public void TestPerft14()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("3k4/8/8/8/8/8/8/R3K3 w Q - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(16, perft.Perft(board, 1));
             Assert.AreEqual(71, perft.Perft(board, 2));
             Assert.AreEqual(1286, perft.Perft(board, 3));
             Assert.AreEqual(7418, perft.Perft(board, 4));
             Assert.AreEqual(141077, perft.Perft(board, 5));
             Assert.AreEqual(803711, perft.Perft(board, 6));
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Castle Rights
         /// </summary>
         [TestMethod]
         public void TestPerft15()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1"));

             //board.SetPosition(FenTranslator.ToBoardState("r3k2r/1b4bq/8/8/8/8/7B/2KR3R b kq - 0 1");
             var perft = new PerfT();

             //List<Tuple<string, ulong>> divides = perft.Divides(board);

             Assert.AreEqual(26, perft.Perft(board, 1));
             Assert.AreEqual(1141, perft.Perft(board, 2));
             Assert.AreEqual(27826, perft.Perft(board, 3));
             Assert.AreEqual(1274206, perft.Perft(board, 4));
             //Assert.AreEqual(31912360, perft.Perft(board, 5));
             //Assert.AreEqual(1509218880, perft.Perft(board, 6));            
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Castle Prevented
         /// </summary>
         [TestMethod]
         public void TestPerft16()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(44, perft.Perft(board, 1));
             Assert.AreEqual(1494, perft.Perft(board, 2));
             Assert.AreEqual(50509, perft.Perft(board, 3));
             Assert.AreEqual(1720476, perft.Perft(board, 4));
             //Assert.AreEqual(58773923, perft.Perft(board, 5));
             //Assert.AreEqual(2010267707, perft.Perft(board, 6));             
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Promote out of Check
         /// </summary>
         [TestMethod]
         public void TestPerft17()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(11, perft.Perft(board, 1));
             Assert.AreEqual(133, perft.Perft(board, 2));
             Assert.AreEqual(1442, perft.Perft(board, 3));
             Assert.AreEqual(19174, perft.Perft(board, 4));
             Assert.AreEqual(266199, perft.Perft(board, 5));
             Assert.AreEqual(3821001, perft.Perft(board, 6));             
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Discovered Check
         /// </summary>
         [TestMethod]
         public void TestPerft18()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(29, perft.Perft(board, 1));
             Assert.AreEqual(165, perft.Perft(board, 2));
             Assert.AreEqual(5160, perft.Perft(board, 3));
             Assert.AreEqual(31961, perft.Perft(board, 4));
             Assert.AreEqual(1004658, perft.Perft(board, 5));
            // Assert.AreEqual(6334638, perft.Perft(board, 6));
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Promote to give check
         /// </summary>
         [TestMethod]
         public void TestPerft19()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("4k3/1P6/8/8/8/8/K7/8 w - - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(9, perft.Perft(board, 1));
             Assert.AreEqual(40, perft.Perft(board, 2));
             Assert.AreEqual(472, perft.Perft(board, 3));
             Assert.AreEqual(2661, perft.Perft(board, 4));
             Assert.AreEqual(38983, perft.Perft(board, 5));
             Assert.AreEqual(217342, perft.Perft(board, 6));
         }

         /// <summary>
         /// http://www.chessprogramming.net/philosophy/perfect-perft/
         ///--Under Promote to give check
         /// </summary>
         [TestMethod]
         public void TestPerft20()
         {
             var board = new Board();

             board.SetPosition(FenTranslator.ToBoardState("8/P1k5/K7/8/8/8/8/8 w - - 0 1"));

             var perft = new PerfT();

             Assert.AreEqual(6, perft.Perft(board, 1), "Failed at 1");
             Assert.AreEqual(27, perft.Perft(board, 2), "Failed at 2");
             Assert.AreEqual(273, perft.Perft(board, 3), "Failed at 3");
             Assert.AreEqual(1329, perft.Perft(board, 4), "Failed at 4");
             Assert.AreEqual(18135, perft.Perft(board, 5), "Failed at 5");
             Assert.AreEqual(92683, perft.Perft(board, 6), "Failed at 6");
             Assert.AreEqual(1555980, perft.Perft(board, 7), "Failed at 7");
             //Assert.AreEqual(8110830, perft.Perft(board, 8));
         }

        #region Perft simple test

        [TestMethod]
        public void TestPerftCheckMate()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("k6r/8/8/8/8/5b2/4n1p1/7K w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(0, perft.Perft(board, 1));
            Assert.AreEqual(0, perft.Perft(board, 2));
        }

        [TestMethod]
        public void TestPerftStalemate()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("k6r/8/8/8/8/5n1b/7p/7K w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(0, perft.Perft(board, 1));
            Assert.AreEqual(0, perft.Perft(board, 2));
        }
          
        #endregion Perft simple test

        #region Individual tests

        [TestMethod]
        public void TestPerftJustKings()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/8/8/8/8/8/2K1k3 w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(3, perft.Perft(board, 1));
            Assert.AreEqual(13, perft.Perft(board, 2));
            Assert.AreEqual(77, perft.Perft(board, 3));
            Assert.AreEqual(484, perft.Perft(board, 4));
            Assert.AreEqual(2630, perft.Perft(board, 5));
            //Assert.AreEqual(16680, perft.Perft(board, 6));
        }


        [TestMethod]
        public void TestPerftKingsAndBishops()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("2b1B3/8/8/8/8/8/8/2K1k3 b - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(10, perft.Perft(board, 1));
            Assert.AreEqual(99, perft.Perft(board, 2));
            Assert.AreEqual(1162, perft.Perft(board, 3));
            Assert.AreEqual(13614, perft.Perft(board, 4));
            Assert.AreEqual(166917, perft.Perft(board, 5));
            //Assert.AreEqual(2038592, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerftKingsAndPawns()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("2k5/2p5/8/8/8/8/3P4/3K4 w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual(6, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual(36, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual(253, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual(1777, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual(13516, perft.Perft(board, 5), "Failed at 5");
            Assert.AreEqual(98271, perft.Perft(board, 6), "Failed at 6");
        }

        [TestMethod]
        public void TestPerftKingsAndKnights()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("2K2k2/8/8/8/8/8/8/2n2N2 w - -"));

            var perft = new PerfT();

            Assert.AreEqual(9, perft.Perft(board, 1));
            Assert.AreEqual(77, perft.Perft(board, 2));
            Assert.AreEqual(789, perft.Perft(board, 3));
            Assert.AreEqual(8001, perft.Perft(board, 4));
            Assert.AreEqual(85768, perft.Perft(board, 5));
            Assert.AreEqual(910419, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerftKingsAndRooks()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/8/8/8/8/2R2r2/1K3k2 w - -"));

            var perft = new PerfT();

            Assert.AreEqual(16, perft.Perft(board, 1));
            Assert.AreEqual(219, perft.Perft(board, 2));
            Assert.AreEqual(3352, perft.Perft(board, 3));
            Assert.AreEqual(50146, perft.Perft(board, 4));
            Assert.AreEqual(783632, perft.Perft(board, 5));
        }

        [TestMethod]
        public void TestPerftKingsAndQueens()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/2Q2q2/8/8/8/8/1K3k2 w - -"));

            var perft = new PerfT();

            Assert.AreEqual(26, perft.Perft(board, 1));
            Assert.AreEqual(512, perft.Perft(board, 2));
            Assert.AreEqual(11318, perft.Perft(board, 3));
            Assert.AreEqual(235936, perft.Perft(board, 4));
            //Assert.AreEqual(4991335, perft.Perft(board, 5));
        }

        #endregion Individual tests

        #region previously failed tests

        /// <summary>
        /// The first time I encountered this position in Arena the engine came up with an illegal move
        /// Not sure if it was the ngine or the UCI interface at fault
        /// </summary>
        [TestMethod]
        public void TestPerftArena_1a()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR4K1 w - - 1 22"));

            var perft = new PerfT();

            Assert.AreEqual(39, perft.Perft(board, 1));
            Assert.AreEqual(1020, perft.Perft(board, 2));
            Assert.AreEqual(37572, perft.Perft(board, 3));
            Assert.AreEqual(1005030, perft.Perft(board, 4));
        }

        [TestMethod]
        public void TestPerftArena_1b()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR1Q2K1 b - - 1 23"));

            var perft = new PerfT();
            
            Assert.AreEqual(28, perft.Perft(board, 1));
            Assert.AreEqual(1356, perft.Perft(board, 2));
            Assert.AreEqual(35758, perft.Perft(board, 3));
            Assert.AreEqual(1695866, perft.Perft(board, 4));
        }

        #endregion previously failed tests

    }
}
