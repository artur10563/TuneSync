namespace Application.Services;

public interface ILoggerService
{
    void Log(string message, LogLevel logLvl, object? context = null, Exception? exception = null);
}

public enum LogLevel
{
    Information,
    Warning,
    Error,
    Critical,
    Trace,
    Debug
};