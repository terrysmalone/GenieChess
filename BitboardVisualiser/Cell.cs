using System.Drawing;

namespace BitboardVisualiser;

public sealed class Cell : ViewModelBase
{
    private bool _isSelected;
    public ulong CellValue { get; }

    public bool IsSelected
    {
        get { return _isSelected; }

        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public Cell(ulong cellValue)
    {
        CellValue = cellValue;
    }
}
