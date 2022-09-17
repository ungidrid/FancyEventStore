using System.Data;
using System.Data.SqlClient;

namespace FancyEventStore.ReadModel
{
    public class ReadModelContext : IReadModelContext, IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection? _connection;

        public ReadModelContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection => OpenConnection();
            

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private IDbConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }
    }
}
