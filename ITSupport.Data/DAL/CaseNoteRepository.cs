using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class CaseNoteRepository
    {
        public List<CaseNote> GetByCaseId(int caseId)
        {
            var list = new List<CaseNote>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT NoteId, CaseId, UserId, NoteText, IsInternal, CreatedAt FROM CaseNotes WHERE CaseId = @CaseId ORDER BY CreatedAt", conn))
            {
                cmd.Parameters.AddWithValue("@CaseId", caseId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public CaseNote GetById(int noteId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT NoteId, CaseId, UserId, NoteText, IsInternal, CreatedAt FROM CaseNotes WHERE NoteId = @NoteId", conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", noteId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(CaseNote note)
        {
            const string sql = @"
                INSERT INTO CaseNotes (CaseId, UserId, NoteText, IsInternal)
                OUTPUT INSERTED.NoteId
                VALUES (@CaseId, @UserId, @NoteText, @IsInternal)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CaseId", note.CaseId);
                cmd.Parameters.AddWithValue("@UserId", note.UserId);
                cmd.Parameters.AddWithValue("@NoteText", note.NoteText);
                cmd.Parameters.AddWithValue("@IsInternal", note.IsInternal);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(CaseNote note)
        {
            const string sql = @"
                UPDATE CaseNotes
                SET NoteText = @NoteText, IsInternal = @IsInternal
                WHERE NoteId = @NoteId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", note.NoteId);
                cmd.Parameters.AddWithValue("@NoteText", note.NoteText);
                cmd.Parameters.AddWithValue("@IsInternal", note.IsInternal);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int noteId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM CaseNotes WHERE NoteId = @NoteId", conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", noteId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private CaseNote Map(SqlDataReader r) => new CaseNote(
            r.GetInt32(r.GetOrdinal("NoteId")),
            r.GetInt32(r.GetOrdinal("CaseId")),
            r.GetInt32(r.GetOrdinal("UserId")),
            r.GetString(r.GetOrdinal("NoteText")),
            r.GetBoolean(r.GetOrdinal("IsInternal")),
            r.GetDateTime(r.GetOrdinal("CreatedAt"))
        );
    }
}
