using System.Collections.ObjectModel;
using System.Drawing;

namespace BitboardVisualiser;

public sealed class BitboardVisualiserViewModel  : ViewModelBase
{
    private ObservableCollection<Cell> _cellCollection;

    public ObservableCollection<Cell> CellCollection
    {
        get => _cellCollection;

        set
        {
            _cellCollection = value;
            OnPropertyChanged();
        }
    }

    public BitboardVisualiserViewModel()
    {
        _cellCollection = new ObservableCollection<Cell>();

        var cellValues = new ulong[64];

        var start = 1u;

        for (var i = 0; i < 64; i++)
        {
            cellValues[i] = start;
            start *=2;
        }

        for (var row = 7; row >=0; row--)
        {
            for (var column = 0; column <= 7; column++)
            {
                _cellCollection.Add(new Cell(cellValues[row*8 + column]));
            }
        }
    }
}
