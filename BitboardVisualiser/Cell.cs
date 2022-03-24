using System.Drawing;

namespace BitboardVisualiser;

public sealed class Cell
{
    public ulong CellValue { get; }

    public bool IsSelected { get; set; }

    public Cell(ulong cellValue)
    {
        CellValue = cellValue;
    }
}
