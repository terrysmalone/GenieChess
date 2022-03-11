namespace ChessEngine.Extensions;

public static class RandomExtensions
{
    public static ulong NextUlong(this Random rnd)
    {
        var buffer = new byte[sizeof(ulong)];
        rnd.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }
}

