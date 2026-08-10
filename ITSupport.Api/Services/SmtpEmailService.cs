using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ITSupport.TicketReceiver.Services
{
    // SOLID: SRP
    // This class is responsible only for sending emails through SMTP.
    public class SmtpEmailService : IEmailService
    {
        public void SendTicketCreatedEmail(int ticketId)
        {
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];

            // The password is read from a Windows environment variable
            // so it is not stored in the source code or Web.config.
            string smtpPassword = Environment.GetEnvironmentVariable("ITSupport_SmtpPassword");

            string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
            string emailTo = ConfigurationManager.AppSettings["EmailTo"];

            if (string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new InvalidOperationException(
                    "The environment variable 'ITSupport_SmtpPassword' was not found.");
            }

            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(
                    smtpUser,
                    smtpPassword
                );

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(emailFrom),
                    Subject = $"New Ticket Created - #{ticketId}",
                    Body =
                        $"A new support ticket has been created successfully.\n\n" +
                        $"Ticket ID: {ticketId}\n\n" +
                        $"This email was generated automatically by ITSupport TicketReceiver.",
                    IsBodyHtml = false
                };

                message.To.Add(emailTo);

                smtpClient.Send(message);
            }
        }
    }
}