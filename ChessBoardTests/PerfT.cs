using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessGame.BoardRepresentation;
using ChessGame.Enums;
using ChessGame.PossibleMoves;
using ChessGame.BoardSearching;
using ChessGame.Debugging;
using ChessGame.MoveSearching;
using ChessGame.NotationHelpers;

namespace ChessBoardTests
{
    public class PerfT
    {
        private bool useHashing = false;

        public bool UseHashing
        {
            get { return useHashing; }
            set { useHashing = value; }
        }

        public ulong Perft(Board boardPosition, int depth)
        {
            if (useHashing)
            {
                Hash hash = TranspositionTable.ProbeTable(boardPosition.Zobrist, depth, decimal.MinValue, decimal.MaxValue);
                if (hash.Key != 0 && hash.Depth == depth)
                {
                    //verify
                    //string boardFEN = boardPosition.GetFENNotation();
                    //boardFEN = boardFEN.Substring(0, boardFEN.Length - 3);
                    //if (!hash.fenPosition.Substring(0, hash.fenPosition.Length - 3).Equals(boardFEN))
                    //{
                    //    Console.WriteLine("Positions do not match");
                    //}

                    if (hash.NodeType == HashNodeType.Exact)
                    {
                        ulong nds = Convert.ToUInt64(hash.Score);
                        return nds;
                    }
                    
                }
            }

            ulong nodes = 0;

            if (depth == 0)
            {
                CountDebugger.Nodes++;
                return 1;
            }
            //moves.CalculateAllMoves(boardPosition);

            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            //if (depth == 1)
            //{
            //    CountDebugger.Nodes += (ulong)moveList.Count;
            //    return (ulong)moveList.Count;
            //}

            for (int i = 0; i < moveList.Count; i++) 
            {
                bool skipMove = false;

                if (moveList[i].SpecialMove == SpecialMoveType.KingCastle || moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
                {
                    PieceColour friendlyColour = PieceColour.White;

                    if(boardPosition.WhiteToMove == false)
                        friendlyColour = PieceColour.Black;

                    if (BoardChecking.IsKingInCheckFast(boardPosition, friendlyColour) || !MoveGeneration.ValidateCastlingMove(boardPosition, moveList[i]))
                    {
                        skipMove = true;
                    }
                }

                if (!skipMove)
                {
                    boardPosition.MakeMove(moveList[i], false);

                    if (MoveGeneration.ValidateMove(boardPosition))
                        nodes += Perft(boardPosition, depth - 1);

                    boardPosition.UnMakeLastMove(false);
                }
            }

            if (useHashing)
            {
                HashNodeType hashNodeType = HashNodeType.Exact;
                RecordHash(boardPosition, depth, nodes, hashNodeType);
            }

            return nodes;
        }

        private void RecordHash(Board boardPosition, int depth, decimal score, HashNodeType hashNodeType)
        {
            Hash hash = new Hash();

            hash.Key = boardPosition.Zobrist;
            hash.Depth = depth;
            hash.NodeType = hashNodeType;
            hash.Score = score;
            //phashe->best = BestMove();
            //hash.fenPosition = FenTranslator.ToFENString(boardPosition.GetCurrentBoardState());

            TranspositionTable.Add(hash);
        }

        public List<Tuple<string, ulong>> Divide(Board boardPosition, int depth)
        {
            List<Tuple<string, ulong>> divides = new List<Tuple<string, ulong>>();
            
            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));

            ulong totalNodes = 0;
            
            Console.WriteLine(string.Format("Moves: {0}", moveList.Count));
            Console.WriteLine("");

            for (int i = 0; i < moveList.Count; i++)
            {
                PieceMoves currentMove = moveList[i];

                string rootMoveString = GetPieceMoveAsString(currentMove);
                
                boardPosition.MakeMove(currentMove, false);

                ulong numberOfNodes = Perft(boardPosition, depth-1);

                boardPosition.UnMakeLastMove(false);

                totalNodes += numberOfNodes;

                divides.Add(new Tuple<string, ulong>(rootMoveString, numberOfNodes));
            }

            return divides;
        }

        public List<Tuple<string, ulong>> Divides(Board boardPosition)
        {
            List<Tuple<string, ulong>> divides = new List<Tuple<string, ulong>>();

            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));

            foreach (var move in moveList)
            {
                boardPosition.MakeMove(move, false);
                                 
                 List<PieceMoves> branchMoves = MoveGeneration.CalculateAllMoves(boardPosition);
                
                 divides.Add(new Tuple<string, ulong>(GetPieceMoveAsString(move), (ulong)branchMoves.Count));

                boardPosition.UnMakeLastMove(false);
            }

            return divides;
        }

        public List<Tuple<string, ulong>> Divides(Board boardPosition, int depth)
        {
            List<Tuple<string, ulong>> divides = new List<Tuple<string, ulong>>();

            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));

            foreach (var move in moveList)
            {
                boardPosition.MakeMove(move, false);

                List<PieceMoves> branchMoves = MoveGeneration.CalculateAllMoves(boardPosition);

                divides.Add(new Tuple<string, ulong>(GetPieceMoveAsString(move), (ulong)branchMoves.Count));

                boardPosition.UnMakeLastMove(false);
            }

            return divides;
        }

        public List<string> GetAllMoves(Board position)
        {
            List<string> movesList = new List<string>();

            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(position));
            
            foreach (PieceMoves move  in moveList)
            {
                string pieceMove = GetPieceMoveAsString(move);
                movesList.Add(pieceMove);
            }

            return movesList;
        }

        private string GetPieceMoveAsString(PieceMoves move)
        {
            string pieceLetter;

            switch (move.Type)
            {
                case (PieceType.Pawn):
                    pieceLetter = "";
                    break;
                case (PieceType.Knight):
                    pieceLetter = "N";
                    break;
                case (PieceType.Bishop):
                    pieceLetter = "B";
                    break;
                case (PieceType.Rook):
                    pieceLetter = "R";
                    break;
                case (PieceType.Queen):
                    pieceLetter = "Q";
                    break;
                case (PieceType.King):
                    pieceLetter = "K";
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognised piece letter: {0}", move.Type));
            }

            string moveFrom = GetPostion(move.Position);
            string moveTo = GetPostion(move.Moves);

            //movesList.Add(string.Format("{0}. {1}{2}-{3}", i, pieceLetter, moveFrom, moveTo));

            return (string.Format("{0}{1}-{2}",pieceLetter, moveFrom, moveTo));
        }

        private string GetPostion(ulong position)
        {
            byte pos = BitboardOperations.GetSquareIndexFromBoardValue(position);

            int file = pos % 8;
            int rank = pos / 8;

            string fileLetter;

            switch (file)
            {
                case (0):
                    fileLetter = "a";
                    break;
                case (1):
                    fileLetter = "b";
                    break;
                case (2):
                    fileLetter = "c";
                    break;
                case (3):
                    fileLetter = "d";
                    break;
                case (4):
                    fileLetter = "e";
                    break;
                case (5):
                    fileLetter = "f";
                    break;
                case (6):
                    fileLetter = "g";
                    break;
                case (7):
                    fileLetter = "h";
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognised position letter: {0},{1}", file, rank));
            }

            return fileLetter + (rank + 1);
        }

    }
}
