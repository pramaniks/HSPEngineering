using Azure.Messaging.ServiceBus;

namespace Core.ServiceBus
{
    public interface IServiceBusProvider
    {
        ServiceBusProcessor CreateProcessor(string QueueName, ServiceBusProcessorOptions? ServiceBusProcessorOptions = null);
        Task SendMessageAsync<T>(T Message, string QueueName);
        Task SendMessageBatchAsync<T>(ICollection<T> MessageList, string QueueName);
    }

    public class ServiceBusProcessorOptions
    {
        public bool AutoCompleteMessages { get; set; }
        public int MaxConcurrentCalls { get; set; }
        public TimeSpan MaxAutoLockRenewalDuration { get; set; }
    }

    public class ServiceBusProvider : IServiceBusProvider
    {
        private IAzureServiceBusProvider _AzureServiceBusProvider;
        public ServiceBusProvider()
        {
            _AzureServiceBusProvider = new AzureServiceBusProvider();
        }

        public async Task SendMessageAsync<T>(T Message, string QueueName)
        {
            await _AzureServiceBusProvider.SendMessageAsync(Message, QueueName);
        }

        public async Task SendMessageBatchAsync<T>(ICollection<T> MessageList, string QueueName)
        {
            await _AzureServiceBusProvider.SendMessageBatchAsync(MessageList, QueueName);
        }

        public ServiceBusProcessor CreateProcessor(string QueueName, ServiceBusProcessorOptions? ServiceBusProcessorOptions = null)
        {
            return _AzureServiceBusProvider.CreateProcessor(QueueName, ServiceBusProcessorOptions);
        }


    }
}
