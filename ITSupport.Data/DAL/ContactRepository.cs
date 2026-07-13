using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class ContactRepository
    {
        public List<Contact> GetAll()
        {
            var list = new List<Contact>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ContactId, CustomerId, ContactName, ContactEmail, ContactPhone, Role, IsPrimary FROM Contacts", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public Contact GetById(int contactId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ContactId, CustomerId, ContactName, ContactEmail, ContactPhone, Role, IsPrimary FROM Contacts WHERE ContactId = @ContactId", conn))
            {
                cmd.Parameters.AddWithValue("@ContactId", contactId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public List<Contact> GetByCustomerId(int customerId)
        {
            var list = new List<Contact>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ContactId, CustomerId, ContactName, ContactEmail, ContactPhone, Role, IsPrimary FROM Contacts WHERE CustomerId = @CustomerId", conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public int Insert(Contact contact)
        {
            const string sql = @"
                INSERT INTO Contacts (CustomerId, ContactName, ContactEmail, ContactPhone, Role, IsPrimary)
                OUTPUT INSERTED.ContactId
                VALUES (@CustomerId, @ContactName, @ContactEmail, @ContactPhone, @Role, @IsPrimary)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", contact.CustomerId);
                cmd.Parameters.AddWithValue("@ContactName", contact.ContactName);
                cmd.Parameters.AddWithValue("@ContactEmail", (object)contact.ContactEmail ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPhone", (object)contact.ContactPhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", (object)contact.Role ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPrimary", contact.IsPrimary);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Contact contact)
        {
            const string sql = @"
                UPDATE Contacts
                SET CustomerId = @CustomerId, ContactName = @ContactName,
                    ContactEmail = @ContactEmail, ContactPhone = @ContactPhone,
                    Role = @Role, IsPrimary = @IsPrimary
                WHERE ContactId = @ContactId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ContactId", contact.ContactId);
                cmd.Parameters.AddWithValue("@CustomerId", contact.CustomerId);
                cmd.Parameters.AddWithValue("@ContactName", contact.ContactName);
                cmd.Parameters.AddWithValue("@ContactEmail", (object)contact.ContactEmail ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPhone", (object)contact.ContactPhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", (object)contact.Role ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPrimary", contact.IsPrimary);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int contactId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Contacts WHERE ContactId = @ContactId", conn))
            {
                cmd.Parameters.AddWithValue("@ContactId", contactId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Contact Map(SqlDataReader r) => new Contact(
            r.GetInt32(r.GetOrdinal("ContactId")),
            r.GetInt32(r.GetOrdinal("CustomerId")),
            r.GetString(r.GetOrdinal("ContactName")),
            r.IsDBNull(r.GetOrdinal("ContactEmail")) ? null : r.GetString(r.GetOrdinal("ContactEmail")),
            r.IsDBNull(r.GetOrdinal("ContactPhone")) ? null : r.GetString(r.GetOrdinal("ContactPhone")),
            r.IsDBNull(r.GetOrdinal("Role")) ? null : r.GetString(r.GetOrdinal("Role")),
            r.GetBoolean(r.GetOrdinal("IsPrimary"))
        );
    }
}
