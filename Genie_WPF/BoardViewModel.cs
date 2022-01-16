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

        public BoardViewModel()
        {
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
