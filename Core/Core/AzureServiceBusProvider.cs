using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Core.Configuration;
using System.Text;
using System.Text.Json;

namespace Core.ServiceBus
{
    internal interface IAzureServiceBusProvider
    {
        ServiceBusProcessor CreateProcessor(string QueueName);
        void Dispose();
        Task SendMessageAsync<T>(T Message, string QueueName);
        Task SendMessageBatchAsync<T>(ICollection<T> MessageList, string QueueName);
    }

    internal class AzureServiceBusProvider : IDisposable, IAzureServiceBusProvider
    {
        ServiceBusClient _Client;
        private ServiceBusAdministrationClient _ServiceBusAdministrationClient;
        ServiceBusSender _Sender;
        private string? _ServiceBusConnectionEndpoint;

        public AzureServiceBusProvider()
        {
            _ServiceBusConnectionEndpoint = AppUtility.ConfigurationManager["AzureServiceBusConnectionString"];
            _ = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            _Client = new ServiceBusClient(_ServiceBusConnectionEndpoint);

            _ServiceBusAdministrationClient = new ServiceBusAdministrationClient(_ServiceBusConnectionEndpoint);
        }

        public async void Dispose()
        {
            if (_Sender != null) await _Sender.DisposeAsync();
            if (_Client != null) await _Client.DisposeAsync();
        }

        public async Task SendMessageAsync<T>(T Message, string QueueName)
        {
            try
            {
                var queueExists = await _ServiceBusAdministrationClient.QueueExistsAsync(QueueName);                    
                if(!queueExists) await _ServiceBusAdministrationClient.CreateQueueAsync(QueueName);
                 _Sender = _Client.CreateSender(QueueName);
                var messageBody = JsonSerializer.Serialize(Message);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "application/json"
                };
                await _Sender.SendMessageAsync(message);
            }
            finally
            {
                if (_Sender != null) await _Sender.DisposeAsync();
            }
        }

        public async Task SendMessageBatchAsync<T>(ICollection<T> MessageList, string QueueName)
        {
            try
            {
                var queueExists = await _ServiceBusAdministrationClient.QueueExistsAsync(QueueName);
                if (!queueExists) await _ServiceBusAdministrationClient.CreateQueueAsync(QueueName);
                _Sender = _Client.CreateSender(QueueName);
                using ServiceBusMessageBatch messageBatch = await _Sender.CreateMessageBatchAsync();

                foreach (var message in MessageList)
                {
                    var messageBody = JsonSerializer.Serialize(message);
                    if (!messageBatch.TryAddMessage(new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody)) { MessageId = Guid.NewGuid().ToString(), ContentType = "application/json" }))
                    {
                        throw new Exception($"The message {messageBody} is too large to fit in the batch.");
                    }
                }
                await _Sender.SendMessagesAsync(messageBatch);
            }
            finally
            {
                if (_Sender != null) await _Sender.DisposeAsync();
            }
        }

        public ServiceBusProcessor CreateProcessor(string QueueName)
        {
            var processor = _Client.CreateProcessor(QueueName, new ServiceBusProcessorOptions());
            return processor;
        }

        //public async Task MessageHandler(ProcessMessageEventArgs args)
        //{
        //    string body = args.Message.Body.ToString();
        //    Console.WriteLine($"Received: {body}");

        //    if (func != null) await func(body); 
        //    await args.CompleteMessageAsync(args.Message);
        //}

        //public Task ErrorHandler(ProcessErrorEventArgs args)
        //{
        //    Console.WriteLine(args.Exception.ToString());
        //    return Task.CompletedTask;
        //}
    }
}
