using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Core.Configuration
{
    public class ConfigurationUtility
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
            var azureAppConfigEndpoint = Environment.GetEnvironmentVariable("AzureAppConfigEndpoint");
            if (!string.IsNullOrEmpty(azureAppConfigEndpoint))
            {
                builder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(azureAppConfigEndpoint), new DefaultAzureCredential())
                    .ConfigureKeyVault(options =>
                    {
                        options.SetCredential(new DefaultAzureCredential());
                    });
                });
            }
            else
            {
                builder.AddJsonFile("appsettings.json");
            }                                  
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
