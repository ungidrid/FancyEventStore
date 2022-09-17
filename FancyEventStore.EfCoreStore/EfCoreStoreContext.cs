using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace FancyEventStore.EfCoreStore
{
    public class EfCoreStoreContext : DbContext
    {
        public EfCoreStoreContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventStream> EventStreams { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Type).IsRequired();
                builder.Property(x => x.Data).IsRequired();

                builder.HasOne(x => x.Stream)
                    .WithMany(x => x.Events)
                    .HasForeignKey(x => x.StreamId);

                builder.ToTable("Events");
            });

            modelBuilder.Entity<EventStream>(builder =>
            {
                builder.HasKey(x => x.StreamId);
                builder.Property(x => x.StreamId).ValueGeneratedNever();
                builder.Property(x => x.Timestamp).IsRowVersion();

                builder.ToTable("EventStreams");
            });

            modelBuilder.Entity<Snapshot>(builder =>
            {
                builder.HasKey(x => x.ShapshotId);
                builder.HasOne(x => x.EventStream)
                    .WithMany()
                    .HasForeignKey(x => x.StreamId);
            });
        }
    }
}
