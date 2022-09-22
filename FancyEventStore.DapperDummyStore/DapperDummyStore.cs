using Dapper;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using System.Data.SqlClient;
using System.Text;

namespace FancyEventStore.DapperDummyStore
{
    internal class DapperDummyStore : IStore
    {
        private readonly string _connectionString;

        public DapperDummyStore(string connectionString)
        {
            _connectionString = connectionString;
            EnsureCreated();
        }

        public async Task AppendEventsAsync(IEnumerable<Event> events)
        {
            var mergeSql = @"MERGE INTO EventStreams AS TARGET
                        USING (SELECT @StreamId StreamId) AS SOURCE
                        ON SOURCE.StreamId = TARGET.StreamId
                        WHEN NOT MATCHED THEN
                        INSERT(StreamId)
                        VALUES(StreamId);";

            var insertSql = @"INSERT INTO Events(StreamId, Created, Type, Data, Version)
                            VALUES ";

            var sb = new StringBuilder();
            sb.AppendLine(mergeSql);
            sb.Append(insertSql);

            var last = events.Last();
            foreach (var e in events)
            {
                sb.Append($"('{e.StreamId}', '{e.Created.ToString("yyyy-MM-dd hh:mm:ss")}', '{e.Type}', '{e.Data}', {e.Version})");
                if (e != last) sb.Append(", ");
            }

            using var connection = GetConnection();
            await connection.ExecuteAsync(sb.ToString(), new { StreamId = last.StreamId });
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(Guid streamId, long? fromVersion = null, long? toVersion = null)
        {
            var sql = @"SELECT * 
                        FROM Events
                        WHERE StreamId = @streamId
                            AND (@fromVersion IS NULL OR Version >= @fromVersion )
                            AND (@toVersion IS NULL OR Version <= @toVersion)";

            using var connection = GetConnection();
            return await connection.QueryAsync<Event>(sql, new { streamId, fromVersion, toVersion });
        }

        public Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            return Task.FromResult((Snapshot)null);
        }

        public async Task<EventStream> GetStreamAsync(Guid streamId)
        {
            var sql = "SELECT * FROM EventStreams WHERE StreamId = @streamId";

            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<EventStream>(sql, new { streamId });
        }

        public Task SaveSnapshot(Snapshot snapshot)
        {
            return Task.CompletedTask;
        }

        private void EnsureCreated()
        {
            var sql = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EventStreams' and xtype='U')
                            CREATE TABLE [dbo].[EventStreams](
                                [StreamId] [uniqueidentifier] PRIMARY KEY NOT NULL
                            );

                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Events' and xtype='U')
                            CREATE TABLE Events (
                                [Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	                            [StreamId] [uniqueidentifier] NOT NULL,
	                            [Created] [datetime2](7) NOT NULL,
	                            [Type] [nvarchar](max) NOT NULL,
	                            [Data] [nvarchar](max) NOT NULL,
	                            [Version] [bigint] NOT NULL,
                                CONSTRAINT [FK_Events_EventStreams_StreamId] FOREIGN KEY(StreamId)
                                REFERENCES EventStreams (StreamId)
                            );";

            using var connection = GetConnection();
            connection.Execute(sql);
        }

        private SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }
    }
}
