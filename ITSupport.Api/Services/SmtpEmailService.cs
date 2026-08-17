using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ITSupport.TicketReceiver.Services
{
    // Responsible for sending email notifications through SMTP.
    public class SmtpEmailService : IEmailService
    {
        public void SendTicketCreatedEmail(int ticketId)
        {
            string recipient = ConfigurationManager.AppSettings["EmailTo"];

            SendEmail(
                "New Ticket Created",
                $"A new support ticket has been created successfully.\n\n" +
                $"Ticket ID: {ticketId}\n\n" +
                $"This email was generated automatically by ITSupport TicketReceiver.",
                recipient
            );
        }

        public void SendCriticalTicketEmail(int ticketId)
        {
            string recipients = ConfigurationManager.AppSettings["CriticalEmailTo"];

            SendEmail(
                $"CRITICAL TICKET - #{ticketId}",
                $"A CRITICAL support ticket has been created.\n\n" +
                $"Ticket ID: {ticketId}\n" +
                $"Severity: Critical\n\n" +
                $"This notification requires immediate attention.",
                recipients
            );
        }

        private void SendEmail(string subject, string body, string recipients)
        {
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];

            string smtpPassword =
                Environment.GetEnvironmentVariable("ITSupport_SmtpPassword");

            string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];

            if (string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new InvalidOperationException(
                    "The environment variable 'ITSupport_SmtpPassword' was not found."
                );
            }

            if (string.IsNullOrWhiteSpace(recipients))
            {
                throw new InvalidOperationException(
                    "No email recipients were configured."
                );
            }

            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(
                    smtpUser,
                    smtpPassword
                );

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(emailFrom);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = false;

                    // Allows multiple recipients separated by ';'
                    string[] emailList = recipients.Split(';');

                    foreach (string email in emailList)
                    {
                        string trimmedEmail = email.Trim();

                        if (!string.IsNullOrWhiteSpace(trimmedEmail))
                        {
                            message.To.Add(trimmedEmail);
                        }
                    }

                    smtpClient.Send(message);
                }
            }
        }
    }
}