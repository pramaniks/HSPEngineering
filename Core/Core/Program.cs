using Core.Cache;
using Core.Configuration;
using Core.Email;
using Core.Logger;
using Core.ServiceBus;
using System.Security.Cryptography;


//var config = new AppConfiguration();

//Console.WriteLine(config["TestApp:Settings:Message"] ?? "Hello world!");
//Console.WriteLine(ConfigurationUtility.ConfigurationManager["TestApp:Settings:Message"] ?? "Hello world!");

//try
//{
//    await ApplicationLogger<Program>.DebugAsync("Debug Message");
//    await ApplicationLogger<Program>.InfoAsync("Info Message");
//}
//catch (Exception ex)
//{

//}


//var cache = new ApplicationCache();
//await cache.SetDataAsync("Nilesh", new Employee { FirstName = "Nilesh 1", LastName = "Shelke" }, DateTimeOffset.Now.AddMinutes(2));
//var employee = await cache.GetDataAsync<Employee>("Nilesh");
//Console.WriteLine($"Value of Key Nilesh is {employee.FirstName} {employee.LastName}");

//var filepath = @"C:\Nuget\MS_Sign.jpg";
//string hash = ""; 
//using (var sha256 = MD5.Create())
//{
//    using (var stream = File.OpenRead(filepath))
//    {
//        byte[] hashBytes = sha256.ComputeHash(stream);
//        hash = Convert.ToBase64String(hashBytes);
//    }
//}

//var emailsender = new EmailSenderUtility();
//var _ToEmailAddressList = ConfigurationUtility.ConfigurationManager["ApplicationEmailNotification:RecipientList"].Split(';').ToList();
//var response =  await emailsender.SendMailAsync(new SendMailRequest
//{
//    MailMessage = new MailMessage
//    {
//        ToEmailAddressList = _ToEmailAddressList,
//        Subject = "This is a test email",
//        Body = "Test Body"
//    }
//});

var queueName = ConfigurationUtility.ConfigurationManager["AzureServiceBusConnection:AccountingRecordSAPQueueName"];
var serviceBusProvider = new ServiceBusProvider();
await serviceBusProvider.SendMessageAsync("Hello There!", queueName);

//var processor = serviceBusProvider.CreateProcessor(queueName);
//processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
//processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
//await processor.StartProcessingAsync();
//Console.WriteLine("Wait for a minute and then press any key to end the processing");
//Console.ReadKey();

//// stop processing 
//Console.WriteLine("\nStopping the receiver...");
//await processor.StopProcessingAsync();
//Console.WriteLine("Stopped receiving messages");


//static Task Processor_ProcessErrorAsync(Azure.Messaging.ServiceBus.ProcessErrorEventArgs arg)
//{
//    Console.WriteLine(arg.Exception.ToString());
//    return Task.CompletedTask;
//}

//static async Task Processor_ProcessMessageAsync(Azure.Messaging.ServiceBus.ProcessMessageEventArgs arg)
//{
//    string body = arg.Message.Body.ToString();
//    Console.WriteLine($"Received: {body}");

//    await arg.CompleteMessageAsync(arg.Message);
//}