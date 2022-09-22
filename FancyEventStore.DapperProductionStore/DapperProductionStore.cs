using Dapper;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using System.Data.SqlClient;
using System.Text;

namespace FancyEventStore.DapperProductionStore
{
    public class DapperProductionStore : IStore
    {
        private readonly IDbContext _context;
        private const string sqlDateFormat = "yyyy-MM-dd hh:mm:ss";

        public DapperProductionStore(IDbContext context)
        {
            _context = context;
        }

        public async Task AppendEventsAsync(EventStream stream, IEnumerable<Event> events)
        {
            var sql =
                @"BEGIN TRANSACTION
                DECLARE @StreamTimestamp ROWVERSION;

                SELECT @StreamTimestamp = Timestamp
                FROM EventStreams
                WHERE StreamId = @StreamId;

                IF @StreamTimestamp IS NOT NULL AND @StreamTimestamp != @Timestamp
	                THROW 424242, 'Concurrency check failed', 1;

                IF @StreamTimestamp IS NULL     
	                INSERT INTO EventStreams(StreamId, Version)
	                VALUES (@StreamId, @Version);
                ELSE 
	                UPDATE EventStreams
	                SET Version = @Version
	                WHERE StreamId = @StreamId;

                INSERT INTO Events(StreamId, Created, Type, Data, Version)
                VALUES ";


            var values = events.Select((x, i) => $"('{x.StreamId}', '{x.Created.ToString(sqlDateFormat)}', '{x.Type}', '{x.Data}', {x.Version})").ToList();
            var valuesString = string.Join(", \n", values);

            var queryBuilder = new StringBuilder(sql);
            queryBuilder.Append(valuesString);
            queryBuilder.AppendLine(";");
            queryBuilder.Append("COMMIT;");

            var finalSql = queryBuilder.ToString();

            try
            {
                await _context.Connection.ExecuteAsync(finalSql, new { stream.StreamId, stream.Version, stream.Timestamp });
            }
            catch(SqlException ex) when (ex.Number == 424242)
            {
                throw new EventStoreConcurrencyException();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(Guid streamId, long? fromVersion = null, long? toVersion = null)
        {
            var sql = 
                @"SELECT * 
                FROM Events
                WHERE StreamId = @streamId
                    AND (@fromVersion IS NULL OR Version >= @fromVersion )
                    AND (@toVersion IS NULL OR Version <= @toVersion)";

            return await _context.Connection.QueryAsync<Event>(sql, new { streamId, fromVersion, toVersion });
        }

        public Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            return null;
        }

        public async Task<EventStream> GetStreamAsync(Guid streamId)
        {
            var sql = "SELECT * FROM EventStreams WHERE StreamId = @streamId";
            return await _context.Connection.QueryFirstAsync<EventStream>(sql, new { streamId });
        }

        public Task SaveSnapshot(Snapshot snapshot)
        {
            return Task.CompletedTask;
        }
    }
}
