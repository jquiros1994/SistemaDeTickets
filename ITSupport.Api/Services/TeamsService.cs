using System.Diagnostics;

namespace ITSupport.TicketReceiver.Services
{
    // Responsible for sending notifications to Microsoft Teams.
    public class TeamsService : ITeamsService
    {
        public void SendCriticalTicketMessage(int ticketId)
        {
            string message =
                $"CRITICAL TICKET CREATED - Ticket ID: {ticketId}";

            Debug.WriteLine($"[TEAMS] {message}");
        }
    }
}