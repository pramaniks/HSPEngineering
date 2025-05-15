using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Core.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using System.Text;
using System.Text.Json;

namespace Core.ServiceBus
{
    internal interface IAzureServiceBusProvider
    {
        ServiceBusProcessor CreateProcessor(string QueueName,ServiceBusProcessorOptions? ServiceBusProcessorOptions = null);
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
            _ServiceBusConnectionEndpoint = ConfigurationUtility.ConfigurationManager["AzureServiceBusConnection:ConnectionEndpoint"];
            _ = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
                RetryOptions = new ServiceBusRetryOptions { MaxRetries = 5, Delay = TimeSpan.FromSeconds(3)}
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
                //var connectionStringBuilder = new ServiceBusConnectionStringBuilder(_ServiceBusConnectionEndpoint);
                //connectionStringBuilder.EntityPath = $"{connectionStringBuilder.Endpoint}/{QueueName}";
                //connectionStringBuilder.TransportType = TransportType.Amqp;
                //var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(connectionStringBuilder.SasKeyName, connectionStringBuilder.SasKey,
                //    TimeSpan.FromMinutes(10),TokenScope.Entity);

                //var token = await tokenProvider.GetTokenAsync(connectionStringBuilder.EntityPath, TimeSpan.FromMinutes(1));
                //var sasToken = token.TokenValue;
                //connectionStringBuilder.SasToken = sasToken;


                //var client = new QueueClient(connectionStringBuilder, ReceiveMode.PeekLock);                
                //var messageBody = JsonSerializer.Serialize(Message);

                //var message = new Message(Encoding.UTF8.GetBytes(messageBody))
                //{
                //    MessageId = Guid.NewGuid().ToString(),
                //    ContentType = "application/json"
                //};

                //await client.SendAsync(message);


                var queueExists = await _ServiceBusAdministrationClient.QueueExistsAsync(QueueName);
                if (!queueExists) await _ServiceBusAdministrationClient.CreateQueueAsync(QueueName);
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

        public ServiceBusProcessor CreateProcessor(string QueueName, ServiceBusProcessorOptions? ServiceBusProcessorOptions = null)
        {
            var processor = ServiceBusProcessorOptions == null ? _Client.CreateProcessor(QueueName, new Azure.Messaging.ServiceBus.ServiceBusProcessorOptions()):
                _Client.CreateProcessor(QueueName, new Azure.Messaging.ServiceBus.ServiceBusProcessorOptions
            {
                AutoCompleteMessages = ServiceBusProcessorOptions.AutoCompleteMessages,
                MaxConcurrentCalls = ServiceBusProcessorOptions.MaxConcurrentCalls,
                MaxAutoLockRenewalDuration = ServiceBusProcessorOptions.MaxAutoLockRenewalDuration
            });
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
