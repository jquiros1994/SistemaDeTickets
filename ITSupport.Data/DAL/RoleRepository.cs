using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class RoleRepository
    {
        public List<Role> GetAll()
        {
            var list = new List<Role>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT RoleId, RoleName, Description FROM Roles", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public Role GetById(int roleId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT RoleId, RoleName, Description FROM Roles WHERE RoleId = @RoleId", conn))
            {
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(Role role)
        {
            const string sql = @"
                INSERT INTO Roles (RoleName, Description)
                OUTPUT INSERTED.RoleId
                VALUES (@RoleName, @Description)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                cmd.Parameters.AddWithValue("@Description", (object)role.Description ?? DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Role role)
        {
            const string sql = @"
                UPDATE Roles SET RoleName = @RoleName, Description = @Description
                WHERE RoleId = @RoleId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@RoleId", role.RoleId);
                cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                cmd.Parameters.AddWithValue("@Description", (object)role.Description ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int roleId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Roles WHERE RoleId = @RoleId", conn))
            {
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Role Map(SqlDataReader r) => new Role(
            r.GetInt32(r.GetOrdinal("RoleId")),
            r.GetString(r.GetOrdinal("RoleName")),
            r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description"))
        );
    }
}
