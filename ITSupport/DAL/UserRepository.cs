using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class UserRepository
    {
        public List<User> GetAll()
        {
            var list = new List<User>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT UserId, Username, Email, PasswordHash, RoleId, CustomerId, EngineerId, IsActive, CreatedAt, LastLoginAt FROM Users", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public User GetById(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT UserId, Username, Email, PasswordHash, RoleId, CustomerId, EngineerId, IsActive, CreatedAt, LastLoginAt FROM Users WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public User GetByUsername(string username)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT UserId, Username, Email, PasswordHash, RoleId, CustomerId, EngineerId, IsActive, CreatedAt, LastLoginAt FROM Users WHERE Username = @Username", conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public User GetByEmail(string email)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT UserId, Username, Email, PasswordHash, RoleId, CustomerId, EngineerId, IsActive, CreatedAt, LastLoginAt FROM Users WHERE Email = @Email", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(User user)
        {
            const string sql = @"
                INSERT INTO Users (Username, Email, PasswordHash, RoleId, CustomerId, EngineerId, IsActive)
                OUTPUT INSERTED.UserId
                VALUES (@Username, @Email, @PasswordHash, @RoleId, @CustomerId, @EngineerId, @IsActive)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@CustomerId", (object)user.CustomerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EngineerId", (object)user.EngineerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(User user)
        {
            const string sql = @"
                UPDATE Users
                SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash,
                    RoleId = @RoleId, CustomerId = @CustomerId, EngineerId = @EngineerId,
                    IsActive = @IsActive
                WHERE UserId = @UserId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@CustomerId", (object)user.CustomerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EngineerId", (object)user.EngineerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateLastLogin(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "UPDATE Users SET LastLoginAt = SYSUTCDATETIME() WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Returns the user (with RoleName populated) if credentials are valid; null otherwise.
        public User ValidateUser(string email, string password)
        {
            const string sql = @"
                SELECT u.UserId, u.Username, u.Email, u.PasswordHash, u.RoleId,
                       u.CustomerId, u.EngineerId, u.IsActive, u.CreatedAt, u.LastLoginAt,
                       r.RoleName
                FROM   Users u
                INNER  JOIN Roles r ON r.RoleId = u.RoleId
                WHERE  u.Email = @Email AND u.IsActive = 1";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    var user = MapWithRole(reader);
                    return PasswordHelper.Verify(password, user.PasswordHash) ? user : null;
                }
            }
        }

        public bool Delete(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Users WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private User MapWithRole(SqlDataReader r)
        {
            var user = Map(r);
            user.RoleName = r.GetString(r.GetOrdinal("RoleName"));
            return user;
        }

        private User Map(SqlDataReader r) => new User(
            r.GetInt32(r.GetOrdinal("UserId")),
            r.GetString(r.GetOrdinal("Username")),
            r.GetString(r.GetOrdinal("Email")),
            r.GetString(r.GetOrdinal("PasswordHash")),
            r.GetInt32(r.GetOrdinal("RoleId")),
            r.IsDBNull(r.GetOrdinal("CustomerId")) ? (int?)null : r.GetInt32(r.GetOrdinal("CustomerId")),
            r.IsDBNull(r.GetOrdinal("EngineerId")) ? (int?)null : r.GetInt32(r.GetOrdinal("EngineerId")),
            r.GetBoolean(r.GetOrdinal("IsActive")),
            r.GetDateTime(r.GetOrdinal("CreatedAt")),
            r.IsDBNull(r.GetOrdinal("LastLoginAt")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("LastLoginAt"))
        );
    }
}
