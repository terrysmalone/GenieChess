﻿using System;
using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngineTests
{
    public class PerfT
    {
        public bool UseHashing { get; set; } = true;

        public ulong Perft(Board boardPosition, int depth)
        {
            if (UseHashing)
            {
                var hash = TranspositionTable.ProbeTable(boardPosition.Zobrist, 
                                                         depth, 
                                                         decimal.MinValue, 
                                                         decimal.MaxValue);

                if (hash.Key != 0 && hash.Depth == depth)
                {
                    if (hash.NodeType == HashNodeType.Exact)
                    {
                        var nds = Convert.ToUInt64(hash.Score);

                        return nds;
                    }
                }
            }

            ulong nodes = 0;

            if (depth == 0)
            {
                return 1;
            }

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            for (var i = 0; i < moveList.Count; i++) 
            {
                var skipMove = false;

                if (moveList[i].SpecialMove == SpecialMoveType.KingCastle || moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
                {
                    var friendlyColour = PieceColour.White;

                    if(boardPosition.WhiteToMove == false)
                        friendlyColour = PieceColour.Black;

                    if (BoardChecking.IsKingInCheck(boardPosition, friendlyColour) || !MoveGeneration.ValidateCastlingMove(boardPosition, moveList[i]))
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

            if (UseHashing)
            {
                var hashNodeType = HashNodeType.Exact;
                RecordHash(boardPosition, depth, nodes, hashNodeType);
            }

            return nodes;
        }

        private static void RecordHash(IBoard boardPosition, int depth, decimal score, HashNodeType hashNodeType)
        {
            var hash = new Hash {Key = boardPosition.Zobrist, Depth = depth, NodeType = hashNodeType, Score = score};
            
            TranspositionTable.Add(hash);
        }

        public List<Tuple<string, ulong>> Divide(Board boardPosition, int depth)
        {
            var divides = new List<Tuple<string, ulong>>();
            
            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));

            ulong totalNodes = 0;
            
            Console.WriteLine($"Moves: {moveList.Count}");
            Console.WriteLine("");

            for (var i = 0; i < moveList.Count; i++)
            {
                var currentMove = moveList[i];

                var rootMoveString = GetPieceMoveAsString(currentMove);
                
                boardPosition.MakeMove(currentMove, false);

                var numberOfNodes = Perft(boardPosition, depth-1);

                boardPosition.UnMakeLastMove(false);

                totalNodes += numberOfNodes;

                divides.Add(new Tuple<string, ulong>(rootMoveString, numberOfNodes));
            }

            return divides;
        }

        public List<Tuple<string, ulong>> Divides(Board boardPosition)
        {
            var divides = new List<Tuple<string, ulong>>();

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));

            foreach (var move in moveList)
            {
                boardPosition.MakeMove(move, false);

                var branchMoves = MoveGeneration.CalculateAllMoves(boardPosition);
                
                divides.Add(new Tuple<string, ulong>(GetPieceMoveAsString(move), (ulong)branchMoves.Count));

                boardPosition.UnMakeLastMove(false);
            }

            return divides;
        }

        public List<Tuple<string, ulong>> Divides(Board boardPosition, int depth)
        {
            var divides = new List<Tuple<string, ulong>>();

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));

            foreach (var move in moveList)
            {
                boardPosition.MakeMove(move, false);

                var branchMoves = MoveGeneration.CalculateAllMoves(boardPosition);

                divides.Add(new Tuple<string, ulong>(GetPieceMoveAsString(move), (ulong)branchMoves.Count));

                boardPosition.UnMakeLastMove(false);
            }

            return divides;
        }

        public List<string> GetAllMoves(Board position)
        {
            var movesList = new List<string>();

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(position));
            
            foreach (var move in moveList)
            {
                var pieceMove = GetPieceMoveAsString(move);
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
                    throw new ArgumentException($"Unrecognised piece letter: {move.Type}");
            }

            var moveFrom = GetPostion(move.Position);
            var moveTo = GetPostion(move.Moves);

            return ($"{pieceLetter}{moveFrom}-{moveTo}");
        }

        private string GetPostion(ulong position)
        {
            var pos = BitboardOperations.GetSquareIndexFromBoardValue(position);

            var file = pos % 8;
            var rank = pos / 8;

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
                    throw new ArgumentException($"Unrecognised position letter: {file},{rank}");
            }

            return fileLetter + (rank + 1);
        }
    }
}
