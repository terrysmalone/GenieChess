using ChessGame.NotationHelpers;
using ChessGame.PossibleMoves;
using ChessGame.ResourceLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Books
{
    public class OpeningBook
    {
        List<Opening> openings = new List<Opening>();
        int plyCount = 0;
        Random rand = new Random();

        public OpeningBook(string bookName)
        {
            openings = ResourceLoader.LoadOpeningBook(bookName);
        }

        /// <summary>
        /// Gets a move from the opening book
        /// 
        /// Note: The move isn't registered as having been made until a call to RegisterMadeMove is carried out
        /// </summary>
        /// <returns></returns>
        public string GetMove()
        {
            //Filter out openings that don't have this move
            for (int i = openings.Count - 1; i >= 0; i--)
            {
                Opening currentOpening = openings[i];

                if (plyCount >= currentOpening.moves.Length)
                    openings.RemoveAt(i);
            }

            if (openings.Count > 0)
            {
                int pos = rand.Next(openings.Count);

                string move = openings[pos].moves[plyCount];

                return move;
            }

            return string.Empty;
        }

        /// <summary>
        /// Tell the opening book that this move was made
        /// </summary>
        public void RegisterMadeMove(string uciMove)
        {
            for (int i = openings.Count-1; i >= 0; i--)
            {
                Opening currentOpening = openings[i];

                if (plyCount >= currentOpening.moves.Length)
                    openings.RemoveAt(i);
                else
                {
                    if(currentOpening.moves[plyCount] != uciMove)
                        openings.RemoveAt(i);
                }
            }

            plyCount++;
        }
    }
}
