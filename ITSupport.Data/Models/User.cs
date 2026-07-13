using System;

namespace ITSupport.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public int? CustomerId { get; set; }
        public int? EngineerId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Populated by ValidateUser (JOIN with Roles) — not stored in Users table
        public string RoleName { get; set; }

        public User() { }

        public User(int userId, string username, string email, string passwordHash,
                    int roleId, int? customerId, int? engineerId,
                    bool isActive, DateTime createdAt, DateTime? lastLoginAt)
        {
            UserId = userId;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
            CustomerId = customerId;
            EngineerId = engineerId;
            IsActive = isActive;
            CreatedAt = createdAt;
            LastLoginAt = lastLoginAt;
        }
    }
}
