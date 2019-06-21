using System;
using System.Collections.Generic;
using System.IO;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourceLoading;

namespace ChessEngineTests
{
    //[Ignore]
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

            //List<Tuple<string, ulong>> divides = perft.Divides(board);
            var divides = perft.Divide(board, 3);

            Assert.AreEqual((ulong)20, perft.Perft(board, 1));
            Assert.AreEqual((ulong)400, perft.Perft(board, 2));
            Assert.AreEqual((ulong)8902, perft.Perft(board, 3));
            Assert.AreEqual((ulong)197281, perft.Perft(board, 4));
            Assert.AreEqual((ulong)4865609, perft.Perft(board, 5));
            //Assert.AreEqual((ulong)119060324, perft.Perft(board, 6));
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

            Assert.AreEqual((ulong)20, perft.Perft(board, 1));
            Assert.AreEqual((ulong)400, perft.Perft(board, 2));
            Assert.AreEqual((ulong)8902, perft.Perft(board, 3));
            Assert.AreEqual((ulong)197281, perft.Perft(board, 4));
            Assert.AreEqual((ulong)4865609, perft.Perft(board, 5));
            //Assert.AreEqual((ulong)119060324, perft.Perft(board, 6));
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

            Assert.AreEqual((ulong)5, perft.Perft(board, 1));
            Assert.AreEqual((ulong)39, perft.Perft(board, 2));
            Assert.AreEqual((ulong)237, perft.Perft(board, 3));
            Assert.AreEqual((ulong)2002, perft.Perft(board, 4));
            Assert.AreEqual((ulong)14062, perft.Perft(board, 5));
            Assert.AreEqual((ulong)120995, perft.Perft(board, 6));
            Assert.AreEqual((ulong)966152, perft.Perft(board, 7));
            Assert.AreEqual((ulong)8103790, perft.Perft(board, 8));
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

            Assert.AreEqual((ulong)17, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual((ulong)341, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual((ulong)6666, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual((ulong)150072, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual((ulong)3186478, perft.Perft(board, 5), "Failed at 5");
            //Assert.AreEqual((ulong)77054993, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerft4()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -"));

            var perft = new PerfT();

            //var divides = perft.Divides(board);

            Assert.AreEqual((ulong)9, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual((ulong)85, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual((ulong)795, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual((ulong)7658, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual((ulong)72120, perft.Perft(board, 5), "Failed at 5");
            Assert.AreEqual((ulong)703851, perft.Perft(board, 6), "Failed at 6");
            //Assert.AreEqual((ulong)6627106, perft.Perft(board, 7));
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

            Assert.AreEqual((ulong)30, perft.Perft(board, 1));
            Assert.AreEqual((ulong)986, perft.Perft(board, 2));
            Assert.AreEqual((ulong)29777, perft.Perft(board, 3));
            Assert.AreEqual((ulong)967198, perft.Perft(board, 4));
            //Assert.AreEqual((ulong)29345534, perft.Perft(board, 5));
            //Assert.AreEqual((ulong)943244129, perft.Perft(board, 6));
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

            Assert.AreEqual((ulong)48, perft.Perft(board, 1));
            Assert.AreEqual((ulong)2039, perft.Perft(board, 2));
            Assert.AreEqual((ulong)97862, perft.Perft(board, 3));
            Assert.AreEqual((ulong)4085603, perft.Perft(board, 4));
            Assert.AreEqual((ulong)193690690, perft.Perft(board, 5));
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

            Assert.AreEqual((ulong)24, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual((ulong)496, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual((ulong)9483, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual((ulong)182838, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual((ulong)3605103, perft.Perft(board, 5), "Failed at 5");
            //Assert.AreEqual((ulong)71179139, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerft8()
        {
            var board = new Board();

            
            board.SetPosition(FenTranslator.ToBoardState("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1"));
                        
            var perft = new PerfT();
            
            Assert.AreEqual((ulong)14, perft.Perft(board, 1));
            Assert.AreEqual((ulong)191, perft.Perft(board, 2));
            Assert.AreEqual((ulong)2812, perft.Perft(board, 3));
            Assert.AreEqual((ulong)43238, perft.Perft(board, 4));
            Assert.AreEqual((ulong)674624, perft.Perft(board, 5));
            //Assert.AreEqual((ulong)11030083, perft.Perft(board, 6));
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

            Assert.AreEqual((ulong)37, perft.Perft(board, 1));
            Assert.AreEqual((ulong)1109, perft.Perft(board, 2));
            Assert.AreEqual((ulong)35507, perft.Perft(board, 3));
            Assert.AreEqual((ulong)1064191, perft.Perft(board, 4));
            //Assert.AreEqual((ulong)33673031, perft.Perft(board, 5));       //116.87 seconds - 08/02/2014
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

            Assert.AreEqual((ulong)18, perft.Perft(board, 1));
            Assert.AreEqual((ulong)92, perft.Perft(board, 2));
            Assert.AreEqual((ulong)1670, perft.Perft(board, 3));
            Assert.AreEqual((ulong)10138, perft.Perft(board, 4));
            Assert.AreEqual((ulong)185429, perft.Perft(board, 5));
            Assert.AreEqual((ulong)1134888, perft.Perft(board, 6));
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

             Assert.AreEqual((ulong)13, perft.Perft(board, 1));
             Assert.AreEqual((ulong)102, perft.Perft(board, 2));
             Assert.AreEqual((ulong)1266, perft.Perft(board, 3));
             Assert.AreEqual((ulong)10276, perft.Perft(board, 4));
             Assert.AreEqual((ulong)135655, perft.Perft(board, 5));
             Assert.AreEqual((ulong)1015133, perft.Perft(board, 6));
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

             Assert.AreEqual((ulong)15, perft.Perft(board, 1));
             Assert.AreEqual((ulong)126, perft.Perft(board, 2));
             Assert.AreEqual((ulong)1928, perft.Perft(board, 3));
             Assert.AreEqual((ulong)13931, perft.Perft(board, 4));
             Assert.AreEqual((ulong)206379, perft.Perft(board, 5));
             Assert.AreEqual((ulong)1440467, perft.Perft(board, 6));
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)15, perft.Perft(board, 1));
             Assert.AreEqual((ulong)66, perft.Perft(board, 2));
             Assert.AreEqual((ulong)1198, perft.Perft(board, 3));
             Assert.AreEqual((ulong)6399, perft.Perft(board, 4));
             Assert.AreEqual((ulong)120330, perft.Perft(board, 5));
             Assert.AreEqual((ulong)661072, perft.Perft(board, 6));
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)16, perft.Perft(board, 1));
             Assert.AreEqual((ulong)71, perft.Perft(board, 2));
             Assert.AreEqual((ulong)1286, perft.Perft(board, 3));
             Assert.AreEqual((ulong)7418, perft.Perft(board, 4));
             Assert.AreEqual((ulong)141077, perft.Perft(board, 5));
             Assert.AreEqual((ulong)803711, perft.Perft(board, 6));
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

             Assert.AreEqual((ulong)26, perft.Perft(board, 1));
             Assert.AreEqual((ulong)1141, perft.Perft(board, 2));
             Assert.AreEqual((ulong)27826, perft.Perft(board, 3));
             Assert.AreEqual((ulong)1274206, perft.Perft(board, 4));
             //Assert.AreEqual((ulong)31912360, perft.Perft(board, 5));
             //Assert.AreEqual((ulong)1509218880, perft.Perft(board, 6));            
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)44, perft.Perft(board, 1));
             Assert.AreEqual((ulong)1494, perft.Perft(board, 2));
             Assert.AreEqual((ulong)50509, perft.Perft(board, 3));
             Assert.AreEqual((ulong)1720476, perft.Perft(board, 4));
             //Assert.AreEqual((ulong)58773923, perft.Perft(board, 5));
             //Assert.AreEqual((ulong)2010267707, perft.Perft(board, 6));             
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)11, perft.Perft(board, 1));
             Assert.AreEqual((ulong)133, perft.Perft(board, 2));
             Assert.AreEqual((ulong)1442, perft.Perft(board, 3));
             Assert.AreEqual((ulong)19174, perft.Perft(board, 4));
             Assert.AreEqual((ulong)266199, perft.Perft(board, 5));
             Assert.AreEqual((ulong)3821001, perft.Perft(board, 6));             
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)29, perft.Perft(board, 1));
             Assert.AreEqual((ulong)165, perft.Perft(board, 2));
             Assert.AreEqual((ulong)5160, perft.Perft(board, 3));
             Assert.AreEqual((ulong)31961, perft.Perft(board, 4));
             Assert.AreEqual((ulong)1004658, perft.Perft(board, 5));
            // Assert.AreEqual((ulong)6334638, perft.Perft(board, 6));
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)9, perft.Perft(board, 1));
             Assert.AreEqual((ulong)40, perft.Perft(board, 2));
             Assert.AreEqual((ulong)472, perft.Perft(board, 3));
             Assert.AreEqual((ulong)2661, perft.Perft(board, 4));
             Assert.AreEqual((ulong)38983, perft.Perft(board, 5));
             Assert.AreEqual((ulong)217342, perft.Perft(board, 6));
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

             var divides = perft.Divides(board);

             Assert.AreEqual((ulong)6, perft.Perft(board, 1), "Failed at 1");
             Assert.AreEqual((ulong)27, perft.Perft(board, 2), "Failed at 2");
             Assert.AreEqual((ulong)273, perft.Perft(board, 3), "Failed at 3");
             Assert.AreEqual((ulong)1329, perft.Perft(board, 4), "Failed at 4");
             Assert.AreEqual((ulong)18135, perft.Perft(board, 5), "Failed at 5");
             Assert.AreEqual((ulong)92683, perft.Perft(board, 6), "Failed at 6");
             Assert.AreEqual((ulong)1555980, perft.Perft(board, 7), "Failed at 7");
             //Assert.AreEqual((ulong)8110830, perft.Perft(board, 8));
         }

        #region Perft simple test

        [TestMethod]
        public void TestPerftCheckMate()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("k6r/8/8/8/8/5b2/4n1p1/7K w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)0, perft.Perft(board, 1));
            Assert.AreEqual((ulong)0, perft.Perft(board, 2));
        }

        [TestMethod]
        public void TestPerftStalemate()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("k6r/8/8/8/8/5n1b/7p/7K w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)0, perft.Perft(board, 1));
            Assert.AreEqual((ulong)0, perft.Perft(board, 2));
        }
          
        #endregion Perft simple test

        #region Individual tests

        [TestMethod]
        public void TestPerftJustKings()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/8/8/8/8/8/2K1k3 w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)3, perft.Perft(board, 1));
            Assert.AreEqual((ulong)13, perft.Perft(board, 2));
            Assert.AreEqual((ulong)77, perft.Perft(board, 3));
            Assert.AreEqual((ulong)484, perft.Perft(board, 4));
            Assert.AreEqual((ulong)2630, perft.Perft(board, 5));
            //Assert.AreEqual((ulong)16680, perft.Perft(board, 6));
        }


        [TestMethod]
        public void TestPerftKingsAndBishops()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("2b1B3/8/8/8/8/8/8/2K1k3 b - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)10, perft.Perft(board, 1));
            Assert.AreEqual((ulong)99, perft.Perft(board, 2));
            Assert.AreEqual((ulong)1162, perft.Perft(board, 3));
            Assert.AreEqual((ulong)13614, perft.Perft(board, 4));
            Assert.AreEqual((ulong)166917, perft.Perft(board, 5));
            //Assert.AreEqual((ulong)2038592, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerftKingsAndPawns()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("2k5/2p5/8/8/8/8/3P4/3K4 w - - 0 1"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)6, perft.Perft(board, 1), "Failed at 1");
            Assert.AreEqual((ulong)36, perft.Perft(board, 2), "Failed at 2");
            Assert.AreEqual((ulong)253, perft.Perft(board, 3), "Failed at 3");
            Assert.AreEqual((ulong)1777, perft.Perft(board, 4), "Failed at 4");
            Assert.AreEqual((ulong)13516, perft.Perft(board, 5), "Failed at 5");
            Assert.AreEqual((ulong)98271, perft.Perft(board, 6), "Failed at 6");
        }

        [TestMethod]
        public void TestPerftKingsAndKnights()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("2K2k2/8/8/8/8/8/8/2n2N2 w - -"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)9, perft.Perft(board, 1));
            Assert.AreEqual((ulong)77, perft.Perft(board, 2));
            Assert.AreEqual((ulong)789, perft.Perft(board, 3));
            Assert.AreEqual((ulong)8001, perft.Perft(board, 4));
            Assert.AreEqual((ulong)85768, perft.Perft(board, 5));
            Assert.AreEqual((ulong)910419, perft.Perft(board, 6));
        }

        [TestMethod]
        public void TestPerftKingsAndRooks()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/8/8/8/8/2R2r2/1K3k2 w - -"));

            var perft = new PerfT();

            Assert.AreEqual((ulong)16, perft.Perft(board, 1));
            Assert.AreEqual((ulong)219, perft.Perft(board, 2));
            Assert.AreEqual((ulong)3352, perft.Perft(board, 3));
            Assert.AreEqual((ulong)50146, perft.Perft(board, 4));
            Assert.AreEqual((ulong)783632, perft.Perft(board, 5));
        }

        [TestMethod]
        public void TestPerftKingsAndQueens()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("8/8/2Q2q2/8/8/8/8/1K3k2 w - -"));

            var perft = new PerfT();

            var divides = perft.Divides(board);

            Assert.AreEqual((ulong)26, perft.Perft(board, 1));
            Assert.AreEqual((ulong)512, perft.Perft(board, 2));
            Assert.AreEqual((ulong)11318, perft.Perft(board, 3));
            Assert.AreEqual((ulong)235936, perft.Perft(board, 4));
            //Assert.AreEqual((ulong)4991335, perft.Perft(board, 5));
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

            var divides = perft.Divides(board);

            Assert.AreEqual((ulong)39, perft.Perft(board, 1));
            Assert.AreEqual((ulong)1020, perft.Perft(board, 2));
            Assert.AreEqual((ulong)37572, perft.Perft(board, 3));
            Assert.AreEqual((ulong)1005030, perft.Perft(board, 4));
        }

        [TestMethod]
        public void TestPerftArena_1b()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR1Q2K1 b - - 1 23"));

            var perft = new PerfT();
            
            Assert.AreEqual((ulong)28, perft.Perft(board, 1));
            Assert.AreEqual((ulong)1356, perft.Perft(board, 2));
            Assert.AreEqual((ulong)35758, perft.Perft(board, 3));
            Assert.AreEqual((ulong)1695866, perft.Perft(board, 4));
        }

        #endregion previously failed tests

    }
}
