using ITSupport.TicketReceiver.DTOs;

namespace ITSupport.TicketReceiver.Observers
{
    /// <summary>
    /// Observer interface
    /// Defines the contart that every observer must implment
    /// each Observer will be notified when a ticket is created successfully
    /// </summary>
    public interface ITicketObserver
    {
        void Update(CreateTicketResponse response);
    }
}