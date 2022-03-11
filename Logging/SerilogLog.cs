using Serilog;
using Serilog.Events;

namespace Logging;

public sealed class SerilogLog : ILog
{
    private readonly ILogger _logger;

    public SerilogLog(string fileName)
    {
        var loggerConfiguration = new LoggerConfiguration().WriteTo.File(fileName);

        _logger = loggerConfiguration.CreateLogger();
    }

    public bool IsVerboseEnabled => _logger.IsEnabled(LogEventLevel.Verbose);
    public void Verbose(string message, params object[] args) => _logger.Verbose(message, args);
    public void Verbose(Exception exception, string message) => _logger.Verbose(exception, message);

    public bool IsDebugEnabled => _logger.IsEnabled(LogEventLevel.Debug);
    public void Debug(string message, params object[] args) => _logger.Debug(message, args);
    public void Debug(Exception exception, string message) => _logger.Debug(exception, message);

    public bool IsInfoEnabled => _logger.IsEnabled(LogEventLevel.Information);
    public void Info(string message, params object[] args) => _logger.Information(message, args);
    public void Info(Exception exception, string message) => _logger.Information(exception, message);

    public bool IsWarnEnabled => _logger.IsEnabled(LogEventLevel.Warning);
    public void Warn(string message, params object[] args) => _logger.Warning(message, args);
    public void Warn(Exception exception, string message) => _logger.Warning(exception, message);

    public bool IsErrorEnabled => _logger.IsEnabled(LogEventLevel.Error);
    public void Error(string message, params object[] args) => _logger.Error(message, args);
    public void Error(Exception exception, string message) => _logger.Error(exception, message);

    public bool IsFatalEnabled => _logger.IsEnabled(LogEventLevel.Fatal);
    public void Fatal(string message, params object[] args) => _logger.Fatal(message, args);
    public void Fatal(Exception exception, string message) => _logger.Fatal(exception, message);
}
