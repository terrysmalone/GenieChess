using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using Logging;

namespace ChessEngine.NotationHelpers;

/// <summary>
/// Translates between BoardState and a FEN string
/// </summary>
public static class FenTranslator
{
    private static readonly ILog log;

    #region ToBoardState methods

    /// <summary>
    /// Converts a fen string to a matching BoardState object
    /// </summary>
    /// <param name="fenNotation"></param>
    /// <returns></returns>
    public static BoardState ToBoardState(string fenNotation)
    {
        // TODO: We need to verify the position

        var state = new BoardState();

        try
        {
            var parts = fenNotation.Split(null);

            SetBoard(ref state, parts[0]);

            var colour = parts[1];

            if (colour.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                state.WhiteToMove = true;
            else if (colour.Equals("b", StringComparison.InvariantCultureIgnoreCase))
                state.WhiteToMove = false;
            else
            {
                log.Error($"Player colour in FEN string not recognised: {colour}");
                throw new ArgumentException($"Player colour string not recognised: {colour}");
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
        catch (Exception)
        {
            log.Error($"Error converting FEN string {fenNotation} to board state");

        }

        return state;
    }

    private static void SetBoard(ref BoardState state, string parts)
    {
        var currentRank = 7;
        var currentFile = 0;

        var fenLetters = parts.ToCharArray();

        foreach (var letter in fenLetters)
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
                            log.Error($"Unrecognised black piece letter: {letter}");
                            throw new Exception($"Unrecognised black piece letter: {letter}");
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
                            log.Error($"Unrecognised white piece letter: {letter}");
                            throw new Exception($"Unrecognised white piece letter: {letter}");
                    }
                }

                currentFile++;
            }
            else if (char.IsNumber(letter))
            {
                var num = (int)char.GetNumericValue(letter);

                currentFile += num;
            }
            else
            {
                log.Error($"Unrecognised board character: {letter}");
                throw new Exception($"Unrecognised board character: {letter}");
            }
        }
    }

    private static void SetCastlingStatus(ref BoardState state, string castlingLetters)
    {
        var castlingChars = castlingLetters.ToCharArray();

        foreach (var letter in castlingChars)
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
                    log.Error($"Unrecognised castling character: {letter}");
                    throw new Exception($"Unrecognised castling character: {letter}");
                }
            }
        }
    }

    private static void SetEnPassantPosition(ref BoardState state, string enPassantLetters)
    {
        var enPassantChars = enPassantLetters.ToCharArray();

        var enPassantFilePos = 0;

        foreach (var letter in enPassantChars)
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
                        log.Error($"En passant letter not recognised: {letter}");
                        throw new Exception($"En passant letter not recognised: {letter}");
                }
            }
            else if (char.IsNumber(letter))
            {
                state.EnPassantPosition = LookupTables.SquareValuesFromPosition[enPassantFilePos, (int)(char.GetNumericValue(letter) - 1)];
            }
            else
            {
                log.Error($"Unrecognised en Passant character: {letter}");
                throw new Exception($"Unrecognised en Passant character: {letter}");
            }
        }
    }

    #endregion ToBoardState methods

    #region ToFENString methods

    public static string ToFenString(BoardState boardState)
    {
        var fenNotation = string.Empty;

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
        var board = new char[8,8];

        var whitePawnPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhitePawns);
        PlaceLetter(ref board, whitePawnPositions, 'P');

        var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteKnights);
        PlaceLetter(ref board, whiteKnightPositions, 'N');

        var whiteBishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteBishops);
        PlaceLetter(ref board, whiteBishopPositions, 'B');

        var whiteRookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteRooks);
        PlaceLetter(ref board, whiteRookPositions, 'R');

        var whiteQueenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteQueen);
        PlaceLetter(ref board, whiteQueenPositions, 'Q');

        var whiteKingPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.WhiteKing);
        PlaceLetter(ref board, whiteKingPositions, 'K');

        var blackPawnPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackPawns);
        PlaceLetter(ref board, blackPawnPositions, 'p');

        var blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackKnights);
        PlaceLetter(ref board, blackKnightPositions, 'n');

        var blackBishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackBishops);
        PlaceLetter(ref board, blackBishopPositions, 'b');

        var blackRookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackRooks);
        PlaceLetter(ref board, blackRookPositions, 'r');

        var blackQueenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackQueen);
        PlaceLetter(ref board, blackQueenPositions, 'q');

        var blackKingPositions = BitboardOperations.GetSquareIndexesFromBoardValue(boardState.BlackKing);
        PlaceLetter(ref board, blackKingPositions, 'k');

        var boardString = WriteBoardToString(board);

        return boardString;
    }

    private static string WriteBoardToString(char[,] board)
    {
        var fenBoard = string.Empty;

        var spaceNum = 0;

        for (var row = 7; row >= 0; row--)
        {
            for (var column = 0; column < 8; column++)
            {
                var currentChar = board[column, row];

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
        foreach (var pieceIndex in piecePositions)
        {
            var row = (byte)(pieceIndex / 8);    // bottom to top
            var column = (byte)(pieceIndex % 8);  //left to right

            board[column, row] = pieceLetter;
        }
    }

    private static string CalculateCastlingRights(BoardState boardState)
    {
        var castlingString = string.Empty;

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
        var square = string.Empty;

        var index = BitboardOperations.GetSquareIndexFromBoardValue(squareValue);

        var row = index / 8;
        var column = (index % 8);

        var columnString = string.Empty;

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
                log.Error($"En passant letter not recognised: {columnString}");
                throw new Exception($"En passant letter not recognised: {columnString}");
        }

        square += columnString + (row+1);

        return square;
    }

    #endregion ToFENString methods
}

