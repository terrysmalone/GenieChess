namespace Genie_WPF;

public sealed class MoveItem : ViewModelBase
{
    public int MoveNumber { get; }

    public string WhiteMove { get; }

    public string BlackMove { get; }

    public MoveItem(int moveNumber, string whiteMove, string blackMove)
    {
        MoveNumber = moveNumber;
        WhiteMove = whiteMove;
        BlackMove = blackMove;
    }
}

