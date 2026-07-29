using Oracle.ManagedDataAccess.Client;

namespace Studentmanagmentsystem.Data
{
    public class OracleDb
    {
        private readonly string _connectionString =
    "User Id=system;Password=bharat;Data Source=localhost:1521/XE;";

        public OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }
}