using System;

namespace Logging;

public interface ILog
{
    bool IsVerboseEnabled { get; }

    /// <summary>
    /// Log extremely detailed information - useful for diagnostics.
    /// </summary>
    void Verbose(string message, params object[] args);

    /// <summary>
    /// Log extremely detailed information - useful for diagnostics.
    /// </summary>
    void Verbose(Exception exception, string message);

    bool IsDebugEnabled { get; }

    /// <summary>
    /// Log internal/detailed information about what is going on in the code.
    /// </summary>
    void Debug(string message, params object[] args);

    /// <summary>
    /// Log internal/detailed information about what is going on in the code.
    /// </summary>
    void Debug(Exception exception, string message);

    bool IsInfoEnabled { get; }

    /// <summary>
    /// Log about what is going on in the general running of the code.
    /// </summary>
    void Info(string message, params object[] args);

    /// <summary>
    /// Log about what is going on in the general running of the code.
    /// </summary>
    void Info(Exception exception, string message);

    bool IsWarnEnabled { get; }

    /// <summary>
    /// Log about something that has gone wrong during an operation, but has been automatically handled, allowing the operation to continue.
    /// </summary>
    void Warn(string message, params object[] args);

    /// <summary>
    /// Log about something that has gone wrong during an operation, but has been automatically handled, allowing the operation to continue.
    /// </summary>
    void Warn(Exception exception, string message);

    bool IsErrorEnabled { get; }

    /// <summary>
    /// Log about something that has gone wrong that prevents an operation from completing.
    /// </summary>
    void Error(string message, params object[] args);

    /// <summary>
    /// Log about something that has gone wrong that prevents an operation from completing.
    /// </summary>
    void Error(Exception exception, string message);

    bool IsFatalEnabled { get; }

    /// <summary>
    /// Log about something catastrophic and unrecoverable that has gone wrong.
    /// </summary>
    void Fatal(string message, params object[] args);

    /// <summary>
    /// Log about something catastrophic and unrecoverable that has gone wrong.
    /// </summary>
    void Fatal(Exception exception, string message);
}
