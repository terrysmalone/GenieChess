using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessGame.BoardRepresentation;
using ChessGame.PossibleMoves;
using ChessGame.ResourceLoading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessBoardTests
{
    [Ignore]
    [TestClass]
    public class PerftRuns
    {
        #region Perft logging methods

        public void LogPerftRun()
        {
            using (StreamWriter writer = new StreamWriter("PerftLog.txt"))
            {
                LogPerft("PerftInitial", writer, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", new List<ulong>() { 20, 400, 8902, 197281, 4865609, 119060324 });
                LogPerft("Perft2", writer, "8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -", new List<ulong>() { 5, 39, 237, 2002, 14062, 120995, 966152, 8103790 });
                LogPerft("Perft3", writer, "r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq -", new List<ulong>() { 17, 341, 6666, 150072, 3186478, 77054993 });
                LogPerft("Perft4", writer, "8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -", new List<ulong>() { 9, 85, 795, 7658, 72120, 703851, 6627106 });
                LogPerft("Perft5", writer, "r3k2r/pb3pp1/5n1p/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R b KQkq -", new List<ulong>() { 30, 986, 29777, 967198, 29345534, 943244129 });
                LogPerft("Perft6", writer, "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", new List<ulong>() { 48, 2039, 97862, 4085603, 193690690 });
                LogPerft("Perft7", writer, "n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1", new List<ulong>() { 24, 496, 9483, 182838, 3605103, 71179139 });
                LogPerft("Perft8", writer, "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", new List<ulong>() { 14, 191, 2812, 43238, 674624, 11030083 });
                LogPerft("Perft9", writer, "q7/2pp3b/8/3n1k1r/R1K1N3/8/B4PP1/7Q b - - 0 1", new List<ulong>() { 37, 1109, 35507, 1064191, 33673031 });
                LogPerft("Perft10", writer, "3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1", new List<ulong>() { 18, 92, 1670, 10138, 185429, 1134888 });
                LogPerft("Perft11", writer, "8/8/4k3/8/2p5/8/B2P2K1/8 w - - 0 1", new List<ulong>() { 13, 102, 1266, 10276, 135655, 1015133 });
                LogPerft("Perft12", writer, "8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1", new List<ulong>() { 15, 126, 1928, 13931, 206379, 1440467 });
                LogPerft("Perft13", writer, "5k2/8/8/8/8/8/8/4K2R w K - 0 1", new List<ulong>() { 15, 66, 1198, 6399, 120330, 661072 });
                LogPerft("Perft14", writer, "3k4/8/8/8/8/8/8/R3K3 w Q - 0 1", new List<ulong>() { 16, 71, 1286, 7418, 141077, 803711 });
                LogPerft("Perft15", writer, "r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1", new List<ulong>() { 26, 1141, 27826, 1274206, 31912360, 1509218880 });
                LogPerft("Perft16", writer, "r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1", new List<ulong>() { 44, 1494, 50509, 1720476, 58773923, 2010267707 });
                LogPerft("Perft17", writer, "2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1", new List<ulong>() { 11, 133, 1442, 19174, 266199, 3821001 });
                LogPerft("Perft18", writer, "8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1", new List<ulong>() { 29, 165, 5160, 31961, 1004658, 6334638 });
                LogPerft("Perft19", writer, "4k3/1P6/8/8/8/8/K7/8 w - - 0 1", new List<ulong>() { 9, 40, 472, 2661, 38983, 217342 });
                LogPerft("Perft20", writer, "8/P1k5/K7/8/8/8/8/8 w - - 0", new List<ulong>() { 6, 27, 273, 1329, 18135, 92683, 1555980, 8110830 });
            }
        }

        public void LogEndPerftRunQuick()
        {
            using (StreamWriter writer = new StreamWriter("PerftLogQuick.txt"))
            {
                LogPerft("PerftInitial", writer, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, 197281);
                LogPerft("Perft2", writer, "8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -", 7, 966152);
                LogPerft("Perft3", writer, "r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq -", 4, 150072);
                LogPerft("Perft4", writer, "8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -", 6, 703851);
                LogPerft("Perft5", writer, "r3k2r/pb3pp1/5n1p/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R b KQkq -", 4, 967198);
                LogPerft("Perft6", writer, "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 3, 97862);
                LogPerft("Perft7", writer, "n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1", 4, 182838);
                LogPerft("Perft8", writer, "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 5 , 674624);
                LogPerft("Perft9", writer, "q7/2pp3b/8/3n1k1r/R1K1N3/8/B4PP1/7Q b - - 0 1", 4, 1064191);
                LogPerft("Perft10", writer, "3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1",6, 1134888);
                LogPerft("Perft11", writer, "8/8/4k3/8/2p5/8/B2P2K1/8 w - - 0 1", 6, 1015133);
                LogPerft("Perft12", writer, "8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1", 6, 1440467);
                LogPerft("Perft13", writer, "5k2/8/8/8/8/8/8/4K2R w K - 0 1", 6, 661072);
                LogPerft("Perft14", writer, "3k4/8/8/8/8/8/8/R3K3 w Q - 0 1", 6, 803711);
                LogPerft("Perft15", writer, "r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1", 4, 1274206);
                LogPerft("Perft16", writer, "r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1", 3, 50509);
                LogPerft("Perft17", writer, "2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1", 6, 3821001);
                LogPerft("Perft18", writer, "8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1", 5, 1004658);
                LogPerft("Perft19", writer, "4k3/1P6/8/8/8/8/K7/8 w - - 0 1", 6, 217342);
                LogPerft("Perft20", writer, "8/P1k5/K7/8/8/8/8/8 w - - 0", 6 , 92683);
            }
        }
        
        private void LogPerft(string title, StreamWriter writer, string fenNotation, List<ulong> runs)
        {
            bool passed = true;
            ulong results;

            Board board = new Board();
            board.SetFenPosition(fenNotation);

            PerfT perft = new PerfT();

            writer.WriteLine();
            writer.WriteLine(title + " - " + fenNotation);
            
            for (int i=0; i<runs.Count; i++)
            {
                Console.WriteLine(string.Format("Checking {0} at depth {1}", title, i + 1));

                if ((results = perft.Perft(board, i+1)) != runs[i])
                {                    
                    writer.Write("FAIL! - ");
                    writer.WriteLine(string.Format("Depth {0} should have {1} nodes but has {2}", i+1, runs[i], results));
                    passed = false;
                }
                
                if (passed == false)
                    break;
            }            

            if (passed)
                writer.WriteLine(string.Format("Passed to depth {0}", runs.Count));
        }

        private void LogPerft(string title, StreamWriter writer, string fenNotation, int depth, ulong expectedResult)
        {
            DateTime startTime = DateTime.Now;

            bool passed = true;
            ulong results;

            Board board = new Board();
            board.SetFenPosition(fenNotation);

            PerfT perft = new PerfT();

            writer.WriteLine();
            writer.WriteLine(title + " - " + fenNotation);

            Console.WriteLine(string.Format("Checking {0} at depth {1}", title, depth));

            if ((results = perft.Perft(board, depth)) != expectedResult)
            {
                writer.Write("FAIL! - ");
                writer.WriteLine(string.Format("Depth {0} should have {1} nodes but has {2}", depth, expectedResult, results));
                passed = false;
                Console.WriteLine("FAILED!");
            }

            if (passed)
            {
                writer.Write(string.Format("Passed to depth {0}", depth));
                Console.WriteLine("passed");
            }

            DateTime endTime = DateTime.Now;

            TimeSpan runningTime = endTime - startTime;
            writer.WriteLine(string.Format(" - Running time:{0}", runningTime.TotalSeconds));

        }

        #endregion Perft logging methods

        //[TestMethod]
        //public void TestMultiPerfT()
        //{
        //    List<PerfTPosition> positions = ResourceLoader.LoadPerfTPositions();

        //    foreach (PerfTPosition position in positions)
        //    {
        //           TestPerft(position);
        //    }

        //}

        //private static void TestPerft(PerfTPosition position)
        //{
        //    ulong maxNodes = 10000000; //Dont test depths that go above this number of nodes

        //    Board board = new Board();
        //    board.SetFENPosition(position.FenPosition);

        //    PerfT perft = new PerfT();

        //    for (int i = 0; i < position.Results.Count; i++)
        //    {
        //        if (position.Results[i] <= maxNodes)
        //        {
        //            ulong result = perft.Perft(board, i+1);
        //            Assert.AreEqual(position.Results[i], result, string.Format("Failed on pos:{0}, depth:{1}. Nodes should be {2}, but were {3}", position.Name, i+1, position.Results[i], result));
        //        }
        //    }           
        //}


        /// <summary>
        /// https://sites.google.com/site/numptychess/perft/position-1
        /// </summary>
        [TestMethod]
        public void TestPerftInitial()
        {
            Board board = new Board();
            board.SetFenPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            PerfT perft = new PerfT();

            //List<Tuple<string, ulong>> divides = perft.Divides(board);

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
            Board board = new Board();
            board.SetFenPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            PerfT perft = new PerfT();
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
            Board board = new Board();
            PerfT perft = new PerfT();

            board.SetFenPosition("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -");

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
            Board board = new Board();

            board.SetFenPosition("r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq -");

            PerfT perft = new PerfT();

            List<Tuple<string, ulong>> divides = perft.Divides(board);

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
            Board board = new Board();

            board.SetFenPosition("8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -");
            //board.SetFENPosition("8/8/8/2k2pP1/p3K3/8/1P6/8 w - f6");
            PerfT perft = new PerfT();

            List<Tuple<string, ulong>> divides = perft.Divides(board);

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
            Board board = new Board();

            board.SetFenPosition("r3k2r/pb3pp1/5n1p/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R b KQkq -");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            board.SetFenPosition("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

            PerfT perft = new PerfT();

            Assert.AreEqual((ulong)48, perft.Perft(board, 1));
            Assert.AreEqual((ulong)2039, perft.Perft(board, 2));
            Assert.AreEqual((ulong)97862, perft.Perft(board, 3));
            Assert.AreEqual((ulong)4085603, perft.Perft(board, 4));
            //Assert.AreEqual((ulong)193690690, perft.Perft(board, 5));
        }

        /// <summary>
        /// http://www.rocechess.ch/perft.html
        /// Good for checking promotions
        /// </summary>
        [TestMethod]
        public void TestPerft7()
        {
            Board board = new Board();

            board.SetFenPosition("n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            
            board.SetFenPosition("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
                        
            PerfT perft = new PerfT();
            
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
            Board board = new Board();

            board.SetFenPosition("q7/2pp3b/8/3n1k1r/R1K1N3/8/B4PP1/7Q b - - 0 1");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            board.SetFenPosition("3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1");

            PerfT perft = new PerfT();

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
             Board board = new Board();

             board.SetFenPosition("8/8/4k3/8/2p5/8/B2P2K1/8 w - - 0 1");

             PerfT perft = new PerfT();

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
             Board board = new Board();

             board.SetFenPosition("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1");

             PerfT perft = new PerfT();

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
             Board board = new Board();

             board.SetFenPosition("5k2/8/8/8/8/8/8/4K2R w K - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
             Board board = new Board();

             board.SetFenPosition("3k4/8/8/8/8/8/8/R3K3 w Q - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
             Board board = new Board();

             board.SetFenPosition("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1");

             //board.SetFENPosition("r3k2r/1b4bq/8/8/8/8/7B/2KR3R b kq - 0 1");
             PerfT perft = new PerfT();

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
             Board board = new Board();

             board.SetFenPosition("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
             Board board = new Board();

             board.SetFenPosition("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
             Board board = new Board();

             board.SetFenPosition("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
             Board board = new Board();

             board.SetFenPosition("4k3/1P6/8/8/8/8/K7/8 w - - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
             Board board = new Board();

             board.SetFenPosition("8/P1k5/K7/8/8/8/8/8 w - - 0 1");

             PerfT perft = new PerfT();

             List<Tuple<string, ulong>> divides = perft.Divides(board);

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
            Board board = new Board();

            board.SetFenPosition("k6r/8/8/8/8/5b2/4n1p1/7K w - - 0 1");

            PerfT perft = new PerfT();

            Assert.AreEqual((ulong)0, perft.Perft(board, 1));
            Assert.AreEqual((ulong)0, perft.Perft(board, 2));
        }

        [TestMethod]
        public void TestPerftStalemate()
        {
            Board board = new Board();

            board.SetFenPosition("k6r/8/8/8/8/5n1b/7p/7K w - - 0 1");

            PerfT perft = new PerfT();

            Assert.AreEqual((ulong)0, perft.Perft(board, 1));
            Assert.AreEqual((ulong)0, perft.Perft(board, 2));
        }
          
        #endregion Perft simple test

        #region Individual tests

        [TestMethod]
        public void TestPerftJustKings()
        {
            Board board = new Board();

            board.SetFenPosition("8/8/8/8/8/8/8/2K1k3 w - - 0 1");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            board.SetFenPosition("2b1B3/8/8/8/8/8/8/2K1k3 b - - 0 1");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            board.SetFenPosition("2k5/2p5/8/8/8/8/3P4/3K4 w - - 0 1");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            board.SetFenPosition("2K2k2/8/8/8/8/8/8/2n2N2 w - -");

            PerfT perft = new PerfT();

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
            Board board = new Board();

            board.SetFenPosition("8/8/8/8/8/8/2R2r2/1K3k2 w - -");

            PerfT perft = new PerfT();

            Assert.AreEqual((ulong)16, perft.Perft(board, 1));
            Assert.AreEqual((ulong)219, perft.Perft(board, 2));
            Assert.AreEqual((ulong)3352, perft.Perft(board, 3));
            Assert.AreEqual((ulong)50146, perft.Perft(board, 4));
            Assert.AreEqual((ulong)783632, perft.Perft(board, 5));
        }

        [TestMethod]
        public void TestPerftKingsAndQueens()
        {
            Board board = new Board();

            board.SetFenPosition("8/8/2Q2q2/8/8/8/8/1K3k2 w - -");

            PerfT perft = new PerfT();

            List<Tuple<string, ulong>> divides = perft.Divides(board);

            Assert.AreEqual((ulong)26, perft.Perft(board, 1));
            Assert.AreEqual((ulong)512, perft.Perft(board, 2));
            Assert.AreEqual((ulong)11318, perft.Perft(board, 3));
            Assert.AreEqual((ulong)235936, perft.Perft(board, 4));
            //Assert.AreEqual((ulong)4991335, perft.Perft(board, 5));
        }

        #endregion Individual tests

        #region previously fialed tests

        /// <summary>
        /// The first time I encountered this position in Arena the engine came up with an illegal move
        /// Not sure if it was the ngine or the UCI interface at fault
        /// </summary>
        [TestMethod]
        public void TestPerftArena_1a()
        {
            Board board = new Board();

            board.SetFenPosition("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR4K1 w - - 1 22");

            PerfT perft = new PerfT();

            List<Tuple<string, ulong>> divides = perft.Divides(board);

            Assert.AreEqual((ulong)39, perft.Perft(board, 1));
            Assert.AreEqual((ulong)1020, perft.Perft(board, 2));
            Assert.AreEqual((ulong)37572, perft.Perft(board, 3));
            Assert.AreEqual((ulong)1005030, perft.Perft(board, 4));
        }

        [TestMethod]
        public void TestPerftArena_1b()
        {
            Board board = new Board();

            board.SetFenPosition("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR1Q2K1 b - - 1 23");

            PerfT perft = new PerfT();

            List<Tuple<string, ulong>> divides = perft.Divides(board);

            Assert.AreEqual((ulong)28, perft.Perft(board, 1));
            Assert.AreEqual((ulong)1356, perft.Perft(board, 2));
            Assert.AreEqual((ulong)35758, perft.Perft(board, 3));
            Assert.AreEqual((ulong)1695866, perft.Perft(board, 4));
        }

        #endregion previously fialed tests

    }
}
