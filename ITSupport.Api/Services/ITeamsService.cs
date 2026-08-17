namespace ITSupport.TicketReceiver.Services
{
    // Defines the contract for sending notifications through Microsoft Teams.
    public interface ITeamsService
    {
        void SendCriticalTicketMessage(int ticketId);
    }
}