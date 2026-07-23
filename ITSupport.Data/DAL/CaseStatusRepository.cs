using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
	// DP: Repository - encapsula el acceso a datos de la tabla CaseStatuses.
	// SOLID: LSP - implementa ICaseStatusRepository y puede sustituirse por cualquier
	// otra implementacion (ej. un mock en pruebas) sin afectar a la capa de negocio.
	public class CaseStatusRepository : ICaseStatusRepository
	{
		public List<CaseStatus> GetAll()
		{
			var list = new List<CaseStatus>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				"SELECT StatusId, StatusName, Description, IsOpen FROM CaseStatuses", conn))
			{
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

		public CaseStatus GetById(int statusId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				"SELECT StatusId, StatusName, Description, IsOpen FROM CaseStatuses WHERE StatusId = @StatusId", conn))
			{
				cmd.Parameters.AddWithValue("@StatusId", statusId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					return reader.Read() ? Map(reader) : null;
			}
		}

		public int Insert(CaseStatus status)
		{
			const string sql = @"
                INSERT INTO CaseStatuses (StatusName, Description, IsOpen)
                OUTPUT INSERTED.StatusId
                VALUES (@StatusName, @Description, @IsOpen)";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@StatusName", status.StatusName);
				cmd.Parameters.AddWithValue("@Description", (object)status.Description ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@IsOpen", status.IsOpen);
				conn.Open();
				return (int)cmd.ExecuteScalar();
			}
		}

		public bool Update(CaseStatus status)
		{
			const string sql = @"
                UPDATE CaseStatuses
                SET StatusName = @StatusName, Description = @Description, IsOpen = @IsOpen
                WHERE StatusId = @StatusId";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@StatusId", status.StatusId);
				cmd.Parameters.AddWithValue("@StatusName", status.StatusName);
				cmd.Parameters.AddWithValue("@Description", (object)status.Description ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@IsOpen", status.IsOpen);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		public bool Delete(int statusId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand("DELETE FROM CaseStatuses WHERE StatusId = @StatusId", conn))
			{
				cmd.Parameters.AddWithValue("@StatusId", statusId);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		private CaseStatus Map(SqlDataReader r) => new CaseStatus(
			r.GetInt32(r.GetOrdinal("StatusId")),
			r.GetString(r.GetOrdinal("StatusName")),
			r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
			r.GetBoolean(r.GetOrdinal("IsOpen"))
		);
	}
}
