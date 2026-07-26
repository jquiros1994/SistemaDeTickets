using ITSupport.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITSupport.TicketReceiver.DTOs;
using ITSupport.Business;
using ITSupport.Business.Results;

namespace ITSupport.TicketReceiver.Services
{
    public class TicketService
    {
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

            return new CreateTicketResponse
            {
                Success = result.Success,
                Message = result.Message,
                TicketId = result.CaseId
            };
        }
    }
}