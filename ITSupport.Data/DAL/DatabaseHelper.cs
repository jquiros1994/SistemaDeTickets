using System.Configuration;
using System.Data.SqlClient;

namespace ITSupport.DAL
{
	// SOLID: SRP - unica responsabilidad: crear conexiones a la base de datos.
	// DP: Factory Method - GetConnection()
	public static class DatabaseHelper
    {
        private static readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["TicketSystemDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
