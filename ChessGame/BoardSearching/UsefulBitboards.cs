using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.BoardSearching
{
    /// <summary>
    /// Predefines useful bitboards
    /// </summary>
    internal static class UsefulBitboards
    {
            public static ulong CentralSquares = 66229406269440;
            public static ulong InnerCentralSquares = 103481868288;
            public static ulong OuterCentralSquares = 66125924401152;

            public static ulong WhiteSquares = 6172840429334713770;
            public static ulong BlackSquares = 12273903644374837845;
    }
}
