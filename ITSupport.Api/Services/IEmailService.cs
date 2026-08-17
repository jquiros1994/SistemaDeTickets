namespace ITSupport.TicketReceiver.Services
{
    // Defines the contract for sending email notifications.
    public interface IEmailService
    {
        void SendTicketCreatedEmail(int ticketId);

        void SendCriticalTicketEmail(int ticketId);
    }
}