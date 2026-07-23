using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
	// DP: Repository
	// SOLID: ISP/DIP 
	public class ProgramRepository : ICatalogRepository<SupportProgram>
	{
		public List<SupportProgram> GetAll()
		{
			var list = new List<SupportProgram>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				"SELECT ProgramId, ProgramName, SupportLevel, Description FROM Programs", conn))
			{
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

		public SupportProgram GetById(int programId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				"SELECT ProgramId, ProgramName, SupportLevel, Description FROM Programs WHERE ProgramId = @ProgramId", conn))
			{
				cmd.Parameters.AddWithValue("@ProgramId", programId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					return reader.Read() ? Map(reader) : null;
			}
		}

		public int Insert(SupportProgram program)
		{
			const string sql = @"
                INSERT INTO Programs (ProgramName, SupportLevel, Description)
                OUTPUT INSERTED.ProgramId
                VALUES (@ProgramName, @SupportLevel, @Description)";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@ProgramName", program.ProgramName);
				cmd.Parameters.AddWithValue("@SupportLevel", program.SupportLevel);
				cmd.Parameters.AddWithValue("@Description", (object)program.Description ?? DBNull.Value);
				conn.Open();
				return (int)cmd.ExecuteScalar();
			}
		}

		public bool Update(SupportProgram program)
		{
			const string sql = @"
                UPDATE Programs
                SET ProgramName = @ProgramName, SupportLevel = @SupportLevel, Description = @Description
                WHERE ProgramId = @ProgramId";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@ProgramId", program.ProgramId);
				cmd.Parameters.AddWithValue("@ProgramName", program.ProgramName);
				cmd.Parameters.AddWithValue("@SupportLevel", program.SupportLevel);
				cmd.Parameters.AddWithValue("@Description", (object)program.Description ?? DBNull.Value);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		public bool Delete(int programId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand("DELETE FROM Programs WHERE ProgramId = @ProgramId", conn))
			{
				cmd.Parameters.AddWithValue("@ProgramId", programId);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		private SupportProgram Map(SqlDataReader r) => new SupportProgram(
			r.GetInt32(r.GetOrdinal("ProgramId")),
			r.GetString(r.GetOrdinal("ProgramName")),
			r.GetString(r.GetOrdinal("SupportLevel")),
			r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description"))
		);
	}
}
