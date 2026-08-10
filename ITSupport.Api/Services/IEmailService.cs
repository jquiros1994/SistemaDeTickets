namespace ITSupport.TicketReceiver.Services
{
    // SOLID: DIP
    // Defines the contract for sending email notifications.
    public interface IEmailService
    {
        void SendTicketCreatedEmail(int ticketId);
    }
}