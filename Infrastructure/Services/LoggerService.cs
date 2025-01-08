using Application.Services;
using Serilog;
using SLog = Serilog.Log;

namespace Infrastructure.Services;

public class LoggerService : ILoggerService
{
    private static string outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}";
    private static string messageTemplate = "{Message} | Context: {@Context}";

    static LoggerService()
    {
        SLog.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File("logs/log.log",
                rollingInterval: RollingInterval.Month,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 5,
                fileSizeLimitBytes: 1024 * 1024 * 10, //10 MB
                outputTemplate: outputTemplate)
            .CreateLogger();
    }


    public void Log(string message, LogLevel logLvl, object? context = null, Exception? exception = null)
    {
        switch (logLvl)
        {
            case LogLevel.Information:
                SLog.Information(messageTemplate, message, context);
                break;

            case LogLevel.Warning:
                SLog.Warning(messageTemplate, message, context);
                break;

            case LogLevel.Error:
                SLog.Error(exception, messageTemplate, message, context);
                break;

            case LogLevel.Critical:
                SLog.Fatal(exception, messageTemplate, message, context);
                break;

            case LogLevel.Trace:
                SLog.Verbose(exception, messageTemplate, message, context);
                break;

            case LogLevel.Debug:
                SLog.Debug(exception, messageTemplate, message, context);
                break;

            default:
                SLog.Warning("Unsupported log level: {LogLevel}. Logging as Information.", logLvl);
                SLog.Information(message);
                break;
        }
    }
}