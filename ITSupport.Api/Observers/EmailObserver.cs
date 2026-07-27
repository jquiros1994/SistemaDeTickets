using System.Diagnostics;
using ITSupport.TicketReceiver.DTOs;

namespace ITSupport.TicketReceiver.Observers
{
    /// <summary>
    /// concrete Observer
    /// Receives notifications from the TicketService after a ticket
    /// has been created successufully.
    /// 
    /// For now just writes a message to confirm the process
    /// </summary>
    public class EmailObserver : ITicketObserver
    {
        public void Update(CreateTicketResponse response)
        {
            Debug.WriteLine(
                $"[EMAIL] Ticket #{response.TicketId} creado correctamente. Enviando correo...");
        }

    }
}