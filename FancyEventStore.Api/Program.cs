
using FancyEventStore.Common;
using FancyEventStore.EfCoreStore;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Serializers;
using FancyEventStore.ReadModel;
using FancyEventStore.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Meta;
using System.Data;
using System.Data.Common;
using System.Reflection;

RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("FancyEventStoreDb");
builder.Services.AddEventStore(Assembly.GetExecutingAssembly(),
    opts =>
    {
        opts.UseEfCore(dbContextOptions => dbContextOptions.UseSqlServer(connectionString));
        opts.EventSerializer = EventSerializers.Json;
    });
builder.Services.AddRepositories();
builder.Services.AddScoped<IReadModelContext>(_ => new ReadModelContext(connectionString));

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
