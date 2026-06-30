using System;

namespace ITSupport.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public int ProgramId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public Customer() { }

        public Customer(int customerId, string firstName, string lastName, string email,
                        string phoneNumber, string country, int programId, bool isActive, DateTime createdAt)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Country = country;
            ProgramId = programId;
            IsActive = isActive;
            CreatedAt = createdAt;
        }
    }
}
