using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngineTests;

public class PerfT
{
    private readonly MoveGeneration _moveGeneration;
    private PieceMover _pieceMover;
    public bool UseHashing { get; set; } = true;

    public PerfT(MoveGeneration moveGeneration)
    {
        _moveGeneration = moveGeneration;
    }

    public int Perft(Board boardPosition, int depth)
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
                    var nds = hash.Score;

                    return nds;
                }
            }
        }

        var nodes = 0;

        if (depth == 0)
        {
            return 1;
        }

        var moveList = new List<PieceMove>(_moveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

        _pieceMover = new PieceMover(boardPosition);

        for (var i = 0; i < moveList.Count; i++)
        {
            var skipMove = false;

            if (moveList[i].SpecialMove == SpecialMoveType.KingCastle)
            {
                if (PieceChecking.IsKingInCheck(boardPosition, boardPosition.WhiteToMove)
                    || !_moveGeneration.ValidateKingsideCastlingMove(boardPosition))
                {
                    skipMove = true;
                }
            }
            else if (moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
            {
                if (PieceChecking.IsKingInCheck(boardPosition, boardPosition.WhiteToMove)
                    || !_moveGeneration.ValidateQueensideCastlingMove(boardPosition))
                {
                    skipMove = true;
                }
            }

            if (!skipMove)
            {
                _pieceMover.MakeMove(moveList[i]);

                if (MoveGeneration.WasLastMoveAllowed(boardPosition))
                {
                    nodes += Perft(boardPosition, depth - 1);
                }

                _pieceMover.UnMakeLastMove(false);
            }
        }

        if (UseHashing)
        {
            RecordHash(boardPosition, depth, nodes, HashNodeType.Exact);
        }

        return nodes;
    }

    private static void RecordHash(Board boardPosition, int depth, int score, HashNodeType hashNodeType)
    {
        var hash = new Hash { Key = boardPosition.Zobrist, Depth = depth, NodeType = hashNodeType, Score = score };

        TranspositionTable.Add(hash);
    }

    public List<Tuple<string, int>> Divide(Board boardPosition, int depth)
    {
        var divides = new List<Tuple<string, int>>();

        var moveList = new List<PieceMove>(_moveGeneration.CalculateAllMoves(boardPosition));

        int totalNodes = 0;

        Console.WriteLine($"Moves: {moveList.Count}");
        Console.WriteLine("");

        _pieceMover = new PieceMover(boardPosition);

        for (var i = 0; i < moveList.Count; i++)
        {
            var currentMove = moveList[i];

            var rootMoveString = GetPieceMoveAsString(currentMove);

            _pieceMover.MakeMove(currentMove);

            var numberOfNodes = Perft(boardPosition, depth-1);

            _pieceMover.UnMakeLastMove(false);

            totalNodes += numberOfNodes;

            divides.Add(new Tuple<string, int>(rootMoveString, numberOfNodes));
        }

        return divides;
    }

    public List<Tuple<string, int>> Divides(Board boardPosition)
    {
        var divides = new List<Tuple<string, int>>();

        var moveList = new List<PieceMove>(_moveGeneration.CalculateAllMoves(boardPosition));

        _pieceMover = new PieceMover(boardPosition);

        foreach (var move in moveList)
        {
            _pieceMover.MakeMove(move);

            var branchMoves = _moveGeneration.CalculateAllMoves(boardPosition);

            divides.Add(new Tuple<string, int>(GetPieceMoveAsString(move), branchMoves.Count));

            _pieceMover.UnMakeLastMove(false);
        }

        return divides;
    }

    public List<Tuple<string, ulong>> Divides(Board boardPosition, int depth)
    {
        var divides = new List<Tuple<string, ulong>>();

        var moveList = new List<PieceMove>(_moveGeneration.CalculateAllMoves(boardPosition));

        _pieceMover = new PieceMover(boardPosition);

        foreach (var move in moveList)
        {
            _pieceMover.MakeMove(move);

            var branchMoves = _moveGeneration.CalculateAllMoves(boardPosition);

            divides.Add(new Tuple<string, ulong>(GetPieceMoveAsString(move), (ulong)branchMoves.Count));

            _pieceMover.UnMakeLastMove(false);
        }

        return divides;
    }

    public List<string> GetAllMoves(Board position)
    {
        var movesList = new List<string>();

        var moveList = new List<PieceMove>(_moveGeneration.CalculateAllMoves(position));

        foreach (var move in moveList)
        {
            var pieceMove = GetPieceMoveAsString(move);
            movesList.Add(pieceMove);
        }

        return movesList;
    }

    private string GetPieceMoveAsString(PieceMove move)
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

    private static string GetPostion(ulong position)
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
