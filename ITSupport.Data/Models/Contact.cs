namespace ITSupport.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public int CustomerId { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Role { get; set; }
        public bool IsPrimary { get; set; }

        public Contact() { }

        public Contact(int contactId, int customerId, string contactName, string contactEmail,
                       string contactPhone, string role, bool isPrimary)
        {
            ContactId = contactId;
            CustomerId = customerId;
            ContactName = contactName;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            Role = role;
            IsPrimary = isPrimary;
        }
    }
}
