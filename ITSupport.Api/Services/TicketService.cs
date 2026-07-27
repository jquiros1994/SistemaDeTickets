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
            // Registramos los observers
            Attach(new EmailObserver());
        }

        public void Attach(ITicketObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(ITicketObserver observer)
        {
            _observers.Remove(observer);
        }

        private void Notify(CreateTicketResponse response)
        {
            foreach (ITicketObserver observer in _observers)
            {
                observer.Update(response);
            }
        }

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
                TicketId = result.CaseId
            };

            // Solo notificamos si el ticket se creó correctamente
            if (response.Success)
            {
                Notify(response);
            }

            return response;
        }
    }
}