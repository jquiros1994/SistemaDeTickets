using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class CustomerRepository
    {
        public List<Customer> GetAll()
        {
            var list = new List<Customer>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, Country, ProgramId, IsActive, CreatedAt FROM Customers", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public Customer GetById(int customerId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, Country, ProgramId, IsActive, CreatedAt FROM Customers WHERE CustomerId = @CustomerId", conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public Customer GetByEmail(string email)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, Country, ProgramId, IsActive, CreatedAt FROM Customers WHERE Email = @Email", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(Customer customer)
        {
            const string sql = @"
                INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, Country, ProgramId, IsActive)
                OUTPUT INSERTED.CustomerId
                VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Country, @ProgramId, @IsActive)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", customer.Country);
                cmd.Parameters.AddWithValue("@ProgramId", customer.ProgramId);
                cmd.Parameters.AddWithValue("@IsActive", customer.IsActive);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Customer customer)
        {
            const string sql = @"
                UPDATE Customers
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email,
                    PhoneNumber = @PhoneNumber, Country = @Country,
                    ProgramId = @ProgramId, IsActive = @IsActive
                WHERE CustomerId = @CustomerId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", customer.Country);
                cmd.Parameters.AddWithValue("@ProgramId", customer.ProgramId);
                cmd.Parameters.AddWithValue("@IsActive", customer.IsActive);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int customerId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Customers WHERE CustomerId = @CustomerId", conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Customer Map(SqlDataReader r) => new Customer(
            r.GetInt32(r.GetOrdinal("CustomerId")),
            r.GetString(r.GetOrdinal("FirstName")),
            r.GetString(r.GetOrdinal("LastName")),
            r.GetString(r.GetOrdinal("Email")),
            r.IsDBNull(r.GetOrdinal("PhoneNumber")) ? null : r.GetString(r.GetOrdinal("PhoneNumber")),
            r.GetString(r.GetOrdinal("Country")),
            r.GetInt32(r.GetOrdinal("ProgramId")),
            r.GetBoolean(r.GetOrdinal("IsActive")),
            r.GetDateTime(r.GetOrdinal("CreatedAt"))
        );
    }
}
