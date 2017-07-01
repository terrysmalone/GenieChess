﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Enums;

namespace ChessGame.PossibleMoves
{
    /// <summary>
    /// Stores data for a piece position and all its possble moves
    /// </summary>
    public struct PieceMoves
    {
        public ulong Position;
        public ulong Moves;
        public PieceType Type;
        public SpecialMoveType SpecialMove;

        public override bool Equals(Object obj)
        {
            return obj is PieceMoves && this == (PieceMoves)obj;
        }
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Moves.GetHashCode() ^ Type.GetHashCode() ^ SpecialMove.GetHashCode();
        }
        public static bool operator ==(PieceMoves x, PieceMoves y)
        {
            return x.Position == y.Position && x.Moves == y.Moves && x.Type == y.Type && x.SpecialMove == y.SpecialMove;
        }
        public static bool operator !=(PieceMoves x, PieceMoves y)
        {
            return !(x == y);
        }
    }    
}
