using System.Collections.Generic;
using System.Text.RegularExpressions;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
    public class ConfigurationBusiness
    {
        private readonly UserRepository _users = new UserRepository();

        public User GetById(int userId)
        {
            return _users.GetById(userId);
        }

        public OperationResult UpdateProfile(int userId, string username, string email, string newPassword)
        {
            var errors = Validate(username, email, newPassword);
            if (errors.Count > 0)
                return OperationResult.Fail(errors);

            var user = _users.GetById(userId);
            if (user == null)
                return OperationResult.Fail("User not found.");

            var byUsername = _users.GetByUsername(username.Trim());
            if (byUsername != null && byUsername.UserId != userId)
                return OperationResult.Fail("That username is already taken.");

            var byEmail = _users.GetByEmail(email.Trim());
            if (byEmail != null && byEmail.UserId != userId)
                return OperationResult.Fail("That email is already in use.");

            user.Username = username.Trim();
            user.Email = email.Trim();
            if (!string.IsNullOrEmpty(newPassword))
                user.PasswordHash = PasswordHelper.Hash(newPassword);

            _users.Update(user);
            return OperationResult.Ok(user.UserId);
        }

        private List<string> Validate(string username, string email, string newPassword)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Username is required.");

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("A valid email is required.");

            if (!string.IsNullOrEmpty(newPassword) && newPassword.Length < 8)
                errors.Add("New password must be at least 8 characters.");

            return errors;
        }
    }
}
