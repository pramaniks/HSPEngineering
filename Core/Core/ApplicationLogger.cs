using Microsoft.Extensions.Logging;
using Serilog;

namespace Core.Logger
{
    public class ApplicationLogger<T>
    {
        private static ILogger<T> _Logger;
        static ApplicationLogger()
        {
            var serilogLogger = new LoggerConfiguration()
                                    .MinimumLevel.Verbose()
                                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                                    .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
                                    .CreateLogger();
            var loggerFactory = new LoggerFactory().AddSerilog(serilogLogger);
            _Logger = loggerFactory.CreateLogger<T>();
        }
        public static async Task DebugAsync(string Message)
        {
            await Task.Run(() => { _Logger.LogDebug(Message); });
        }

        public static async Task InfoAsync(string Message)
        {
            await Task.Run(() => { _Logger.LogInformation(Message); });
        }
        public static async Task ErrorAsync(string Message)
        {
            await Task.Run(() => { _Logger.LogError(Message); });
        }

        public static async Task CriticalAsync(string Message)
        {
            await Task.Run(() => { _Logger.LogCritical(Message); });
        }

        public static async Task WarningAsync(string Message)
        {
            await Task.Run(() => { _Logger.LogWarning(Message); });
        }

        public static async Task TraceAsync(string Message)
        {
            await Task.Run(() => { _Logger.LogTrace(Message); });
        }
    }
}
