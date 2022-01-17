using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Genie_WPF.Annotations;

namespace Genie_WPF
{
    public sealed class ChessPiece : INotifyPropertyChanged
    {
        private Point _pos;
        private PieceType _type;
        private Player _player;

        public Point Pos
        {
            get => _pos;
            set { _pos = value; OnPropertyChanged(); }
        }

        public PieceType Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public Player Player
        {
            get => _player;
            set { _player = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
