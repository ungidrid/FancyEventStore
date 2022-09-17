using FancyEventStore.Common;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Serializers;
using FancyEventStore.MongoDbStore;
using FancyEventStore.ReadModel;
using FancyEventStore.Repositories;
using ProtoBuf.Meta;
using System.Reflection;

RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionString = builder.Configuration.GetConnectionString("FancyEventStoreDb");
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbEventStore");
builder.Services.AddEventStore(Assembly.GetExecutingAssembly(),
    opts =>
    {
        //opts.UseEfCore(dbContextOptions => dbContextOptions.UseSqlServer(connectionString));
        opts.UseMongoDb(mongoConnectionString);
        opts.EventSerializer = EventSerializers.Json;
    });

builder.Services.AddRepositories();
builder.Services.AddScoped<IReadModelContext>(_ => new ReadModelContext(sqlConnectionString));

var app = builder.Build();

app.Use((context, next) =>
{
    try
    {
        return next();
    }
    catch
    {
        return null;
    }
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
