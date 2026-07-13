using System.Web.Helpers;

namespace ITSupport.DAL
{
    public static class PasswordHelper
    {
        public static string Hash(string plainPassword)
        {
            return Crypto.HashPassword(plainPassword);
        }

        public static bool Verify(string plainPassword, string storedHash)
        {
            try { return Crypto.VerifyHashedPassword(storedHash, plainPassword); }
            catch { return false; }
        }
    }
}
