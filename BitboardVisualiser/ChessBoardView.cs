using System.Drawing;
using System.Windows.Forms;
using ChessGame.BoardRepresentation.Enums;

namespace BitboardVisualiser
{
    public partial class ChessBoardView : Form
    {
        PictureBox[,] chessSquare;

        public ChessBoardView()
        {
            InitializeComponent();

            this.ClientSize = new Size(400, 400);

            chessSquare = new PictureBox[8, 8];

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    chessSquare[i, j] = new PictureBox();
                    chessSquare[i, j].Size = new Size(50, 50);
                    chessSquare[i, j].Location = new Point((i * 50), (400 - (j+1) * 50));
                    //chessSquare[i, j].BorderStyle = BorderStyle.FixedSingle;
                    chessSquare[i, j].SizeMode = PictureBoxSizeMode.StretchImage;

                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                            chessSquare[i, j].BackColor = Color.Teal;
                        else
                            chessSquare[i, j].BackColor = Color.LightCyan;

                    }
                    else
                    {
                        if (j % 2 == 0)
                            chessSquare[i, j].BackColor = Color.LightCyan;
                        else
                            chessSquare[i, j].BackColor = Color.Teal;
                    }

                    this.Controls.Add(chessSquare[i, j]);
                }   
            }
        }

        internal void AddPiece(PieceType pieceType, PieceColour pieceColour, byte location)
        {
            //0 is bottom left travelling right and up 
                        
            var row = (int)location / 8;
            var col = (int)location % 8;

            var image = GetImage(pieceType, pieceColour);
            chessSquare[col, row].Image = image;

        }

        private Image GetImage(PieceType pieceType, PieceColour pieceColour)
        {
            if (pieceColour == PieceColour.White)
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return Properties.Resources.pawn_white;
                    case PieceType.Knight:
                        return Properties.Resources.knight_white;
                    case PieceType.Bishop:
                        return Properties.Resources.bishop_white;
                    case PieceType.Rook:
                        return Properties.Resources.rook_white;
                    case PieceType.Queen:
                        return Properties.Resources.queen_white;
                    case PieceType.King:
                        return Properties.Resources.king_white;

                }
            }
            else
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return Properties.Resources.pawn_black;
                    case PieceType.Knight:
                        return Properties.Resources.knight_black;
                    case PieceType.Bishop:
                        return Properties.Resources.bishop_black;
                    case PieceType.Rook:
                        return Properties.Resources.rook_black;
                    case PieceType.Queen:
                        return Properties.Resources.queen_black;
                    case PieceType.King:
                        return Properties.Resources.king_black;
                }
            }

            return null;
        }
    }
}
