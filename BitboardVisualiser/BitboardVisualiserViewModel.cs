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

        _cellCollection.Add(new Cell(72057594037927936u));
        _cellCollection.Add(new Cell(144115188075855872u));
        _cellCollection.Add(new Cell(288230376151711744));
    }
}
