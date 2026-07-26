using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITSupport.TicketReceiver.DTOs
{
    public class CreateTicketResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int TicketId { get; set; }
    }
}