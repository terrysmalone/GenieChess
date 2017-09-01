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
        private readonly List<Opening> m_Openings;
        private int m_PlyCount;
        private readonly Random m_Rand = new Random();

        public OpeningBook(string bookName)
        {
            m_Openings = ResourceLoader.LoadOpeningBook(bookName);
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
            for (var i = m_Openings.Count - 1; i >= 0; i--)
            {
                var currentOpening = m_Openings[i];

                if (m_PlyCount >= currentOpening.moves.Length)
                    m_Openings.RemoveAt(i);
            }

            if (m_Openings.Count <= 0) return string.Empty;

            var pos = m_Rand.Next(m_Openings.Count);

            var move = m_Openings[pos].moves[m_PlyCount];

            return move;
        }

        /// <summary>
        /// Tell the opening book that this move was made
        /// </summary>
        public void RegisterMadeMove(string uciMove)
        {
            for (var i = m_Openings.Count-1; i >= 0; i--)
            {
                var currentOpening = m_Openings[i];

                if (m_PlyCount >= currentOpening.moves.Length)
                {
                    m_Openings.RemoveAt(i);
                }
                else
                {
                    if (currentOpening.moves[m_PlyCount] != uciMove)
                    {
                        m_Openings.RemoveAt(i);
                    }
                }
            }

            m_PlyCount++;
        }
    }
}
