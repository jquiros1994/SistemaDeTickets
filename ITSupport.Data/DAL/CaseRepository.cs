using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
	// DP: Repository - esta clase implementa el patron Repository
	// SOLID: SRP - su unica responsabilidad es la persistencia de Casos
	// SOLID: DIP - implementa ICaseRepository 
	public class CaseRepository : ICaseRepository
	{
		private const string SelectColumns = @"
            CaseId, CaseNumber, StatusId, PriorityId, Title, Description,
            ContactId, ProgramId, Country, OwnerId, CreatedByUserId,
            PreferredContact, CreatedAt, UpdatedAt, ClosedAt";

		public List<Case> GetAll()
		{
			var list = new List<Case>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand($"SELECT {SelectColumns} FROM Cases ORDER BY CreatedAt DESC", conn))
			{
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

		public List<ITSupport.Models.CaseViewModel> GetAllWithDetails(int? statusId = null, string search = null)
		{
			var list = new List<ITSupport.Models.CaseViewModel>();
			var where = "WHERE 1=1";
			if (statusId.HasValue) where += " AND c.StatusId = @StatusId";
			if (!string.IsNullOrWhiteSpace(search)) where += " AND (c.Title LIKE @Search OR c.CaseNumber LIKE @Search)";

			var sql = $@"
                SELECT c.CaseId, c.CaseNumber, c.Title, c.Description,
					   c.Application,
					   cs.StatusName, cs.IsOpen,
                       p.PriorityName, p.SlaHours,
                       ct.ContactName, ct.ContactEmail,
                       se.Name AS OwnerName,
                       pr.ProgramName,
                       c.Country, c.PreferredContact,
                       c.CreatedAt, c.UpdatedAt, c.ClosedAt
                FROM   Cases c
                JOIN   CaseStatuses      cs ON cs.StatusId   = c.StatusId
                JOIN   Priorities        p  ON p.PriorityId  = c.PriorityId
                JOIN   Contacts          ct ON ct.ContactId  = c.ContactId
                JOIN   Programs          pr ON pr.ProgramId  = c.ProgramId
                LEFT JOIN SupportEngineers se ON se.EngineerId = c.OwnerId
                {where}
                ORDER BY c.CreatedAt DESC";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				if (statusId.HasValue)
					cmd.Parameters.AddWithValue("@StatusId", statusId.Value);
				if (!string.IsNullOrWhiteSpace(search))
					cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

				conn.Open();
				using (var r = cmd.ExecuteReader())
				{
					while (r.Read())
					{
                        list.Add(new ITSupport.Models.CaseViewModel
                        {
                            CaseId = r.GetInt32(r.GetOrdinal("CaseId")),
                            CaseNumber = r.IsDBNull(r.GetOrdinal("CaseNumber"))? null: r.GetString(r.GetOrdinal("CaseNumber")),
                            Title = r.GetString(r.GetOrdinal("Title")),
                            Description = r.GetString(r.GetOrdinal("Description")),
                            Application = r.IsDBNull(r.GetOrdinal("Application"))? null: r.GetString(r.GetOrdinal("Application")),
                            StatusName = r.GetString(r.GetOrdinal("StatusName")),
                            IsOpen = r.GetBoolean(r.GetOrdinal("IsOpen")),
                            PriorityName = r.GetString(r.GetOrdinal("PriorityName")),
                            SlaHours = r.GetInt32(r.GetOrdinal("SlaHours")),
                            ContactName = r.GetString(r.GetOrdinal("ContactName")),
                            ContactEmail = r.IsDBNull(r.GetOrdinal("ContactEmail"))? null: r.GetString(r.GetOrdinal("ContactEmail")),
                            OwnerName = r.IsDBNull(r.GetOrdinal("OwnerName"))? null: r.GetString(r.GetOrdinal("OwnerName")),
                            ProgramName = r.GetString(r.GetOrdinal("ProgramName")),
                            Country = r.GetString(r.GetOrdinal("Country")),
                            PreferredContact = r.IsDBNull(r.GetOrdinal("PreferredContact"))? null: r.GetString(r.GetOrdinal("PreferredContact")),
                            CreatedAt = r.GetDateTime(r.GetOrdinal("CreatedAt")),
                            UpdatedAt = r.GetDateTime(r.GetOrdinal("UpdatedAt")),
                            ClosedAt = r.IsDBNull(r.GetOrdinal("ClosedAt"))? (DateTime?)null: r.GetDateTime(r.GetOrdinal("ClosedAt"))
                        });
                    }
				}
			}
			return list;
		}

		public ITSupport.Models.CaseViewModel GetByIdWithDetails(int caseId)
		{
			const string sql = @"
                SELECT c.CaseId, c.CaseNumber, c.Title, c.Description,
                       cs.StatusName, cs.IsOpen,
                       p.PriorityName, p.SlaHours,
                       ct.ContactName, ct.ContactEmail,
                       se.Name AS OwnerName,
                       pr.ProgramName,
                       c.Country, c.PreferredContact,
                       c.CreatedAt, c.UpdatedAt, c.ClosedAt
                FROM   Cases c
                JOIN   CaseStatuses      cs ON cs.StatusId   = c.StatusId
                JOIN   Priorities        p  ON p.PriorityId  = c.PriorityId
                JOIN   Contacts          ct ON ct.ContactId  = c.ContactId
                JOIN   Programs          pr ON pr.ProgramId  = c.ProgramId
                LEFT JOIN SupportEngineers se ON se.EngineerId = c.OwnerId
                WHERE  c.CaseId = @CaseId";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@CaseId", caseId);
				conn.Open();
				using (var r = cmd.ExecuteReader())
				{
					if (!r.Read()) return null;
					return new ITSupport.Models.CaseViewModel
					{
						CaseId = r.GetInt32(r.GetOrdinal("CaseId")),
						CaseNumber = r.IsDBNull(r.GetOrdinal("CaseNumber")) ? null : r.GetString(r.GetOrdinal("CaseNumber")),
						Title = r.GetString(r.GetOrdinal("Title")),
						Description = r.GetString(r.GetOrdinal("Description")),
						StatusName = r.GetString(r.GetOrdinal("StatusName")),
						IsOpen = r.GetBoolean(r.GetOrdinal("IsOpen")),
						PriorityName = r.GetString(r.GetOrdinal("PriorityName")),
						SlaHours = r.GetInt32(r.GetOrdinal("SlaHours")),
						ContactName = r.GetString(r.GetOrdinal("ContactName")),
						ContactEmail = r.IsDBNull(r.GetOrdinal("ContactEmail")) ? null : r.GetString(r.GetOrdinal("ContactEmail")),
						OwnerName = r.IsDBNull(r.GetOrdinal("OwnerName")) ? null : r.GetString(r.GetOrdinal("OwnerName")),
						ProgramName = r.GetString(r.GetOrdinal("ProgramName")),
						Country = r.GetString(r.GetOrdinal("Country")),
						PreferredContact = r.IsDBNull(r.GetOrdinal("PreferredContact")) ? null : r.GetString(r.GetOrdinal("PreferredContact")),
						CreatedAt = r.GetDateTime(r.GetOrdinal("CreatedAt")),
						UpdatedAt = r.GetDateTime(r.GetOrdinal("UpdatedAt")),
						ClosedAt = r.IsDBNull(r.GetOrdinal("ClosedAt")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("ClosedAt")),
					};
				}
			}
		}

		public Case GetById(int caseId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				$"SELECT {SelectColumns} FROM Cases WHERE CaseId = @CaseId", conn))
			{
				cmd.Parameters.AddWithValue("@CaseId", caseId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					return reader.Read() ? Map(reader) : null;
			}
		}

		public List<Case> GetByStatus(int statusId)
		{
			var list = new List<Case>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				$"SELECT {SelectColumns} FROM Cases WHERE StatusId = @StatusId ORDER BY CreatedAt DESC", conn))
			{
				cmd.Parameters.AddWithValue("@StatusId", statusId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

		public List<Case> GetByOwner(int engineerId)
		{
			var list = new List<Case>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				$"SELECT {SelectColumns} FROM Cases WHERE OwnerId = @OwnerId ORDER BY PriorityId, CreatedAt DESC", conn))
			{
				cmd.Parameters.AddWithValue("@OwnerId", engineerId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

		public List<Case> GetByContact(int contactId)
		{
			var list = new List<Case>();
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(
				$"SELECT {SelectColumns} FROM Cases WHERE ContactId = @ContactId ORDER BY CreatedAt DESC", conn))
			{
				cmd.Parameters.AddWithValue("@ContactId", contactId);
				conn.Open();
				using (var reader = cmd.ExecuteReader())
					while (reader.Read())
						list.Add(Map(reader));
			}
			return list;
		}

        public int Insert(Case c)
        {
            const string sql = @"
        INSERT INTO Cases
			(StatusId, PriorityId, Title, Description, Application,
			ContactId, ProgramId, Country, OwnerId, CreatedByUserId, PreferredContact)
        OUTPUT INSERTED.CaseId
        VALUES
            (@StatusId, @PriorityId, @Title, @Description, @Application,
             @ContactId, @ProgramId, @Country, @OwnerId, @CreatedByUserId, @PreferredContact)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@StatusId", c.StatusId);
                cmd.Parameters.AddWithValue("@PriorityId", c.PriorityId);
                cmd.Parameters.AddWithValue("@Title", c.Title);
                cmd.Parameters.AddWithValue("@Description", c.Description);
                cmd.Parameters.AddWithValue("@Application",
                    (object)c.Application ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactId", c.ContactId);
                cmd.Parameters.AddWithValue("@ProgramId", c.ProgramId);
                cmd.Parameters.AddWithValue("@Country", c.Country);
                cmd.Parameters.AddWithValue("@OwnerId",
                    (object)c.OwnerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedByUserId", c.CreatedByUserId);
                cmd.Parameters.AddWithValue("@PreferredContact",
                    (object)c.PreferredContact ?? DBNull.Value);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Case c)
		{
			const string sql = @"
                UPDATE Cases
                SET StatusId = @StatusId, PriorityId = @PriorityId, Title = @Title,
                    Description = @Description, ContactId = @ContactId, ProgramId = @ProgramId,
                    Country = @Country, OwnerId = @OwnerId, PreferredContact = @PreferredContact,
                    UpdatedAt = SYSUTCDATETIME()
                WHERE CaseId = @CaseId";

			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@CaseId", c.CaseId);
				cmd.Parameters.AddWithValue("@StatusId", c.StatusId);
				cmd.Parameters.AddWithValue("@PriorityId", c.PriorityId);
				cmd.Parameters.AddWithValue("@Title", c.Title);
				cmd.Parameters.AddWithValue("@Description", c.Description);
				cmd.Parameters.AddWithValue("@ContactId", c.ContactId);
				cmd.Parameters.AddWithValue("@ProgramId", c.ProgramId);
				cmd.Parameters.AddWithValue("@Country", c.Country);
				cmd.Parameters.AddWithValue("@OwnerId", (object)c.OwnerId ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@PreferredContact", (object)c.PreferredContact ?? DBNull.Value);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		public bool SetClosedAt(int caseId, DateTime? closedAt)
		{
			const string sql = "UPDATE Cases SET ClosedAt = @ClosedAt WHERE CaseId = @CaseId";
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@CaseId", caseId);
				cmd.Parameters.AddWithValue("@ClosedAt", (object)closedAt ?? DBNull.Value);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		public bool Delete(int caseId)
		{
			using (var conn = DatabaseHelper.GetConnection())
			using (var cmd = new SqlCommand("DELETE FROM Cases WHERE CaseId = @CaseId", conn))
			{
				cmd.Parameters.AddWithValue("@CaseId", caseId);
				conn.Open();
				return cmd.ExecuteNonQuery() > 0;
			}
		}

		private Case Map(SqlDataReader r) => new Case(
			r.GetInt32(r.GetOrdinal("CaseId")),
			r.IsDBNull(r.GetOrdinal("CaseNumber")) ? null : r.GetString(r.GetOrdinal("CaseNumber")),
			r.GetInt32(r.GetOrdinal("StatusId")),
			r.GetInt32(r.GetOrdinal("PriorityId")),
			r.GetString(r.GetOrdinal("Title")),
			r.GetString(r.GetOrdinal("Description")),
			r.GetInt32(r.GetOrdinal("ContactId")),
			r.GetInt32(r.GetOrdinal("ProgramId")),
			r.GetString(r.GetOrdinal("Country")),
			r.IsDBNull(r.GetOrdinal("OwnerId")) ? (int?)null : r.GetInt32(r.GetOrdinal("OwnerId")),
			r.GetInt32(r.GetOrdinal("CreatedByUserId")),
			r.IsDBNull(r.GetOrdinal("PreferredContact")) ? null : r.GetString(r.GetOrdinal("PreferredContact")),
			r.GetDateTime(r.GetOrdinal("CreatedAt")),
			r.GetDateTime(r.GetOrdinal("UpdatedAt")),
			r.IsDBNull(r.GetOrdinal("ClosedAt")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("ClosedAt"))
		);
	}
}
