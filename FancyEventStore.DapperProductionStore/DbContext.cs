using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DapperProductionStore
{
    public class DbContext: IDbContext
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
            EnsureCreated();
        }

        public IDbConnection Connection => _connection ?? OpenConnection();

        public void EnsureCreated()
        {
            var createTablesSql =
                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EventStreams' and xtype='U')
                BEGIN
                    CREATE TABLE [dbo].[EventStreams](
	                    [StreamId] [uniqueidentifier] NOT NULL,
	                    [Version] [bigint] NOT NULL,
	                    [Timestamp] [timestamp] NULL,
                        CONSTRAINT [PK_EventStreams] PRIMARY KEY CLUSTERED 
                    (
	                    [StreamId] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                    ) ON [PRIMARY];
                END;

                
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Events' AND xtype='U')
                BEGIN
                    CREATE TABLE [dbo].[Events](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [StreamId] [uniqueidentifier] NOT NULL,
	                    [Created] [datetime2](7) NOT NULL,
	                    [Type] [nvarchar](max) NOT NULL,
	                    [Data] [nvarchar](max) NOT NULL,
	                    [Version] [bigint] NOT NULL,
                        CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
                    (
	                    [Id] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                    ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_EventStreams_StreamId] FOREIGN KEY([StreamId])
                    REFERENCES [dbo].[EventStreams] ([StreamId])
                    ON DELETE CASCADE;

                    ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_EventStreams_StreamId];
                END;

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Snapshots' AND xtype='U')
                BEGIN
                    CREATE TABLE [dbo].[Snapshots](
	                    [ShapshotId] [int] IDENTITY(1,1) NOT NULL,
	                    [StreamId] [uniqueidentifier] NOT NULL,
	                    [Data] [nvarchar](max) NULL,
	                    [Version] [bigint] NOT NULL,
	                    [CreatedAt] [datetime2](7) NOT NULL,
                        CONSTRAINT [PK_Snapshots] PRIMARY KEY CLUSTERED 
                    (
	                    [ShapshotId] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                    ALTER TABLE [dbo].[Snapshots] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [CreatedAt];

                    ALTER TABLE [dbo].[Snapshots]  WITH CHECK ADD  CONSTRAINT [FK_Snapshots_EventStreams_StreamId] FOREIGN KEY([StreamId])
                    REFERENCES [dbo].[EventStreams] ([StreamId])
                    ON DELETE CASCADE;

                    ALTER TABLE [dbo].[Snapshots] CHECK CONSTRAINT [FK_Snapshots_EventStreams_StreamId];
                END;";

            Connection.Execute(createTablesSql);
        }

        private IDbConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            _connection = connection;
            return connection;
        }
    }
}
