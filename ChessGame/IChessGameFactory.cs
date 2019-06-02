namespace ChessGame
{
    public interface IChessGameFactory
    {
        Game CreateChessGame(bool useOpeningBook);
    }
}