using Dapper;
using FancyEventStore.EventStore.Abstractions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    public class Test1DummyDapper : Test1Base
    {
        private readonly string _connectionString;

        public Test1DummyDapper(IEventStore eventStore, string resultFileName, string connectionString) : base(eventStore, resultFileName)
        {
            _connectionString = connectionString;
        }

        protected override async Task CleanData()
        {
            var sql = @"ALTER TABLE [dbo].[Events] DROP CONSTRAINT [FK_Events_EventStreams_StreamId]

                        TRUNCATE TABLE Events;
                        TRUNCATE TABLE EventStreams;

                        ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_EventStreams_StreamId] FOREIGN KEY([StreamId])
                        REFERENCES [dbo].[EventStreams] ([StreamId])


                        ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_EventStreams_StreamId]
                        ";

            var connection = new SqlConnection(_connectionString);
            connection.Open();
            await connection.ExecuteAsync(sql);
        }
    }
}
