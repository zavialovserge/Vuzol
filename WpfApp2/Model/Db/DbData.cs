using System.Data;
using System.Data.SqlClient;
namespace Vuzol.Model.Db
{
    public class DbData
    {
        private readonly string _connectionString;
        public DbData()
        {
            _connectionString = SqlConnectionDb.ConnectionString;
        }

        public DbData(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection Connect()
        {
            return new SqlConnection(_connectionString);
        }       
    }
}
