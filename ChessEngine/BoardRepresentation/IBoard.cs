using System.Collections.Generic;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.PossibleMoves;

namespace ChessEngine.BoardRepresentation
{
    public interface IBoard
    {
        ulong WhitePawns { get; }
        ulong WhiteKnights { get; }
        ulong WhiteBishops { get; }
        ulong WhiteRooks { get; }
        ulong WhiteQueen { get; }
        ulong WhiteKing { get; }

        ulong BlackPawns { get; }
        ulong BlackKnights { get; }
        ulong BlackBishops { get; }
        ulong BlackRooks { get; }
        ulong BlackQueen { get; }
        ulong BlackKing { get; }

        ulong AllWhiteOccupiedSquares { get; }
        ulong AllBlackOccupiedSquares { get; }

        ulong AllOccupiedSquares { get; }

        ulong EmptySquares { get; }

        ulong WhiteOrEmpty { get; }
        ulong BlackOrEmpty { get; }

        bool WhiteCanCastleQueenside { get; set; }
        bool WhiteCanCastleKingside { get; set; }

        bool BlackCanCastleQueenside { get; set; }
        bool BlackCanCastleKingside { get; set; }

        ulong EnPassantPosition { get; }

        bool WhiteToMove { get; }

        int HalfMoveClock { get; }

        int FullMoveClock { get; }

        ulong Zobrist { get; }
        
        void InitaliseStartingPosition();

        void ClearBoard();

        void SetPosition(BoardState state);

        void PlacePiece(PieceType typeToPlace, bool pieceColour, int file, int rank);

        void PlacePiece(PieceType typeToPlace, bool pieceColour, ulong squareToPlace);

        void RemovePiece(int positionToClear);

        void RemovePiece(int file, int rank);

        void RemovePiece(ulong squareToClear);

        void AllowAllCastling(bool value);

        void MakeMove(PieceMoves move, bool confirmedMove);

        void MakeMove(ulong moveFromBoard, 
            ulong moveToBoard, 
            PieceType pieceToMove, 
            SpecialMoveType specialMove, 
            bool confirmedMove);

        void SwitchSides();

        void UnMakeLastMove();

        void CalculateUsefulBitboards();

        BoardState GetCurrentBoardState();

        void ResetFlags();

        void WriteBoardToConsole();
    }
}