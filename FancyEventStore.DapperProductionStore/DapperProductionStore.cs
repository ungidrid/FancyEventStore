using Dapper;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using System.Data.Common;
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
                @"
                DECLARE @StreamTimestamp ROWVERSION;

                SELECT @StreamTimestamp = Timestamp
                FROM EventStreams WITH (UPDLOCK, ROWLOCK)
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
                
                ";

            var insertStatement =
                @"INSERT INTO Events(StreamId, Created, Type, Data, Version)
                VALUES ";

            var values = events.Select((x, i) => $"('{x.StreamId}', '{x.Created.ToString(sqlDateFormat)}', '{x.Type}', '{x.Data}', {x.Version})")
                .Chunk(999)
                .Select(x => string.Join(", \n", x))
                .ToList();

            var queryBuilder = new StringBuilder(sql);
            foreach(var value in values)
            {
                queryBuilder.Append(insertStatement);
                queryBuilder.Append(value);
                queryBuilder.AppendLine(";");
            }

            var finalSql = queryBuilder.ToString();

            try
            {
                using (var transaction = _context.Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var result = await _context.Connection.ExecuteAsync(finalSql, new { stream.StreamId, stream.Version, stream.Timestamp }, transaction);
                    transaction.Commit();
                }
            }
            catch(SqlException ex)
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

        public async Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            var sql =
                @"SELECT TOP(1) * 
                  FROM Snapshots
                  WHERE StreamId = @streamId AND (@version IS NULL OR Version <= @version)
                  ORDER BY Version DESC;";

            return await _context.Connection.QueryFirstOrDefaultAsync<Snapshot>(sql, new { streamId, version });
        }

        public async Task<EventStream> GetStreamAsync(Guid streamId)
        {
            var sql = "SELECT * FROM EventStreams WHERE StreamId = @streamId";
            return await _context.Connection.QueryFirstOrDefaultAsync<EventStream>(sql, new { streamId });
        }

        public async Task SaveSnapshot(Snapshot snapshot)
        {
            var sql =
                 @"INSERT INTO Snapshots(StreamId, Data, Version, CreatedAt)
                  VALUES (@StreamId, @Data, @Version, @CreatedAt);";

            await _context.Connection.ExecuteAsync(sql, snapshot);
        }
    }
}
