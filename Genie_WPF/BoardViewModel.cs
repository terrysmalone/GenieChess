using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ChessEngine.BoardRepresentation;
using Genie_WPF.Annotations;

namespace Genie_WPF
{
    public sealed class BoardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ChessPiece> _chessPieces = new()
        {
                new ChessPiece{Pos=new Point(0, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(1, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(2, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(3, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(4, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(5, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(6, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(7, 6), Type=PieceType.Pawn, Player=Player.White},
                new ChessPiece{Pos=new Point(0, 7), Type=PieceType.Rook, Player=Player.White},
                new ChessPiece{Pos=new Point(1, 7), Type=PieceType.Knight, Player=Player.White},
                new ChessPiece{Pos=new Point(2, 7), Type=PieceType.Bishop, Player=Player.White},
                new ChessPiece{Pos=new Point(3, 7), Type=PieceType.King, Player=Player.White},
                new ChessPiece{Pos=new Point(4, 7), Type=PieceType.Queen, Player=Player.White},
                new ChessPiece{Pos=new Point(5, 7), Type=PieceType.Bishop, Player=Player.White},
                new ChessPiece{Pos=new Point(6, 7), Type=PieceType.Knight, Player=Player.White},
                new ChessPiece{Pos=new Point(7, 7), Type=PieceType.Rook, Player=Player.White},
                new ChessPiece{Pos=new Point(0, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(1, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(2, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(3, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(4, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(5, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(6, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(7, 1), Type=PieceType.Pawn, Player=Player.Black},
                new ChessPiece{Pos=new Point(0, 0), Type=PieceType.Rook, Player=Player.Black},
                new ChessPiece{Pos=new Point(1, 0), Type=PieceType.Knight, Player=Player.Black},
                new ChessPiece{Pos=new Point(2, 0), Type=PieceType.Bishop, Player=Player.Black},
                new ChessPiece{Pos=new Point(3, 0), Type=PieceType.King, Player=Player.Black},
                new ChessPiece{Pos=new Point(4, 0), Type=PieceType.Queen, Player=Player.Black},
                new ChessPiece{Pos=new Point(5, 0), Type=PieceType.Bishop, Player=Player.Black},
                new ChessPiece{Pos=new Point(6, 0), Type=PieceType.Knight, Player=Player.Black},
                new ChessPiece{Pos=new Point(7, 0), Type=PieceType.Rook, Player=Player.Black}
            };

        public BoardViewModel()
        {
            //SetBoard(boardState);
        }

        public ObservableCollection<ChessPiece> ChessPieces
        {
            get { return _chessPieces; }

            set
            {
                _chessPieces = value;
                OnPropertyChanged();
            }
        }

        public void AddPiece(object whitePawn, string position)
        {
            
        }
        private static void SetBoard(BoardState boardState)
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
