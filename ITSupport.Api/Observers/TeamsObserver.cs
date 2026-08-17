using ITSupport.TicketReceiver.DTOs;
using ITSupport.TicketReceiver.Services;

namespace ITSupport.TicketReceiver.Observers
{
    // DP: Observer - Concrete Observer
    // Sends a Teams notification when a critical ticket is created.
    public class TeamsObserver : ITicketObserver
    {
        private readonly ITeamsService _teamsService;

        public TeamsObserver()
        {
            _teamsService = new TeamsService();
        }

        public void Update(CreateTicketResponse response)
        {
            if (response == null || !response.Success)
            {
                return;
            }

            if (response.Severity == "Critical")
            {
                _teamsService.SendCriticalTicketMessage(response.TicketId);
            }
        }
    }
}