using ChessEngine.BoardRepresentation;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class PerftVerificationTests
    {
        // https://sites.google.com/site/numptychess/perft/position-1
        [Test]
        public void TestPerftInitial([Values] bool useHashing)
        {
            var board = new Board();
            board.SetPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            Assert.That(perft.Perft(board, 1), Is.EqualTo(20));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(400));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(8902));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(197281));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(4865609));
        }
        
        // https://sites.google.com/site/numptychess/perft/position-2
        [Test]
        public void TestPerft2([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(5));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(39));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(237));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(2002));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(14062));
            Assert.That(perft.Perft(board, 6), Is.EqualTo(120995));
            Assert.That(perft.Perft(board, 7), Is.EqualTo(966152));
            Assert.That(perft.Perft(board, 8), Is.EqualTo(8103790));
        }
        
        // https://sites.google.com/site/numptychess/perft/position-3
        [Test]
        public void TestPerft3([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(17));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(341));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(6666));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(150072));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(3186478));
        }

        [Test]
        public void TestPerft4([Values] bool useHashing)
        {
            var board = new Board();
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("8/5p2/8/2k3P1/p3K3/8/1P6/8 b - -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(9));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(85));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(795));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(7658));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(72120));
            Assert.That(perft.Perft(board, 6), Is.EqualTo(703851));
            //Assert.AreEqual(6627106, perft.Perft(board, 7));
        }
        
        // https://sites.google.com/site/numptychess/perft/position-5
        [Test]
        public void TestPerft5([Values] bool useHashing)
        {
            var board = new Board();

            var perft = new PerfT
            {
                UseHashing = useHashing
            };
            
            board.SetPosition("r3k2r/pb3pp1/5n1p/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R b KQkq -");

            Assert.AreEqual(30, perft.Perft(board, 1));
            Assert.AreEqual(986, perft.Perft(board, 2));
            Assert.AreEqual(29777, perft.Perft(board, 3));
            Assert.AreEqual(967198, perft.Perft(board, 4));
        }

        // "kiwipete"
        // http://chessprogramming.wikispaces.com/Perft+Results
        [Test]
        public void TestPerft6([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(48));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(2039));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(97862));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(4085603));
        }

        // "kiwipete" (above) failed at higher depth than I do in unit tests. This test plays 3 moves
        // in from that, closer to the failed position to make sure it doesn't go unnoticed again
        // It failed because of a bug in the en-passant zobrist hashing
        [Test]
        public void TestPerft6Fail([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N1BQ1p/PPP1BPPP/R3K2R w KQkq c6 0 2");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(50));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(2028));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(98109));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(3901513));
        }

        // http://www.rocechess.ch/perft.html
        // Good for checking promotions
        [Test]
        public void TestPerft7([Values] bool useHashing)
        {
            var board = new Board();

            var perft = new PerfT
            {
                UseHashing = useHashing
            };
            
            board.SetPosition("n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(24));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(496));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(9483));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(182838));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(3605103));
        }

        [Test]
        public void TestPerft8([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
                        
            Assert.That(perft.Perft(board, 1), Is.EqualTo(14)); 
            Assert.That(perft.Perft(board, 2), Is.EqualTo(191)); 
            Assert.That(perft.Perft(board, 3), Is.EqualTo(2812)); 
            Assert.That(perft.Perft(board, 4), Is.EqualTo(43238)); 
            Assert.That(perft.Perft(board, 5), Is.EqualTo(674624)); 
            //Assert.AreEqual(11030083, perft.Perft(board, 6));
        }
        
        // A contrived position where the kings are in the middle of the board and can be checked lots next move
        [Test]
        public void TestPerft9([Values] bool useHashing)
        {
            var board = new Board();

            var perft = new PerfT
            {
                UseHashing = useHashing
            };
            
            board.SetPosition("q7/2pp3b/8/3n1k1r/R1K1N3/8/B4PP1/7Q b - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(37)); 
            Assert.That(perft.Perft(board, 2), Is.EqualTo(1109)); 
            Assert.That(perft.Perft(board, 3), Is.EqualTo(35507)); 
            Assert.That(perft.Perft(board, 4), Is.EqualTo(1064191)); 
        }

        // http://www.chessprogramming.net/philosophy/perfect-perft/
        [Test]
        public void TestPerft10([Values] bool useHashing)
        {
            var board = new Board();

            var perft = new PerfT
            {
                UseHashing = useHashing
            };
            
            board.SetPosition("3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(18)); 
            Assert.That(perft.Perft(board, 2), Is.EqualTo(92)); 
            Assert.That(perft.Perft(board, 3), Is.EqualTo(1670)); 
            Assert.That(perft.Perft(board, 4), Is.EqualTo(10138)); 
            Assert.That(perft.Perft(board, 5), Is.EqualTo(185429)); 
            Assert.That(perft.Perft(board, 6), Is.EqualTo(1134888)); 
        }
        
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Illegal ep move #2
         [Test]
         public void TestPerft11([Values] bool useHashing)
         {
             var board = new Board();
             
             var perft = new PerfT
             {
                 UseHashing = useHashing
             };

             board.SetPosition("8/8/4k3/8/2p5/8/B2P2K1/8 w - - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(13)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(102)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(1266)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(10276)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(135655)); 
             Assert.That(perft.Perft(board, 6), Is.EqualTo(1015133)); 
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // EP Capture Checks Opponent
         [Test]
         public void TestPerft12([Values] bool useHashing)
         {
             var board = new Board();
             
             var perft = new PerfT
             {
                 UseHashing = useHashing
             };

             board.SetPosition("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(15)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(126)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(1928)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(13931)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(206379)); 
             Assert.That(perft.Perft(board, 6), Is.EqualTo(1440467)); 
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Short Castling Gives Check
         [Test]
         public void TestPerft13([Values] bool useHashing)
         {
             var board = new Board();

             var perft = new PerfT
             {
                 UseHashing = useHashing
             };
             
             board.SetPosition("5k2/8/8/8/8/8/8/4K2R w K - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(15)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(66)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(1198)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(6399)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(120330)); 
             Assert.That(perft.Perft(board, 6), Is.EqualTo(661072)); 
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Long Castling Gives Check
         [Test]
         public void TestPerft14([Values] bool useHashing)
         {
             var board = new Board();

             var perft = new PerfT
             {
                 UseHashing = useHashing
             };
             
             board.SetPosition("3k4/8/8/8/8/8/8/R3K3 w Q - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(16)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(71)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(1286)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(7418)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(141077)); 
             Assert.That(perft.Perft(board, 6), Is.EqualTo(803711)); 
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         /// Castle Rights
         [Test]
         public void TestPerft15([Values] bool useHashing)
         {
             var board = new Board();
             
             var perft = new PerfT
             {
                 UseHashing = useHashing
             };

             board.SetPosition("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(26)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(1141)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(27826)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(1274206));         
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Castle Prevented
         [Test]
         public void TestPerft16([Values] bool useHashing)
         {
             var board = new Board();

             var perft = new PerfT
             {
                 UseHashing = useHashing
             };
             
             board.SetPosition("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(44)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(1494)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(50509)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(1720476));            
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Promote out of Check
         [Test]
         public void TestPerft17([Values] bool useHashing)
         {
             var board = new Board();

             var perft = new PerfT
             {
                 UseHashing = useHashing
             };
             
             board.SetPosition("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(11)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(133)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(1442)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(19174)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(266199)); 
             Assert.That(perft.Perft(board, 6), Is.EqualTo(3821001));              
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Discovered Check
         [Test]
         public void TestPerft18([Values] bool useHashing)
         {
             var board = new Board();
             
             var perft = new PerfT
             {
                 UseHashing = useHashing
             };

             board.SetPosition("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(29)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(165)); 
             Assert.That(perft.Perft(board, 3), Is.EqualTo(5160)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(31961)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(1004658)); 
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Promote to give check
         [Test]
         public void TestPerft19([Values] bool useHashing)
         {
             var board = new Board();
             
             var perft = new PerfT
             {
                 UseHashing = useHashing
             };

             board.SetPosition("4k3/1P6/8/8/8/8/K7/8 w - - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(9)); 
             Assert.That(perft.Perft(board, 2), Is.EqualTo(40));
             Assert.That(perft.Perft(board, 3), Is.EqualTo(472)); 
             Assert.That(perft.Perft(board, 4), Is.EqualTo(2661)); 
             Assert.That(perft.Perft(board, 5), Is.EqualTo(38983)); 
             Assert.That(perft.Perft(board, 6), Is.EqualTo(217342)); 
         }
         
         // http://www.chessprogramming.net/philosophy/perfect-perft/
         // Under Promote to give check
         [Test]
         public void TestPerft20([Values] bool useHashing)
         {
             var board = new Board();
             
             var perft = new PerfT
             {
                 UseHashing = useHashing
             };

             board.SetPosition("8/P1k5/K7/8/8/8/8/8 w - - 0 1");

             Assert.That(perft.Perft(board, 1), Is.EqualTo(6), "Failed at 1");
             Assert.That(perft.Perft(board, 2), Is.EqualTo(27), "Failed at 2");
             Assert.That(perft.Perft(board, 3), Is.EqualTo(273), "Failed at 3");
             Assert.That(perft.Perft(board, 4), Is.EqualTo(1329), "Failed at 4");
             Assert.That(perft.Perft(board, 5), Is.EqualTo(18135), "Failed at 5");
             Assert.That(perft.Perft(board, 6), Is.EqualTo(92683), "Failed at 6");
             Assert.That(perft.Perft(board, 7), Is.EqualTo(1555980), "Failed at 7");
         }

        [Test]
        public void TestPerftCheckMate([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("k6r/8/8/8/8/5b2/4n1p1/7K w - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(0));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(0));
        }

        [Test]
        public void TestPerftStalemate([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("k6r/8/8/8/8/5n1b/7p/7K w - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(0));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(0));
        }

        [Test]
        public void TestPerftJustKings([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("8/8/8/8/8/8/8/2K1k3 w - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(3));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(13));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(77));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(484));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(2630));
        }


        [Test]
        public void TestPerftKingsAndBishops([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("2b1B3/8/8/8/8/8/8/2K1k3 b - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(10));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(99));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(1162));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(13614));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(166917));
        }

        [Test]
        public void TestPerftKingsAndPawns([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("2k5/2p5/8/8/8/8/3P4/3K4 w - - 0 1");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(6), "Failed at 1");
            Assert.That(perft.Perft(board, 2), Is.EqualTo(36), "Failed at 2");
            Assert.That(perft.Perft(board, 3), Is.EqualTo(253), "Failed at 3");
            Assert.That(perft.Perft(board, 4), Is.EqualTo(1777), "Failed at 4");
            Assert.That(perft.Perft(board, 5), Is.EqualTo(13516), "Failed at 5");
            Assert.That(perft.Perft(board, 6), Is.EqualTo(98271), "Failed at 6");
        }

        [Test]
        public void TestPerftKingsAndKnights([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("2K2k2/8/8/8/8/8/8/2n2N2 w - -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(9));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(77));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(789));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(8001));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(85768));
            Assert.That(perft.Perft(board, 6), Is.EqualTo(910419));
        }

        [Test]
        public void TestPerftKingsAndRooks([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("8/8/8/8/8/8/2R2r2/1K3k2 w - -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(16));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(219));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(3352));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(50146));
            Assert.That(perft.Perft(board, 5), Is.EqualTo(783632));
        }

        [Test]
        public void TestPerftKingsAndQueens([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("8/8/2Q2q2/8/8/8/8/1K3k2 w - -");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(26));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(512));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(11318));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(235936));
        }
        
        // The first time I encountered this position in Arena the engine came up with an illegal move
        // Not sure if it was the engine or the UCI interface at fault
        [Test]
        public void TestPerftArena_1a([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR4K1 w - - 1 22");

            Assert.That(perft.Perft(board, 1), Is.EqualTo(39));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(1020));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(37572));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(1005030));
        }

        [Test]
        public void TestPerftArena_1b([Values] bool useHashing)
        {
            var board = new Board();
            
            var perft = new PerfT
            {
                UseHashing = useHashing
            };

            board.SetPosition("5rk1/1Bp1bpp1/1p6/7Q/8/3P4/5PPP/rR1Q2K1 b - - 1 23");
            
            Assert.That(perft.Perft(board, 1), Is.EqualTo(28));
            Assert.That(perft.Perft(board, 2), Is.EqualTo(1356));
            Assert.That(perft.Perft(board, 3), Is.EqualTo(35758));
            Assert.That(perft.Perft(board, 4), Is.EqualTo(1695866));
        }
    }
}
