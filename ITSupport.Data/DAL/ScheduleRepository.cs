using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class ScheduleRepository
    {
        public List<Schedule> GetAll()
        {
            var list = new List<Schedule>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ScheduleId, ScheduleName, TimeZone, StartTime, EndTime, WorkDays FROM Schedules", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public Schedule GetById(int scheduleId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ScheduleId, ScheduleName, TimeZone, StartTime, EndTime, WorkDays FROM Schedules WHERE ScheduleId = @ScheduleId", conn))
            {
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(Schedule schedule)
        {
            const string sql = @"
                INSERT INTO Schedules (ScheduleName, TimeZone, StartTime, EndTime, WorkDays)
                OUTPUT INSERTED.ScheduleId
                VALUES (@ScheduleName, @TimeZone, @StartTime, @EndTime, @WorkDays)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ScheduleName", schedule.ScheduleName);
                cmd.Parameters.AddWithValue("@TimeZone", schedule.TimeZone);
                cmd.Parameters.AddWithValue("@StartTime", schedule.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", schedule.EndTime);
                cmd.Parameters.AddWithValue("@WorkDays", schedule.WorkDays);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Schedule schedule)
        {
            const string sql = @"
                UPDATE Schedules
                SET ScheduleName = @ScheduleName, TimeZone = @TimeZone,
                    StartTime = @StartTime, EndTime = @EndTime, WorkDays = @WorkDays
                WHERE ScheduleId = @ScheduleId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ScheduleId", schedule.ScheduleId);
                cmd.Parameters.AddWithValue("@ScheduleName", schedule.ScheduleName);
                cmd.Parameters.AddWithValue("@TimeZone", schedule.TimeZone);
                cmd.Parameters.AddWithValue("@StartTime", schedule.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", schedule.EndTime);
                cmd.Parameters.AddWithValue("@WorkDays", schedule.WorkDays);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int scheduleId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Schedules WHERE ScheduleId = @ScheduleId", conn))
            {
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Schedule Map(SqlDataReader r) => new Schedule(
            r.GetInt32(r.GetOrdinal("ScheduleId")),
            r.GetString(r.GetOrdinal("ScheduleName")),
            r.GetString(r.GetOrdinal("TimeZone")),
            (TimeSpan)r["StartTime"],
            (TimeSpan)r["EndTime"],
            r.GetString(r.GetOrdinal("WorkDays"))
        );
    }
}
