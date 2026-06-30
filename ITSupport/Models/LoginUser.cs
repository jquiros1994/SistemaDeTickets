namespace ITSupport.Models
{
    public class LoginUser
    {
        public int    UserId     { get; set; }
        public string Username   { get; set; }
        public string Email      { get; set; }
        public string RoleName   { get; set; }
        public int?   CustomerId { get; set; }
        public int?   EngineerId { get; set; }
    }
}
