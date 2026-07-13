using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class EngineerLevelRepository
    {
        public List<EngineerLevel> GetAll()
        {
            var list = new List<EngineerLevel>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT LevelId, LevelName, Description FROM EngineerLevels", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public EngineerLevel GetById(int levelId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT LevelId, LevelName, Description FROM EngineerLevels WHERE LevelId = @LevelId", conn))
            {
                cmd.Parameters.AddWithValue("@LevelId", levelId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(EngineerLevel level)
        {
            const string sql = @"
                INSERT INTO EngineerLevels (LevelName, Description)
                OUTPUT INSERTED.LevelId
                VALUES (@LevelName, @Description)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LevelName", level.LevelName);
                cmd.Parameters.AddWithValue("@Description", (object)level.Description ?? DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(EngineerLevel level)
        {
            const string sql = @"
                UPDATE EngineerLevels
                SET LevelName = @LevelName, Description = @Description
                WHERE LevelId = @LevelId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LevelId", level.LevelId);
                cmd.Parameters.AddWithValue("@LevelName", level.LevelName);
                cmd.Parameters.AddWithValue("@Description", (object)level.Description ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int levelId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM EngineerLevels WHERE LevelId = @LevelId", conn))
            {
                cmd.Parameters.AddWithValue("@LevelId", levelId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private EngineerLevel Map(SqlDataReader r) => new EngineerLevel(
            r.GetInt32(r.GetOrdinal("LevelId")),
            r.GetString(r.GetOrdinal("LevelName")),
            r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description"))
        );
    }
}
