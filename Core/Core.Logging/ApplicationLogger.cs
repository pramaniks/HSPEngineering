using Core.Configuration;
using Core.Email;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Core.Logger
{

    public class LogMessageEntity : TableEntity
    {
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public string Identifier { get; set; }
        public string OperationName { get; set; }

        public LogMessageEntity()
        {

        }

        public LogMessageEntity(string ApplicationName)
        {
            PartitionKey = ApplicationName;
            RowKey = Guid.NewGuid().ToString();
        }
    }
    public class ApplicationLogger<T>
    {
        private static string _TableStorageConnectionString;
        private static string _TableName = "applog";
        private static CloudTable _Table;
        private static EmailSenderUtility _EmailClient;
        private static List<string> _ToEmailAddressList;
        private static string? _AppName;

        static ApplicationLogger()
        {            
            assignTableStorageConnectionString();
            assignTableStorageClient();
            _AppName = $"{Assembly.GetEntryAssembly().GetName().Name}";
        }
        private static void assignTableStorageConnectionString()
        {
            _TableStorageConnectionString = ConfigurationUtility.ConfigurationManager["LogTableStorageConnection"];
        }

        private static void assignTableStorageClient()
        {
            var storageAcc = CloudStorageAccount.Parse(_TableStorageConnectionString);
            var tblclient = storageAcc.CreateCloudTableClient(new TableClientConfiguration());
            _Table = tblclient.GetTableReference(_TableName);
        }

        private static async Task logMessage(string appName , string message, string logLevel, string operationName = "", string identitfier = "")
        {
            try
            {
                var logMessage = new LogMessageEntity(appName);
                logMessage.LogLevel = logLevel;
                logMessage.Message = message;
                logMessage.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                logMessage.OperationName = operationName;
                logMessage.Identifier = identitfier;
                var operation = TableOperation.Insert(logMessage);
                var result = await _Table.ExecuteAsync(operation);
            }
            catch (Exception) { }
        }
        public static async Task DebugAsync(string Message, string OperationName = "", string Identifier = "")
        {
            var message = $"{typeof(T).Assembly.GetName().Name}:{Message}";
            var partitionKey = $"{_AppName}_{DateTime.Now.ToString("yyyy-MM-dd")}";
            await logMessage(partitionKey, message, "Debug", OperationName, Identifier);
        }

        public static async Task InfoAsync(string Message, string OperationName = "", string Identifier = "")
        {
            var message = $"{typeof(T).Assembly.GetName().Name}:{Message}";
            var partitionKey = $"{_AppName}_{DateTime.Now.ToString("yyyy-MM-dd")}";
            await logMessage(partitionKey, message, "Info", OperationName, Identifier);
            
        }
        public static async Task ErrorAsync(string Message, string OperationName = "", string Identifier = "")
        {
            var message = $"{typeof(T).Assembly.GetName().Name}:{Message}";
            var partitionKey = $"{_AppName}_{DateTime.Now.ToString("yyyy-MM-dd")}";
            await logMessage(partitionKey, message, "Error", OperationName, Identifier);
            await sendEmail(message);
        }

        private static async Task sendEmail(string message)
        {
            try
            {
                _EmailClient = new EmailSenderUtility();
                _ToEmailAddressList = ConfigurationUtility.ConfigurationManager["ApplicationEmailNotification:RecipientList"].Split(';').ToList();
                await _EmailClient.SendMailAsync(new SendMailRequest { MailMessage = new MailMessage { Subject = $"{_AppName} : Application Error", Body = message, ToEmailAddressList = _ToEmailAddressList } });
            }
            catch(Exception) { }            
        }

        public static async Task CriticalAsync(string Message)
        {
            var message = $"{typeof(T).Assembly.GetName().Name}:{Message}";
            var partitionKey = $"{_AppName}_{DateTime.Now.ToString("yyyy-MM-dd")}";
            await logMessage(partitionKey, Message, "Critical");
        }

        public static async Task WarningAsync(string Message)
        {
            var message = $"{typeof(T).Assembly.GetName().Name}:{Message}";
            var partitionKey = $"{_AppName}_{DateTime.Now.ToString("yyyy-MM-dd")}";
            await logMessage(partitionKey, Message, "Warning");
        }

        public static async Task TraceAsync(string Message)
        {
            var message = $"{typeof(T).Assembly.GetName().Name}:{Message}";
            var partitionKey = $"{_AppName}_{DateTime.Now.ToString("yyyy-MM-dd")}";
            await logMessage(partitionKey, Message, "Trace");
        }
    }
}
