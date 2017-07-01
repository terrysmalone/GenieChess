using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Extensions
{
    public static class RandomExtensions
    {
        public static ulong NextUlong(this Random rnd)
        {
            var buffer = new byte[sizeof(ulong)];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
