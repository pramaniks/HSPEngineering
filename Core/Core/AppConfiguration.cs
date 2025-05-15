using Microsoft.Extensions.Configuration;

namespace Core.Configuration
{
    public class AppUtility
    {
        private static AppConfiguration _configurationManager = new AppConfiguration();
        public static AppConfiguration ConfigurationManager => _configurationManager;
    }
    public interface IAppConfiguration
    {
        string? this[string key] { get; }
    }

    public class AppConfiguration : IAppConfiguration
    {
        private IConfiguration _Configuration;

        public AppConfiguration()
        {
            var builder = new ConfigurationBuilder();

            var connectionstring = Environment.GetEnvironmentVariable("AppConfigConnectionString");
            if (!string.IsNullOrEmpty(connectionstring)) builder.AddAzureAppConfiguration(connectionstring);
            builder.AddJsonFile("appsettings.json");
            _Configuration = builder.Build();
        }

        public string? this[string key]
        {
            get
            {
                if (_Configuration == null) _ = new AppConfiguration();
                return _Configuration[key];
            }
        }
    }
}
