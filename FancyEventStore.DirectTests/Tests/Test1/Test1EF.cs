using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EfCoreStore;
using FancyEventStore.EventStore.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    public class Test1EF: Test1Base
    {
        public EfCoreStoreContext _context { get; }

        public Test1EF(IEventStore eventStore, EfCoreStoreContext context, string resultFileName) : base(eventStore, resultFileName)
        {
            _context = context;
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        protected override async Task CleanData()
        {
            var sql = @"DELETE FROM Events;
                        DELETE FROM EventStreams;

                        ALTER TABLE [dbo].[Snapshots] DROP CONSTRAINT [FK_Snapshots_EventStreams_StreamId];
                        ALTER TABLE [dbo].[Events] DROP CONSTRAINT [FK_Events_EventStreams_StreamId];

                        TRUNCATE TABLE Events;
                        TRUNCATE TABLE EventStreams;

                        ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_EventStreams_StreamId] FOREIGN KEY([StreamId])
                        REFERENCES [dbo].[EventStreams] ([StreamId])
                        ON DELETE CASCADE;

                        ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_EventStreams_StreamId];

                        ALTER TABLE [dbo].[Snapshots]  WITH CHECK ADD  CONSTRAINT [FK_Snapshots_EventStreams_StreamId] FOREIGN KEY([StreamId])
                        REFERENCES [dbo].[EventStreams] ([StreamId])
                        ON DELETE CASCADE;

                        ALTER TABLE [dbo].[Snapshots] CHECK CONSTRAINT [FK_Snapshots_EventStreams_StreamId];
                        ";

            await _context.Database.ExecuteSqlRawAsync(sql);
            _context.ChangeTracker.Clear();
        }
    }
}
