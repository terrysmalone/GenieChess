using System.Runtime.Serialization;

namespace ChessEngine.Exceptions;

[Serializable]
public class ChessBoardException : Exception
{
    public ChessBoardException()
        : base() { }

    public ChessBoardException(string message)
        : base(message) { }

    public ChessBoardException(string format, params object[] args)
        : base(string.Format(format, args)) { }

    public ChessBoardException(string message, Exception innerException)
        : base(message, innerException) { }

    public ChessBoardException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }

    protected ChessBoardException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

