namespace ResourceLoading;

public class OpeningBook : IOpeningBook
{
    private List<Opening> _openings;

    private int _plyCount;

    private readonly Random _rand = new Random();

    public OpeningBook(string bookNameFilePath)
    {
        _openings = LoadOpeningBook(bookNameFilePath);

        FilePath = bookNameFilePath;
    }

    private List<Opening> LoadOpeningBook(string bookNamePath)
    {
        _plyCount = 0;

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
        for (var i = _openings.Count - 1; i >= 0; i--)
        {
            var currentOpening = _openings[i];

            if (_plyCount >= currentOpening.Moves.Length)
                _openings.RemoveAt(i);
        }

        if (_openings.Count <= 0) return string.Empty;

        var pos = _rand.Next(_openings.Count);

        var move = _openings[pos].Moves[_plyCount];

        return move;
    }

    // Tell the opening book that this move was made
    public void RegisterMadeMove(string uciMove)
    {
        for (var i = _openings.Count-1; i >= 0; i--)
        {
            var currentOpening = _openings[i];

            if (_plyCount >= currentOpening.Moves.Length)
            {
                _openings.RemoveAt(i);
            }
            else
            {
                if (currentOpening.Moves[_plyCount] != uciMove)
                {
                    _openings.RemoveAt(i);
                }
            }
        }

        _plyCount++;
    }

    public void ResetBook()
    {
        _plyCount = 0;
        _openings = LoadOpeningBook(FilePath);
    }

    public string FilePath { get; }
}

