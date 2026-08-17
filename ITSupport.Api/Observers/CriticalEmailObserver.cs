using ITSupport.TicketReceiver.DTOs;
using ITSupport.TicketReceiver.Services;

namespace ITSupport.TicketReceiver.Observers
{
    // DP: Observer - Concrete Observer
    // Sends an additional email notification when a critical ticket is created.
    public class CriticalEmailObserver : ITicketObserver
    {
        private readonly IEmailService _emailService;

        public CriticalEmailObserver()
        {
            _emailService = new SmtpEmailService();
        }

        public void Update(CreateTicketResponse response)
        {
            if (response == null || !response.Success)
            {
                return;
            }

            if (response.PriorityId != 1)
            {
                return;
            }

            _emailService.SendCriticalTicketEmail(response.TicketId);
        }
    }
}