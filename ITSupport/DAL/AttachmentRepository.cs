using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class AttachmentRepository
    {
        public List<Attachment> GetByCaseId(int caseId)
        {
            var list = new List<Attachment>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT AttachmentId, CaseId, FileName, FilePath, FileSize, ContentType, UploadedByUserId, UploadedAt FROM Attachments WHERE CaseId = @CaseId ORDER BY UploadedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@CaseId", caseId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public Attachment GetById(int attachmentId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT AttachmentId, CaseId, FileName, FilePath, FileSize, ContentType, UploadedByUserId, UploadedAt FROM Attachments WHERE AttachmentId = @AttachmentId", conn))
            {
                cmd.Parameters.AddWithValue("@AttachmentId", attachmentId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(Attachment attachment)
        {
            const string sql = @"
                INSERT INTO Attachments (CaseId, FileName, FilePath, FileSize, ContentType, UploadedByUserId)
                OUTPUT INSERTED.AttachmentId
                VALUES (@CaseId, @FileName, @FilePath, @FileSize, @ContentType, @UploadedByUserId)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CaseId", attachment.CaseId);
                cmd.Parameters.AddWithValue("@FileName", attachment.FileName);
                cmd.Parameters.AddWithValue("@FilePath", attachment.FilePath);
                cmd.Parameters.AddWithValue("@FileSize", (object)attachment.FileSize ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContentType", (object)attachment.ContentType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UploadedByUserId", attachment.UploadedByUserId);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Delete(int attachmentId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Attachments WHERE AttachmentId = @AttachmentId", conn))
            {
                cmd.Parameters.AddWithValue("@AttachmentId", attachmentId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Attachment Map(SqlDataReader r) => new Attachment(
            r.GetInt32(r.GetOrdinal("AttachmentId")),
            r.GetInt32(r.GetOrdinal("CaseId")),
            r.GetString(r.GetOrdinal("FileName")),
            r.GetString(r.GetOrdinal("FilePath")),
            r.IsDBNull(r.GetOrdinal("FileSize")) ? (long?)null : r.GetInt64(r.GetOrdinal("FileSize")),
            r.IsDBNull(r.GetOrdinal("ContentType")) ? null : r.GetString(r.GetOrdinal("ContentType")),
            r.GetInt32(r.GetOrdinal("UploadedByUserId")),
            r.GetDateTime(r.GetOrdinal("UploadedAt"))
        );
    }
}
