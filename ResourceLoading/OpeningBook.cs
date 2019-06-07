using System;
using System.Collections.Generic;
using System.IO;
using ResourceLoading;

namespace ResourceLoading
{
    public class OpeningBook : IOpeningBook
    {
        private List<Opening> m_Openings;

        private int m_PlyCount;

        private readonly Random m_Rand = new Random();

        public OpeningBook(string bookNameFilePath)
        {
            m_Openings = LoadOpeningBook(bookNameFilePath);

            FilePath = bookNameFilePath;
        }

        private List<Opening> LoadOpeningBook(string bookNamePath)
        {
            m_PlyCount = 0;

            var openings = new List<Opening>();

            var lines = File.ReadAllLines(bookNamePath);

            foreach (var line in lines)
            {
                var moves = SplitIntoChunks(line, 4);

                var opening = new Opening { Moves = moves };

                openings.Add(opening);
            }

            return openings;
        }

        private static string[] SplitIntoChunks(string str, int chunkSize)
        {
            var moveCount = Convert.ToInt32(Math.Ceiling(str.Length / (double)chunkSize));

            var parts = new string[moveCount];

            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = str.Substring(i * chunkSize, chunkSize);
            }

            //Last one may be smaller so get it here
            //int lastSize = str.Length % chunkSize;
            //parts[parts.Length - 1] = str.Substring(start, lastSize);

            return parts;
        }

        // Gets a move from the opening book
        // 
        // Note: The move isn't registered as having been made until
        //       a call to RegisterMadeMove is carried out
        public string GetMove()
        {
            //Filter out openings that don't have this move
            for (var i = m_Openings.Count - 1; i >= 0; i--)
            {
                var currentOpening = m_Openings[i];

                if (m_PlyCount >= currentOpening.Moves.Length)
                    m_Openings.RemoveAt(i);
            }

            if (m_Openings.Count <= 0) return string.Empty;

            var pos = m_Rand.Next(m_Openings.Count);

            var move = m_Openings[pos].Moves[m_PlyCount];

            return move;
        }

        // Tell the opening book that this move was made
        public void RegisterMadeMove(string uciMove)
        {
            for (var i = m_Openings.Count-1; i >= 0; i--)
            {
                var currentOpening = m_Openings[i];

                if (m_PlyCount >= currentOpening.Moves.Length)
                {
                    m_Openings.RemoveAt(i);
                }
                else
                {
                    if (currentOpening.Moves[m_PlyCount] != uciMove)
                    {
                        m_Openings.RemoveAt(i);
                    }
                }
            }

            m_PlyCount++;
        }

        public void ResetBook()
        {
            m_PlyCount = 0;
            m_Openings = LoadOpeningBook(FilePath);
        }

        public string FilePath { get; }
    }
}
