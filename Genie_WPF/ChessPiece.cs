using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Genie_WPF.Annotations;

namespace Genie_WPF
{
    public sealed class ChessPiece : INotifyPropertyChanged
    {
        public ChessPiece()
        {

        }

        private Point _Pos;

        public Point Pos
        {
            get { return _Pos; }
            set { _Pos = value; OnPropertyChanged(); }
        }

        private PieceType _Type;
        public PieceType Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged(); }
        }

        private Player _Player;
        public Player Player
        {
            get { return _Player; }
            set { _Player = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
