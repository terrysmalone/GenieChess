using System;
using ChessEngine.BoardRepresentation.Enums;

namespace ChessEngine.PossibleMoves
{
    /// <summary>
    /// Stores data for a piece position and all its possible moves
    /// </summary>
    public struct PieceMove
    {
        public ulong Position;
        public ulong Moves;
        public PieceType Type;
        public SpecialMoveType SpecialMove;

        public override bool Equals(Object obj)
        {
            return obj is PieceMove && this == (PieceMove)obj;
        }
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Moves.GetHashCode() ^ Type.GetHashCode() ^ SpecialMove.GetHashCode();
        }
        public static bool operator ==(PieceMove x, PieceMove y)
        {
            return x.Position == y.Position && x.Moves == y.Moves && x.Type == y.Type && x.SpecialMove == y.SpecialMove;
        }
        public static bool operator !=(PieceMove x, PieceMove y)
        {
            return !(x == y);
        }
    }    
}
