using ITSupport.Api.DTOs;
using ITSupport.Business;
using ITSupport.Business.Results;
using ITSupport.TicketReceiver.DTOs;
using ITSupport.TicketReceiver.Observers;
using System.Collections.Generic;

namespace ITSupport.TicketReceiver.Services
{
    public class TicketService
    {
        private readonly List<ITicketObserver> _observers = new List<ITicketObserver>();

        public TicketService()
        {
            // Register observers
            Attach(new EmailObserver());
            Attach(new CriticalEmailObserver());
        }

        /// <summary>
        /// Registers a new observer that will receive notifications
        /// after a ticket is created.
        /// </summary>
        public void Attach(ITicketObserver observer)
        {
            _observers.Add(observer);
        }

        /// <summary>
        /// Removes an observer from the notification list.
        /// </summary>
        public void Detach(ITicketObserver observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>
        /// Notifies all registered observers that a ticket
        /// has been created successfully.
        /// </summary>
        private void Notify(CreateTicketResponse response)
        {
            foreach (ITicketObserver observer in _observers)
            {
                observer.Update(response);
            }
        }

        /// <summary>
        /// Creates a new support ticket using the business layer.
        /// If the operation succeeds, all registered observers
        /// are notified.
        /// </summary>
        public CreateTicketResponse CreateTicket(CreateTicketRequest request)
        {
            CaseBusiness business = new CaseBusiness();

            CaseCreationResult result = business.CreateExternalCase(
                request.Title,
                request.Description,
                request.PriorityId,
                request.ContactId,
                request.ProgramId,
                request.Country,
                1,
                "Portal");

            CreateTicketResponse response = new CreateTicketResponse
            {
                Success = result.Success,
                Message = result.Message,
                TicketId = result.CaseId,
                Severity = request.Severity
            };

            // Notify observers only if the ticket was created successfully
            if (response.Success)
            {
                Notify(response);
            }

            return response;
        }
    }
}