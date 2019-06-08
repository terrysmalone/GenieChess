using System.Windows.Forms;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(BitboardVisualiser.DebuggerBitboard),
typeof(VisualizerObjectSource),
Target = typeof(System.UInt64),
Description = "Displays the contents of a UInt64 bitboard on an 8x8 grid")]
namespace BitboardVisualiser
{
    public class DebuggerBitboard : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var bitboardValue = (ulong)objectProvider.GetObject();

            var bits = new char[64];

            var currentValue = 9223372036854775808;  //square h8

            for (var i = 63; i >= 0; i--)
            {
                if (bitboardValue >= currentValue)
                {
                    bits[i] = 'X';

                    bitboardValue -= currentValue;                    
                }

                currentValue = currentValue >> 1;
            }

            var finalValue = "";

            for (var row = 7; row >= 0; row--)
            {
                finalValue += "|";

                for (var column = 0; column < 8; column++)
                {
                    if (bits[column + (row * 8)] == 'X')
                    {
                        finalValue += " x |";
                    }
                    else
                    {
                        finalValue += " _ |";
                    }
                }

                finalValue += "\n";
                
            }

            MessageBox.Show(finalValue);
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = 
                new VisualizerDevelopmentHost(objectToVisualize, typeof(DebuggerBitboard));

            visualizerHost.ShowVisualizer();
        }
    }
}
