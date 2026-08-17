using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
	// DP: Repository 
	// SOLID: ISP/DIP 
	public class SupportEngineerRepository : ICatalogRepository<SupportEngineer>
	{
		public List<SupportEngineer> GetAll()
		{
			var list = new List<SupportEngineer>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
                "SELECT EngineerId, Name, Email, PhoneNumber, LevelId, ScheduleId, IsActive FROM SupportEngineers", conn))
			{
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

		public SupportEngineer GetById(int engineerId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
                "SELECT EngineerId, Name, Email, PhoneNumber, LevelId, ScheduleId, IsActive FROM SupportEngineers WHERE EngineerId = @EngineerId", conn))
			{
				cmd.Parameters.AddWithValue("@EngineerId", engineerId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					return reader.Read() ? Map(reader) : null;
			}
		}

		public SupportEngineer GetByEmail(string email)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
                "SELECT EngineerId, Name, Email, PhoneNumber, LevelId, ScheduleId, IsActive FROM SupportEngineers WHERE Email = @Email", conn))
			{
				cmd.Parameters.AddWithValue("@Email", email);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					return reader.Read() ? Map(reader) : null;
			}
		}

		public int Insert(SupportEngineer engineer)
		{
			const string sql = @"
                INSERT INTO SupportEngineers (Name, Email, PhoneNumber, LevelId, ScheduleId, JobTitle, IsActive)
                OUTPUT INSERTED.EngineerId
                VALUES (@Name, @Email, @PhoneNumber, @LevelId, @ScheduleId, @JobTitle, @IsActive)";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@Name", engineer.Name);
				cmd.Parameters.AddWithValue("@Email", engineer.Email);
				cmd.Parameters.AddWithValue("@PhoneNumber", (object)engineer.PhoneNumber ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@LevelId", engineer.LevelId);
				cmd.Parameters.AddWithValue("@ScheduleId", engineer.ScheduleId);
				cmd.Parameters.AddWithValue("@JobTitle", engineer.JobTitle);
				cmd.Parameters.AddWithValue("@IsActive", engineer.IsActive);
				conn.Open();
				return (int)cmd.ExecuteScalar();
			}
		}

		public bool Update(SupportEngineer engineer)
		{
			const string sql = @"
                UPDATE SupportEngineers
                SET Name = @Name, Email = @Email, PhoneNumber = @PhoneNumber,
                    LevelId = @LevelId, ScheduleId = @ScheduleId, JobTitle = @JobTitle, IsActive = @IsActive
                WHERE EngineerId = @EngineerId";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@EngineerId", engineer.EngineerId);
				cmd.Parameters.AddWithValue("@Name", engineer.Name);
				cmd.Parameters.AddWithValue("@Email", engineer.Email);
				cmd.Parameters.AddWithValue("@PhoneNumber", (object)engineer.PhoneNumber ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@LevelId", engineer.LevelId);
				cmd.Parameters.AddWithValue("@ScheduleId", engineer.ScheduleId);
				cmd.Parameters.AddWithValue("@JobTitle", engineer.JobTitle);
				cmd.Parameters.AddWithValue("@IsActive", engineer.IsActive);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		public bool Delete(int engineerId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand("DELETE FROM SupportEngineers WHERE EngineerId = @EngineerId", conn))
			{
				cmd.Parameters.AddWithValue("@EngineerId", engineerId);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

        public List<EngineerViewModel> GetAllWithLevel()
        {
            var list = new List<EngineerViewModel>();

            const string sql = @"
				SELECT se.EngineerId,
					   se.Name,
					   se.Email,
					   se.PhoneNumber,
					   se.IsActive,
					   el.LevelName
				FROM SupportEngineers se
				JOIN EngineerLevels el ON el.LevelId = se.LevelId
				ORDER BY se.Name";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EngineerViewModel
                        {
                            EngineerId = (int)reader["EngineerId"],
                            Name = (string)reader["Name"],
                            Email = (string)reader["Email"],
                            PhoneNumber = reader["PhoneNumber"] as string,

                            // La BD actual no tiene JobTitle
                            JobTitle = null,

                            LevelName = (string)reader["LevelName"],
                            IsActive = (bool)reader["IsActive"]
                        });
                    }
                }
            }

            return list;
        }

        private SupportEngineer Map(SqlDataReader r) => new SupportEngineer(
			r.GetInt32(r.GetOrdinal("EngineerId")),
			r.GetString(r.GetOrdinal("Name")),
			r.GetString(r.GetOrdinal("Email")),
			r.IsDBNull(r.GetOrdinal("PhoneNumber"))? null: r.GetString(r.GetOrdinal("PhoneNumber")),
			r.GetInt32(r.GetOrdinal("LevelId")),
			r.GetInt32(r.GetOrdinal("ScheduleId")),null,
			r.GetBoolean(r.GetOrdinal("IsActive"))
		);
    }
}
