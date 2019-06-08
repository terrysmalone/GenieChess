using System;

namespace ChessGame.BoardSearching
{
    /// <summary>
    /// Initialises and generates lookup tables for speeding up bitboard translations
    /// </summary>
    [Serializable]
    public static class LookupTables
    {
        #region properties

        private static bool _initialised;

        #region square value properties

        public static ulong[] SquareValuesFromIndex = new ulong[64];
        public static ulong[,] SquareValuesFromPosition = new ulong[8, 8];

        #region Square values from board position

        public static ulong A1;
        public static ulong B1;
        public static ulong C1;
        public static ulong D1;
        public static ulong E1;
        public static ulong F1;
        public static ulong G1;
        public static ulong H1;

        public static ulong A2;
        public static ulong B2;
        public static ulong C2;
        public static ulong D2;
        public static ulong E2;
        public static ulong F2;
        public static ulong G2;
        public static ulong H2;

        public static ulong A3;
        public static ulong B3;
        public static ulong C3;
        public static ulong D3;
        public static ulong E3;
        public static ulong F3;
        public static ulong G3;
        public static ulong H3;

        public static ulong A4;
        public static ulong B4;
        public static ulong C4;
        public static ulong D4;
        public static ulong E4;
        public static ulong F4;
        public static ulong G4;
        public static ulong H4;

        public static ulong A5;
        public static ulong B5;
        public static ulong C5;
        public static ulong D5;
        public static ulong E5;
        public static ulong F5;
        public static ulong G5;
        public static ulong H5;

        public static ulong A6;
        public static ulong B6;
        public static ulong C6;
        public static ulong D6;
        public static ulong E6;
        public static ulong F6;
        public static ulong G6;
        public static ulong H6;

        public static ulong A7;
        public static ulong B7;
        public static ulong C7;
        public static ulong D7;
        public static ulong E7;
        public static ulong F7;
        public static ulong G7;
        public static ulong H7;

        public static ulong A8;
        public static ulong B8;
        public static ulong C8;
        public static ulong D8;
        public static ulong E8;
        public static ulong F8;
        public static ulong G8;
        public static ulong H8;

        #endregion Square values from board osition
        
        #endregion

        #region direction boards

        public static ulong[] UpBoard = new ulong[64];
        public static ulong[] RightBoard = new ulong[64];
        public static ulong[] DownBoard = new ulong[64];
        public static ulong[] LeftBoard = new ulong[64];

        public static ulong[] UpRightBoard = new ulong[64];
        public static ulong[] DownRightBoard = new ulong[64];
        public static ulong[] DownLeftBoard = new ulong[64];
        public static ulong[] UpLeftBoard = new ulong[64];

        public static ulong NotABoard = 18374403900871474942;
        public static ulong NotHBoard = 9187201950435737471;
        #endregion direction boards

        #region rank and file masks

        #region rank masks

        public static ulong RankMask1;
        public static ulong RankMask2;
        public static ulong RankMask3;
        public static ulong RankMask4;
        public static ulong RankMask5;
        public static ulong RankMask6;
        public static ulong RankMask7;
        public static ulong RankMask8;

        #endregion rank masks

        #region file masks

        public static ulong FileMaskA;
        public static ulong FileMaskB;
        public static ulong FileMaskC;
        public static ulong FileMaskD;
        public static ulong FileMaskE;
        public static ulong FileMaskF;
        public static ulong FileMaskG;
        public static ulong FileMaskH;

        #endregion file masks

        #region file mask lookup methods

        public static ulong[] FileMaskByColumn = new ulong[8];
        public static ulong[] FileMaskByIndex = new ulong[64];

        #endregion

        #endregion rank and file masks

        public static ulong WhiteCastlingQueensideObstructionPath;
        public static ulong WhiteCastlingQueensideAttackPath;
        public static ulong WhiteCastlingKingsideObstructionPath;
        public static ulong WhiteCastlingKingsideAttackPath;

        public static ulong BlackCastlingQueensideObstructionPath;
        public static ulong BlackCastlingQueensideAttackPath;
        public static ulong BlackCastlingKingsideObstructionPath;
        public static ulong BlackCastlingKingsideAttackPath;

        #endregion properties
                
        public static void InitialiseAllTables()
        {
            if (_initialised)
                return;

            GenerateSquareValueTable();

            CalculateDirectionBoards();

            CalculateRanksAndFileMasks();

            InitialiseSquareValues();

            InitialiseCastlingPathBoards();

            _initialised = true;
        }

        private static void InitialiseSquareValues()
        {
            A1 = 1;
            B1 = 2 * A1;
            C1 = 2 * B1;
            D1 = 2 * C1;
            E1 = 2 * D1;
            F1 = 2 * E1;
            G1 = 2 * F1;
            H1 = 2 * G1;

            A2 = 2 * H1;
            B2 = 2 * A2;
            C2 = 2 * B2;
            D2 = 2 * C2;
            E2 = 2 * D2;
            F2 = 2 * E2;
            G2 = 2 * F2;
            H2 = 2 * G2;

            A3 = 2 * H2;
            B3 = 2 * A3;
            C3 = 2 * B3;
            D3 = 2 * C3;
            E3 = 2 * D3;
            F3 = 2 * E3;
            G3 = 2 * F3;
            H3 = 2 * G3;

            A4 = 2 * H3;
            B4 = 2 * A4;
            C4 = 2 * B4;
            D4 = 2 * C4;
            E4 = 2 * D4;
            F4 = 2 * E4;
            G4 = 2 * F4;
            H4 = 2 * G4;

            A5 = 2 * H4;
            B5 = 2 * A5;
            C5 = 2 * B5;
            D5 = 2 * C5;
            E5 = 2 * D5;
            F5 = 2 * E5;
            G5 = 2 * F5;
            H5 = 2 * G5;

            A6 = 2 * H5;
            B6 = 2 * A6;
            C6 = 2 * B6;
            D6 = 2 * C6;
            E6 = 2 * D6;
            F6 = 2 * E6;
            G6 = 2 * F6;
            H6 = 2 * G6;

            A7 = 2 * H6;
            B7 = 2 * A7;
            C7 = 2 * B7;
            D7 = 2 * C7;
            E7 = 2 * D7;
            F7 = 2 * E7;
            G7 = 2 * F7;
            H7 = 2 * G7;

            A8 = 2 * H7;
            B8 = 2 * A8;
            C8 = 2 * B8;
            D8 = 2 * C8;
            E8 = 2 * D8;
            F8 = 2 * E8;
            G8 = 2 * F8;
            H8 = 2 * G8;
        }

        #region square value methods

        /// <summary>
        /// Generates a table which gives the bitboard value of all 64 squares
        /// </summary>
        private static void GenerateSquareValueTable()
        {
            GenerateSquareValuesFromFromIndex();
            GenerateSquareValuesFromFromPosition();            
        }

        private static void GenerateSquareValuesFromFromIndex()
        {
            SquareValuesFromIndex[0] = 1;

            for (var i = 1; i < SquareValuesFromIndex.Length; i++)
            {
                //Left shift gives the same result as multiplying by two but is faster
                SquareValuesFromIndex[i] = SquareValuesFromIndex[i - 1] << 1;
            }
        }

        private static void GenerateSquareValuesFromFromPosition()
        {
            ulong squareValue = 1;

            for (byte rank = 0; rank < 8; rank++)        
            {
                for (byte file = 0; file < 8; file++)
                {
                    SquareValuesFromPosition[file, rank] = squareValue;
                    squareValue = squareValue << 1;
                }
            }
        }

        #endregion square value methods

        #region Direction board methods

        private static void CalculateDirectionBoards()
        {
            CalculateUpDirectionBoards();
            CalculateRightDirectionBoards();
            CalculateDownDirectionBoards();
            CalculateLeftDirectionBoards();

            CalculateUpRightDirectionBoards();
            CalculateDownRightDirectionBoards();
            CalculateDownLeftDirectionBoards();
            CalculateUpLeftDirectionBoards();
        }

        private static void CalculateUpDirectionBoards()
        {
            for (var startRank = 0; startRank < 7; startRank++)        //Only goes to 7 as rank 8 will all be empty since there are no up moves
            {
                for (var startFile = 0; startFile < 8; startFile++)
                {
                    var rank = startRank + 1;
                    
                    ulong upSquares = 0;

                    while(rank < 8)
                    {
                        upSquares = upSquares | SquareValuesFromPosition[startFile, rank];

                        rank++;
                    }

                    UpBoard[(startRank * 8) + startFile] = upSquares;
                }
            }
        }

        private static void CalculateRightDirectionBoards()
        {
            for (var startRank = 0; startRank < 8; startRank++)        
            {
                for (var startFile = 0; startFile < 7; startFile++)    //Only goes to 7 as file 8 will all be empty since there are no right moves
                {
                    var file = startFile + 1;

                    ulong rightSquares = 0;

                    while (file < 8)
                    {
                        rightSquares = rightSquares | SquareValuesFromPosition[file, startRank];

                        file++;
                    }

                    RightBoard[(startRank*8)+startFile] = rightSquares;
                }
            }
        }

        private static void CalculateDownDirectionBoards()
        {
            for (var startRank = 1; startRank < 8; startRank++)        //Starts at 1 as rank 0 will all be empty since there are no up moves
            {
                for (var startFile = 0; startFile < 8; startFile++)
                {
                    var rank = startRank - 1;

                    ulong downSquares = 0;

                    while (rank >= 0)
                    {
                        downSquares = downSquares | SquareValuesFromPosition[startFile, rank];

                        rank--;
                    }

                    DownBoard[(startRank * 8) + startFile] = downSquares;
                }
            }
        }

        private static void CalculateLeftDirectionBoards()
        {
            for (var startRank = 0; startRank < 8; startRank++)
            {
                for (var startFile = 1; startFile < 8; startFile++)    //Starts at 1 as file 0 will all be empty since there are no left moves
                {
                    var file = startFile - 1;

                    ulong leftSquares = 0;

                    while (file >= 0)
                    {
                        leftSquares = leftSquares | SquareValuesFromPosition[file, startRank];

                        file--;
                    }

                    LeftBoard[(startRank * 8) + startFile] = leftSquares;
                }
            }
        }

        private static void CalculateUpRightDirectionBoards()
        {
            for (var startRank = 0; startRank < 7; startRank++)
            {
                for (var startFile = 0; startFile < 7; startFile++)    //Stops at 6 as file and rank 7 will all be empty since there are no up right moves
                {
                    var file = startFile + 1;
                    var rank = startRank + 1;

                    ulong upRightSquares = 0;

                    while (file < 8 && rank < 8)
                    {
                        upRightSquares = upRightSquares | SquareValuesFromPosition[file, rank];

                        file++;
                        rank++;
                    }

                    UpRightBoard[(startRank * 8) + startFile] = upRightSquares;
                }
            }
        }

        private static void CalculateDownRightDirectionBoards()
        {
            for (var startRank = 1; startRank < 8; startRank++)     //Starts at 1 as rank 0 will all be empty since there are no down right moves
            {
                for (var startFile = 0; startFile < 7; startFile++)    //Stops at 6 as file 7 will all be empty since there are no down right moves
                {
                    var file = startFile + 1;
                    var rank = startRank - 1;

                    ulong downRightSquares = 0;

                    while (file < 8 && rank >= 0)
                    {
                        downRightSquares = downRightSquares | SquareValuesFromPosition[file, rank];

                        file++;
                        rank--;
                    }

                    DownRightBoard[(startRank * 8) + startFile] = downRightSquares;
                }
            }
        }

        private static void CalculateDownLeftDirectionBoards()
        {
            for (var startRank = 1; startRank < 8; startRank++)     //Starts at 1 as rank 0 will all be empty since there are no down left moves
            {
                for (var startFile = 1; startFile < 8; startFile++)    //Starts at 1 as file 0 will all be empty since there are no down left moves
                {
                    var file = startFile - 1;
                    var rank = startRank - 1;

                    ulong downLeftSquares = 0;

                    while (file >= 0 && rank >= 0)
                    {
                        downLeftSquares = downLeftSquares | SquareValuesFromPosition[file, rank];

                        file--;
                        rank--;
                    }

                    DownLeftBoard[(startRank * 8) + startFile] = downLeftSquares;
                }
            }
        }

        private static void CalculateUpLeftDirectionBoards()
        {
            for (var startRank = 0; startRank < 7; startRank++)     
            {
                for (var startFile = 1; startFile < 8; startFile++)   
                {
                    var file = startFile - 1;
                    var rank = startRank + 1;

                    ulong upLeftSquares = 0;

                    while (file >= 0 && rank < 8)
                    {
                        upLeftSquares = upLeftSquares | SquareValuesFromPosition[file, rank];

                        file--;
                        rank++;
                    }

                    UpLeftBoard[(startRank * 8) + startFile] = upLeftSquares;
                }
            }
        }

        #endregion Direction board methods

        #region file and rank mask methods

        /// <summary>
        /// Calculates masks where an entire rank or file is set to 1
        /// </summary>
        private static void CalculateRanksAndFileMasks()
        {
            CalculateRankMasks();
            CalculateFileMasks();

            CalculateFileMaskByColumn();
            CalculateFileMaskByIndex();
        }

        private static void CalculateFileMasks()
        {
            FileMaskA = CalculateHorizontalMask(0);
            FileMaskB = CalculateHorizontalMask(1);
            FileMaskC = CalculateHorizontalMask(2);
            FileMaskD = CalculateHorizontalMask(3);
            FileMaskE = CalculateHorizontalMask(4);
            FileMaskF = CalculateHorizontalMask(5);
            FileMaskG = CalculateHorizontalMask(6);
            FileMaskH = CalculateHorizontalMask(7);

        }

        private static ulong CalculateHorizontalMask(int row)
        {
            ulong mask = 0;

            for (var column = 0; column < 8; column++)
            {
                mask = mask | SquareValuesFromPosition[row, column];
            }

            return mask;
        }

        private static void CalculateRankMasks()
        {
            RankMask1 = CalculateVerticalMask(0);
            RankMask2 = CalculateVerticalMask(1);
            RankMask3 = CalculateVerticalMask(2);
            RankMask4 = CalculateVerticalMask(3);
            RankMask5 = CalculateVerticalMask(4);
            RankMask6 = CalculateVerticalMask(5);
            RankMask7 = CalculateVerticalMask(6);
            RankMask8 = CalculateVerticalMask(7);
        }

        private static ulong CalculateVerticalMask(int column)
        {
            ulong mask = 0;

            for (var row = 0; row < 8; row++)
            {
                mask = mask | SquareValuesFromPosition[row, column];
            }

            return mask;
        }

        #region mask by num methods

        private static void CalculateFileMaskByColumn()
        { 
            FileMaskByColumn[0] = FileMaskA;
            FileMaskByColumn[1] = FileMaskB;
            FileMaskByColumn[2] = FileMaskC;
            FileMaskByColumn[3] = FileMaskD;
            FileMaskByColumn[4] = FileMaskE;
            FileMaskByColumn[5] = FileMaskF;
            FileMaskByColumn[6] = FileMaskG;
            FileMaskByColumn[7] = FileMaskH;             
        }

        private static void CalculateFileMaskByIndex()
        {  
            for (var index = 0; index < 64; index++)
            {
                var column = index % 8;

                switch (column)
                {
                    case 0:
                        FileMaskByIndex[index] = FileMaskA;
                        break;
                    case 1:
                        FileMaskByIndex[index] = FileMaskB;
                        break;
                    case 2:
                        FileMaskByIndex[index] = FileMaskC;
                        break;
                    case 3:
                        FileMaskByIndex[index] = FileMaskD;
                        break;
                    case 4:
                        FileMaskByIndex[index] = FileMaskE;
                        break;
                    case 5:
                        FileMaskByIndex[index] = FileMaskF;
                        break;
                    case 6:
                        FileMaskByIndex[index] = FileMaskG;
                        break;
                    case 7:
                        FileMaskByIndex[index] = FileMaskH;
                        break;
                    default:
                        throw new Exception("Bad column given: " + column);
                }

            }
        }

        #endregion mask by num methods

        #endregion file and rank mask methods

        private static void InitialiseCastlingPathBoards()
        {
            WhiteCastlingQueensideObstructionPath = B1 | C1 | D1;
            WhiteCastlingQueensideAttackPath = D1;
            WhiteCastlingKingsideObstructionPath = F1 | G1;
            WhiteCastlingKingsideAttackPath = F1;

            BlackCastlingQueensideObstructionPath = B8 | C8 | D8;
            BlackCastlingQueensideAttackPath = D8;
            BlackCastlingKingsideObstructionPath = F8 | G8;
            BlackCastlingKingsideAttackPath = F8;
        }
    }
}
