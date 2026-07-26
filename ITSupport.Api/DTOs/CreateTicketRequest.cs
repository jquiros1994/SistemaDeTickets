namespace ITSupport.Api.DTOs
{
    public class CreateTicketRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Application { get; set; }

        public string Severity { get; set; }
    }
}