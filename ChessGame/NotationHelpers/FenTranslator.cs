using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.BoardSearching;
using ChessGame.Enums;
using log4net;

namespace ChessGame.NotationHelpers
{
    /// <summary>
    /// Translates between BoardState and a FEN string
    /// </summary>
    public static class FenTranslator
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region ToBoardState methods

        /// <summary>
        /// Converts a fen string to a matching BoardState object
        /// </summary>
        /// <param name="fenNotation"></param>
        /// <returns></returns>
        public static BoardState ToBoardState(string fenNotation)
        {
            BoardState state = new BoardState();

            try
            {
                string[] parts = fenNotation.Split(null);

                SetBoard(ref state, parts[0]);

                string colour = parts[1];

                if (colour.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                    state.WhiteToMove = true;
                else if (colour.Equals("b", StringComparison.InvariantCultureIgnoreCase))
                    state.WhiteToMove = false;
                else
                {
                    log.Error(string.Format("Player colour in FEN string not recognised: {0}", colour));
                    throw new ArgumentException(string.Format("Player colour string not recognised: {0}", colour));
                }

                if (parts.Length > 2)
                    SetCastlingStatus(ref state, parts[2]);

                if (parts.Length > 3)
                    SetEnPassantPosition(ref state, parts[3]);

                if (parts.Length > 4)
                    state.HalfMoveClock = Convert.ToInt32(parts[4]);
                else
                    state.HalfMoveClock = 0;


                if (parts.Length > 5)
                    state.FullMoveClock = Convert.ToInt32(parts[5]);
                else
                    state.FullMoveClock = 0;

            }
            catch (Exception exc)
            {
                log.Error(string.Format("Error converting FEN string {0} to board state", fenNotation));
                    
            }

            return state;
        }

        private static void SetBoard(ref BoardState state, string parts)
        {
            int currentRank = 7;
            int currentFile = 0;

            char[] fenLetters = parts.ToCharArray();

            foreach (char letter in fenLetters)
            {
                if (letter.Equals('/'))
                {
                    currentRank--;
                    currentFile = 0;
                }
                else if (char.IsLetter(letter))
                {
                    if (char.IsLower(letter))
                    {
                        //Black
                        switch (letter)
                        {
                            case ('p'):
                                state.BlackPawns |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('n'):
                                state.BlackKnights |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('b'):
                                state.BlackBishops |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('r'):
                                state.BlackRooks |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('q'):
                                state.BlackQueen |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('k'):
                                state.BlackKing |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            default:
                                log.Error(string.Format("Unrecognised black piece letter: {0}", letter));                    
                                throw new Exception(string.Format("Unrecognised black piece letter: {0}", letter));
                        }
                    }
                    else
                    {
                        switch (letter)
                        {
                            case ('P'):
                                state.WhitePawns |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('N'):
                                state.WhiteKnights |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('B'):
                                state.WhiteBishops |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('R'):
                                state.WhiteRooks |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('Q'):
                                state.WhiteQueen |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            case ('K'):
                                state.WhiteKing |= LookupTables.SquareValuesFromPosition[currentFile, currentRank];
                                break;
                            default:
                                log.Error(string.Format("Unrecognised white piece letter: {0}", letter));                    
                                throw new Exception(string.Format("Unrecognised white piece letter: {0}", letter));
                        }
                    } 

                    currentFile++;
                }
                else if (char.IsNumber(letter))
                {
                    int num = (int)char.GetNumericValue(letter);

                    currentFile += num;
                }
                else
                {
                    log.Error(string.Format("Unrecognised board character: {0}", letter));
                    throw new Exception(string.Format("Unrecognised board character: {0}", letter));
                }
            }
        }

        private static void SetCastlingStatus(ref BoardState state, string castlingLetters)
        {
            char[] castlingChars = castlingLetters.ToCharArray();

            foreach (char letter in castlingChars)
            {
                if (letter.Equals('-'))
                {
                    state.WhiteCanCastleQueenside = false;
                    state.WhiteCanCastleKingside = false;
                    state.BlackCanCastleQueenside = false;
                    state.BlackCanCastleKingside = false;
                }
                else if (char.IsLetter(letter))
                {
                    if (letter.Equals('K'))
                        state.WhiteCanCastleKingside = true;
                    else if (letter.Equals('k'))
                        state.BlackCanCastleKingside = true;
                    else if (letter.Equals('Q'))
                        state.WhiteCanCastleQueenside = true;
                    else if (letter.Equals('q'))
                        state.BlackCanCastleQueenside = true;
                    else
                    {
                        log.Error(string.Format("Unrecognised castling character: {0}", letter));
                        throw new Exception(string.Format("Unrecognised castling character: {0}", letter));
                    }
                }
            }
        }

        private static void SetEnPassantPosition(ref BoardState state, string enPassantLetters)
        {
            char[] enPassantChars = enPassantLetters.ToCharArray();

            int enPassantFilePos = 0;

            foreach (char letter in enPassantChars)
            {
                if (letter.Equals('-'))
                {
                    state.EnPassantPosition = 0;
                }
                else if (char.IsLetter(letter))
                {
                    switch (letter)
                    {
                        case ('a'):
                            enPassantFilePos = 0;
                            break;
                        case ('b'):
                            enPassantFilePos = 1;
                            break;
                        case ('c'):
                            enPassantFilePos = 2;
                            break;
                        case ('d'):
                            enPassantFilePos = 3;
                            break;
                        case ('e'):
                            enPassantFilePos = 4;
                            break;
                        case ('f'):
                            enPassantFilePos = 5;
                            break;
                        case ('g'):
                            enPassantFilePos = 6;
                            break;
                        case ('h'):
                            enPassantFilePos = 7;
                            break;
                        default:
                            log.Error(string.Format("En passant letter not recognised: {0}", letter)); 
                            throw new Exception(string.Format("En passant letter not recognised: {0}", letter));
                    }
                }
                else if (char.IsNumber(letter))
                {
                    state.EnPassantPosition = LookupTables.SquareValuesFromPosition[enPassantFilePos, (int)(char.GetNumericValue(letter) - 1)];
                }
                else
                {
                    log.Error(string.Format("Unrecognised en Passant character: {0}", letter));
                    throw new Exception(string.Format("Unrecognised en Passant character: {0}", letter));
                }
            }
        }

        #endregion ToBoardState methods

        #region ToFENString methods

        public static string ToFENString(BoardState boardState)
        {
            string fenNotation = string.Empty;

            try
            {
                fenNotation += CalculateBoardString(boardState);
                fenNotation += " ";

                if (boardState.WhiteToMove)
                    fenNotation += "w";
                else
                    fenNotation += "b";

                fenNotation += " ";
                fenNotation += CalculateCastlingRights(boardState);
                fenNotation += " ";
                fenNotation += CalculateEnPassantSquare(boardState);
                fenNotation += " ";
                fenNotation += boardState.HalfMoveClock;
                fenNotation += " ";
                fenNotation += boardState.FullMoveClock;
            }
            catch (Exception exc)
            {
                log.Error(string.Format("Error converting Board state to FEN string"), exc);                        
            }
            return fenNotation;
        }

        private static string CalculateEnPassantSquare(BoardState boardState)
        {
            if (boardState.EnPassantPosition == 0)
                return "-";
            else
                return CalculateEnPassantSquare(boardState.EnPassantPosition);
        }

        private static string CalculateBoardString(BoardState boardState)
        {
            char[,] board = new char[8,8];

            List<byte> whitePawnPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhitePawns);
            PlaceLetter(ref board, whitePawnPositions, 'P');

            List<byte> whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteKnights);
            PlaceLetter(ref board, whiteKnightPositions, 'N');

            List<byte> whiteBishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteBishops);
            PlaceLetter(ref board, whiteBishopPositions, 'B');

            List<byte> whiteRookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteRooks);
            PlaceLetter(ref board, whiteRookPositions, 'R');

            List<byte> whiteQueenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteQueen);
            PlaceLetter(ref board, whiteQueenPositions, 'Q');

            List<byte> whiteKingPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteKing);
            PlaceLetter(ref board, whiteKingPositions, 'K');

            List<byte> blackPawnPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackPawns);
            PlaceLetter(ref board, blackPawnPositions, 'p');

            List<byte> blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackKnights);
            PlaceLetter(ref board, blackKnightPositions, 'n');

            List<byte> blackBishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackBishops);
            PlaceLetter(ref board, blackBishopPositions, 'b');

            List<byte> blackRookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackRooks);
            PlaceLetter(ref board, blackRookPositions, 'r');

            List<byte> blackQueenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackQueen);
            PlaceLetter(ref board, blackQueenPositions, 'q');

            List<byte> blackKingPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackKing);
            PlaceLetter(ref board, blackKingPositions, 'k');

            string boardString = WriteBoardToString(board);

            return boardString;
        }

        private static string WriteBoardToString(char[,] board)
        {
            string fenBoard = string.Empty;

            int spaceNum = 0;
            
            for (int row = 7; row >= 0; row--)
            {
                for (int column = 0; column < 8; column++)
                {
                    char currentChar = board[column, row];

                    if (char.IsLetter(currentChar))
                    {
                        if(spaceNum>0)
                        {
                            fenBoard += spaceNum;
                            spaceNum = 0;
                        }

                        fenBoard  += currentChar;
                    }
                    else if (currentChar == '\0')
                    {
                        spaceNum++;
                    }
                }

                if (spaceNum > 0)
                {
                    fenBoard += spaceNum;
                    spaceNum = 0;
                }

                fenBoard += "/";
            }

            fenBoard = fenBoard.Remove(fenBoard.Length - 1, 1);

            return fenBoard;
        }

        private static void PlaceLetter(ref char[,] board, List<byte> piecePositions, char pieceLetter)
        {
            foreach (byte pieceIndex in piecePositions)
            {
                byte row = (byte)(pieceIndex / 8);    // bottom to top
                byte column = (byte)(pieceIndex % 8);  //left to right

                board[column, row] = pieceLetter;
            }
        }

        private static string CalculateCastlingRights(BoardState boardState)
        {
            string castlingString = string.Empty;

            if (!boardState.WhiteCanCastleKingside && !boardState.WhiteCanCastleQueenside && !boardState.BlackCanCastleKingside && !boardState.BlackCanCastleQueenside)
                castlingString += "-";
            else
            {
                if (boardState.WhiteCanCastleKingside)
                {
                    castlingString += "K";
                }

                if (boardState.WhiteCanCastleQueenside)
                {
                    castlingString += "Q";
                }

                if (boardState.BlackCanCastleKingside)
                {
                    castlingString += "k";
                }

                if (boardState.BlackCanCastleQueenside)
                {
                    castlingString += "q";
                }
            }

            return castlingString;
        }

        private static string CalculateEnPassantSquare(ulong squareValue)
        {
            string square = string.Empty;

            byte index = BitboardOperations.GetSquareIndexFromBoardValue(squareValue);

            int row = index / 8;
            int column = (index % 8);

            string columnString = string.Empty;

            switch (column)
            {
                case (0):
                    columnString = "a";
                    break;
                case (1):
                    columnString = "b";
                    break;
                case (2):
                    columnString = "c";
                    break;
                case (3):
                    columnString = "d";
                    break;
                case (4):
                    columnString = "e";
                    break;
                case (5):
                    columnString = "f";
                    break;
                case (6):
                    columnString = "g";
                    break;
                case (7):
                    columnString = "h";
                    break;
                default:
                    log.Error(string.Format("En passant letter not recognised: {0}", columnString));    
                    throw new Exception(string.Format("En passant letter not recognised: {0}", columnString));
            }

            square += columnString + (row+1);

            return square;
        }

        #endregion ToFENString methods

        /// <summary>
        /// Gets the player to moe from the FEN string
        /// </summary>
        /// <param name="startingPosition"></param>
        /// <returns></returns>
        public static PieceColour GetPlayerColour(string startingPosition)
        {
            string[] parts = startingPosition.Split(null);

            string colour = parts[1];

            if (colour.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                return PieceColour.White;
            else
                return PieceColour.Black;
        }
    }
}
