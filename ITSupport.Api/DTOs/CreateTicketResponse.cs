namespace ITSupport.TicketReceiver.DTOs
{
    public class CreateTicketResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int TicketId { get; set; }

        public string Severity { get; set; }
        public int PriorityId { get; set; }
    }
}