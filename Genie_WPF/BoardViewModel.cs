using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using ChessEngine.BoardRepresentation;
using Genie_WPF.Annotations;

namespace Genie_WPF
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private string _b6;
        private string _c6;

        public BoardViewModel()
        {
            B6 = "WhitePawn";
            C6 = "BlackRook";

            var chessBoard = (UniformGrid)App.Current.Windows[0].FindName("ChessBoard");
        }

        public string B6
        {
            get => _b6;

            set
            {
                _b6 = value;
                OnPropertyChanged("B6");
            }
        }

        public string C6
        {
            get => _c6;

            set
            {
                _c6 = value;
                OnPropertyChanged("C6");
            }
        }


        public BitmapImage GridImageA
        {
            get;

            set;
        }

        private string _myTextBoxValue;
        public string MyTextBoxValue
        {
            get => _myTextBoxValue;

            set
            {
                _myTextBoxValue = value;
                OnPropertyChanged("MyTextBoxValue");
            }
        }
        public object FullGrid
        {
            get { throw new System.NotImplementedException(); }
        }

        public void AddPiece(object whitePawn, string position)
        {
            
        }
        public void SetBoard(BoardState boardState)
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
