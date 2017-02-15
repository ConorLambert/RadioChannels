using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using System;

namespace RadioChannels.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            /*
            var myMessage = new SendGridMessage();
            EmailAddress destinationEmailAddress = new EmailAddress(message.Destination);
            myMessage.AddTo(destinationEmailAddress);
            EmailAddress fromEmailAddress = new EmailAddress("Joe@contoso.com", "Joe S.");
            myMessage.From = fromEmailAddress; // new System.Net.Mail.MailAddress(fromEmailAddress);
            myMessage.Subject = message.Subject;
            myMessage.PlainTextContent = message.Body;
            myMessage.HtmlContent = message.Body;

            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["mailAccount"],
                       ConfigurationManager.AppSettings["mailPassword"]
                       );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            if (transportWeb != null)
            {
                await transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                await Task.FromResult(0);
            }
            */

            var apiKey = "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"; // Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Example User");
            var subject = message.Subject;
            var to = new EmailAddress(message.Destination);
            var plainTextContent = message.Body;
            var htmlContent = message.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}