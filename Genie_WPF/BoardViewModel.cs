using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ChessEngine.BoardRepresentation;
using Genie_WPF.Annotations;
using Genie_WPF.Properties;

namespace Genie_WPF
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private string _gridImage;

        public BoardViewModel()
        {
            GridImage = "WhitePawn";
        }

        public string GridImage
        {
            get => _gridImage;

            set
            {
                _gridImage = value;
                OnPropertyChanged("GridImage");
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
