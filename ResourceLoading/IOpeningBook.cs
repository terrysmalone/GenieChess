namespace ResourceLoading
{
    public interface IOpeningBook
    {
        string GetMove();

        void RegisterMadeMove(string uciMove);

        void ResetBook();
    }
}