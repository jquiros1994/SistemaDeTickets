using System.Web.Http;

namespace ITSupport.Api.Controllers
{
    [RoutePrefix("api/tickets")]
    public class TicketsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Test()
        {
            return Ok("Ticket Receiver funcionando correctamente.");
        }
    }
}