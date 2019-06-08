using System;

namespace ChessGame.PossibleMoves
{
    /// <summary>
    /// Generates arrays for all valid moves, ignoring all other pieces, 
    /// that each piece can make from any position on the board
    /// </summary>
    public static class PieceValidMoves
    {
        static bool initilaised = false;

        public static void GenerateMoveArrays()
        {
            if (initilaised == false)
            {
                ValidMoveArrays.WhitePawnMoves = new ulong[64];
                ValidMoveArrays.WhitePawnCaptures = new ulong[64];
                ValidMoveArrays.BlackPawnMoves = new ulong[64];
                ValidMoveArrays.BlackPawnCaptures = new ulong[64];

                ValidMoveArrays.KnightMoves = new ulong[64];
                ValidMoveArrays.KingMoves = new ulong[64];

                InitialiseMovesWhitePawns();
                InitialiseMovesBlackPawns();
                InitialiseMovesKnights();
                InitialiseMovesKing();

                initilaised = true;
            }
        }

        #region Initialise pieces methods

        private static void InitialiseMovesWhitePawns()
        {
            InitialiseWhitePawnMoves();
            InitialiseWhitePawnCaptures();
        }

        private static void InitialiseWhitePawnMoves()
        { 
            for (byte startFile = 0; startFile <8 ; startFile++)
            {
                for (byte startRank = 1; startRank < 7; startRank++)
                {
                    ulong validMoves = 0;

                    var file = (byte)(startFile);
                    var rank = (byte)(startRank + 1);

                    validMoves = validMoves | CalculateSquareValue(file, rank);

                    //Add initial two moves
                    if (startRank == 1)
                    {
                        rank = 3;
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    var startSquare = (byte)(startFile + (startRank * 8));
                    ValidMoveArrays.WhitePawnMoves[startSquare] = validMoves;
                }
            }
        }

        private static void InitialiseWhitePawnCaptures()
        {
            for (byte startFile = 0; startFile < 8; startFile++)
            {
                for (byte startRank = 1; startRank < 7; startRank++)
                {
                    ulong validMoves = 0;

                    var file = (byte)(startFile + 1);
                    var rank = (byte)(startRank + 1);

                    if (file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile - 1);
                    rank = (byte)(startRank + 1);

                    if (file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }
                   
                    var startSquare = (byte)(startFile + (startRank * 8));
                    ValidMoveArrays.WhitePawnCaptures[startSquare] = validMoves;
                }
            }
        }

        private static void InitialiseMovesBlackPawns()
        {
            InitialiseBlackPawnMoves();
            InitialiseBlackPawnCaptures();
        }

        private static void InitialiseBlackPawnMoves()
        {
            for (byte startFile = 0; startFile < 8; startFile++)
            {
                for (byte startRank = 1; startRank < 7; startRank++)
                {
                    ulong validMoves = 0;

                    var file = (byte)(startFile);
                    var rank = (byte)(startRank - 1);

                    validMoves = validMoves | CalculateSquareValue(file, rank);

                    //Add initial two moves
                    if (startRank == 6)
                    {
                        rank = 4;
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    var startSquare = (byte)(startFile + (startRank * 8));
                    ValidMoveArrays.BlackPawnMoves[startSquare] = validMoves;
                }
            }
        }

        private static void InitialiseBlackPawnCaptures()
        {
            for (byte startFile = 0; startFile < 8; startFile++)
            {
                for (byte startRank = 1; startRank < 7; startRank++)
                {
                    ulong validMoves = 0;

                    var file = (byte)(startFile + 1);
                    var rank = (byte)(startRank - 1);

                    if (file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile - 1);
                    rank = (byte)(startRank - 1);

                    if (file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    var startSquare = (byte)(startFile + (startRank * 8));
                    ValidMoveArrays.BlackPawnCaptures[startSquare] = validMoves;
                }
            }
        }

        private static void InitialiseMovesKnights()
        {
            for (byte startFile = 0; startFile < 8; startFile++)
            {
                for (byte startRank = 0; startRank < 8; startRank++)
                {
                    //PieceMove move = new PieceMove();
                    //move.Position = CalculateSquareValue(startFile, startRank);
                    //move.Type = PieceType.Knight;

                    ulong validMoves = 0;

                    //
                    var file = (byte)(startFile + 1);
                    var rank = (byte)(startRank + 2);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile + 2);
                    rank = (byte)(startRank + 1);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile + 2);
                    rank = (byte)(startRank - 1);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile + 1);
                    rank = (byte)(startRank - 2);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile - 1);
                    rank = (byte)(startRank - 2);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile - 2);
                    rank = (byte)(startRank - 1);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile - 2);
                    rank = (byte)(startRank + 1);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    file = (byte)(startFile - 1);
                    rank = (byte)(startRank + 2);

                    if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                    {
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //move.Moves = validMoves;

                    var startSquare = (byte)(startFile + (startRank * 8));

                    ValidMoveArrays.KnightMoves[startSquare] = validMoves;
                }
            }

        }

        private static void InitialiseMovesKing()
        {
            for (byte startFile = 0; startFile < 8; startFile++)
            {
                for (byte startRank = 0; startRank < 8; startRank++)
                {
                    //PieceMove move = new PieceMove();
                    //move.Position = CalculateSquareValue(startFile, startRank);
                    //move.Type = PieceType.King;
                    
                    ulong validMoves = 0;
                    
                    //Up
                    var file = startFile;
                    var rank = startRank;

                    if (rank < 7)
                    {
                        rank++;

                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //Up right
                    file = startFile;
                    rank = startRank;

                    if (rank < 7 && file < 7)
                    {
                        rank++;
                        file++;

                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //Right
                    file = startFile;
                    rank = startRank;

                    if (file < 7)
                    {
                        file++;

                        validMoves = validMoves | CalculateSquareValue(file, rank);                      
                    }

                    //Right down
                    file = startFile;
                    rank = startRank;

                    if (file < 7 && rank > 0)
                    {
                        file++;
                        rank--;

                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //Down
                    file = startFile;
                    rank = startRank;

                    if (rank > 0)
                    {
                        rank--;
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //Left Down
                    file = startFile;
                    rank = startRank;

                    if (rank > 0 && file > 0)
                    {
                        rank--;
                        file--;
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //Left 
                    file = startFile;
                    rank = startRank;

                    if (file > 0)
                    {
                        file--;
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //Left Up
                    file = startFile;
                    rank = startRank;

                    if (file > 0 && rank < 7)
                    {
                        file--;
                        rank++;
                        validMoves = validMoves | CalculateSquareValue(file, rank);
                    }

                    //move.Moves = validMoves;

                    var startSquare = (byte)(startFile + (startRank * 8));

                    ValidMoveArrays.KingMoves[startSquare] = validMoves;

                }
            }
        }

        #endregion Initialise pieces methods

        #region move calculations

        private static ulong CalculateSquareValue(byte file, byte rank)
        {
            ulong moveIndex = (byte)(file + (rank * 8));
            var moveSquare = (ulong)Math.Pow(2, moveIndex);

            return moveSquare;
        }

        #endregion move calculations        
    }
}
