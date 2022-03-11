namespace ChessEngine;

public interface IChessGameFactory
{
    Game CreateChessGame(bool useOpeningBook);
}
