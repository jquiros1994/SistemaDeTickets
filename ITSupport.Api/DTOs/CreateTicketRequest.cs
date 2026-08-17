namespace ITSupport.Api.DTOs
{
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Application { get; set; }
        public int PriorityId { get; set; }
        public int ContactId { get; set; }
        public int ProgramId { get; set; }
        public string Country { get; set; }
    }
}