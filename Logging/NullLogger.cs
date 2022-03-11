namespace Logging;

public sealed class NullLogger : ILog
{
    public bool IsVerboseEnabled => false;

    public void Verbose(string message, params object[] args)
    { }

    public void Verbose(Exception exception, string message)
    { }

    public bool IsDebugEnabled => false;

    public void Debug(string message, params object[] args)
    { }

    public void Debug(Exception exception, string message)
    { }

    public bool IsInfoEnabled => false;

    public void Info(string message, params object[] args)
    { }

    public void Info(Exception exception, string message)
    { }

    public bool IsWarnEnabled => false;

    public void Warn(string message, params object[] args)
    { }

    public void Warn(Exception exception, string message)
    { }

    public bool IsErrorEnabled => false;

    public void Error(string message, params object[] args)
    { }

    public void Error(Exception exception, string message)
    { }

    public bool IsFatalEnabled => false;

    public void Fatal(string message, params object[] args)
    { }

    public void Fatal(Exception exception, string message)
    { }
}