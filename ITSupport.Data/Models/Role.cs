using System;

namespace ITSupport.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public Role() { }

        public Role(int roleId, string roleName, string description)
        {
            RoleId = roleId;
            RoleName = roleName;
            Description = description;
        }
    }
}
