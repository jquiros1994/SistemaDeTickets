using ITSupport.TicketReceiver.DTOs;
using ITSupport.TicketReceiver.Services;

namespace ITSupport.TicketReceiver.Observers
{
    // DP: Observer - Concrete Observer
    // Reacts when a ticket is created successfully
    // by requesting an email notification.
    public class EmailObserver : ITicketObserver
    {
        private readonly IEmailService _emailService;

        public EmailObserver()
        {
            _emailService = new SmtpEmailService();
        }

        public void Update(CreateTicketResponse response)
        {
            if (response == null || !response.Success)
            {
                return;
            }

            _emailService.SendTicketCreatedEmail(response.TicketId);
        }
    }
}