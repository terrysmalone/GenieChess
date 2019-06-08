using System;

namespace ChessEngineTests
{
    public static class RandomExtensions
    {
        public static ulong Nextulong(this Random rnd)
        {
            var buffer = new byte[sizeof(ulong)];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
