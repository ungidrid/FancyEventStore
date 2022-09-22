﻿using FancyEventStore.EfCoreStore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests
{
    public static class Configuration
    {
        public const string SqlConnectionString = "Server=127.0.0.1\\mssql,5434;Database=EventStore;User Id=SA;Password=Pass@word;";//"Data Source=LAPTOP-DDOSKACH;Initial Catalog=FancyEventStoreDb;Integrated Security=True;Connect Timeout=3600;";
        public const string UnsafeSqlConnectionString = "Server=127.0.0.1\\mssql,5434;Database=EventStore;User Id=SA;Password=Pass@word;";//"Data Source=LAPTOP-DDOSKACH;Initial Catalog = FancyEventStoreUnsafeDb; Integrated Security = True; Connect Timeout = 3600";
        public const string MongoConnectionString = "mongodb://rootuser:rootpass@localhost:27017"; //"mongodb+srv://root:prokiller00@eventstore.rf4fems.mongodb.net/test";
    }
}