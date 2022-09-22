using FancyEventStore.DapperProductionStore;

var connectionString = "Data Source=LAPTOP-DDOSKACH;Initial Catalog = FancyEventStoreDb; Integrated Security = True; Connect Timeout = 3600";
var context = new DbContext(connectionString);

context.EnsureCreated();