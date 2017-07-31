using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;

namespace ChessGame.NotationHelpers
{
    public interface INotationTranslator
    {
        BoardState ToBoardState(string notation);

        string FromBoardState(BoardState boardState);

        string ToBoardMove(string notation);

        string FromBoardMove(Board board, ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove);
    }
}
