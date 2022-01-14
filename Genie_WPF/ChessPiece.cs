using System.Windows;

namespace Genie_WPF
{
    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public enum Player
    {
        White,
        Black
    }

    public class ChessPiece : ViewModelBase
    {
        private Point _Pos;
        public Point Pos
        {
            get { return this._Pos; }
            set { this._Pos = value; RaisePropertyChanged(nameof(Pos)); }
        }

        private PieceType _Type;
        public PieceType Type
        {
            get { return this._Type; }
            set { this._Type = value; RaisePropertyChanged(nameof(Type)); }
        }

        private Player _Player;
        public Player Player
        {
            get { return this._Player; }
            set { this._Player = value; RaisePropertyChanged(nameof(Player)); }
        }
    }
}
