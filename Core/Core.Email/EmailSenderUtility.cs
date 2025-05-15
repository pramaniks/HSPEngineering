using Core.Common;
using Core.Configuration;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;

namespace Core.Email
{
    public class MailMessage 
    {
        public ICollection<string> ToEmailAddressList { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class SendMailRequest 
    { 
        public MailMessage MailMessage { get; set; }
    }

    public class SendMailResponse 
    {
        public ValidationResults ValidationResults { get; set; }
    }

    public interface IEmailSenderUtility
    {
        Task<SendMailResponse> SendMailAsync(SendMailRequest Request);
    }
    public class EmailSenderUtility : IEmailSenderUtility
    {
        private string? _SmtpServerDetails;
        private SendMailRequest _Request;
        private SendMailResponse _Response;
        private string _SmtpServer;
        private int _SmtpPort;
        private string _SmtpUsername;
        private string _Smtppassword;

        public async Task<SendMailResponse> SendMailAsync(SendMailRequest Request)
        {
            _Request = Request;
            _Response = new SendMailResponse { ValidationResults = new ValidationResults() };

            assignMailServerDetails();
            parseSmtpServerDetails();
            await sendEmail();

            return _Response;
        }

        private void assignMailServerDetails()
        {
            if (!_Response.ValidationResults.IsValid) return;
           _SmtpServerDetails = ConfigurationUtility.ConfigurationManager["ApplicationEmailNotification:SmtpServerDetails"];
        }

        private void parseSmtpServerDetails()
        {
            if (!_Response.ValidationResults.IsValid) return;

            var smptServerDetails = _SmtpServerDetails.Split(';').ToList();
            _SmtpServer = smptServerDetails[0];
            _SmtpPort = Convert.ToInt32(smptServerDetails[1]);
            _SmtpUsername = smptServerDetails[2];
            _Smtppassword = smptServerDetails[3];
        }

        private async Task sendEmail()
        {
            if (!_Response.ValidationResults.IsValid) return;

            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                var toAddressList = _Request.MailMessage.ToEmailAddressList;
                toAddressList.ToList().ForEach(x => msg.To.Add(x));
                msg.From = new MailAddress(_SmtpUsername);                
                msg.Subject = _Request.MailMessage.Subject;
                msg.Body = _Request.MailMessage.Body;
                msg.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_SmtpServer)
                {
                    Port = _SmtpPort,
                    Credentials = new NetworkCredential(_SmtpUsername, _Smtppassword),
                    EnableSsl = true,
                };

                await smtpClient.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                var message = $"Exception occurred {ex.Message} {ex.StackTrace} {ex.InnerException?.StackTrace}";
                _Response.ValidationResults.AddResult(message);
            }
        }
    }
}