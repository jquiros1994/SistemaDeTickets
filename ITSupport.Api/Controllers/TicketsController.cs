using System.Web.Http;
using ITSupport.Api.DTOs;
using ITSupport.TicketReceiver.DTOs;
using ITSupport.TicketReceiver.Services;

namespace ITSupport.TicketReceiver.Controllers
{
    public class TicketsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Create(CreateTicketRequest request)
        {
            TicketService service = new TicketService();
            CreateTicketResponse response = service.CreateTicket(request);
            return Ok(response);
        }
    }
}