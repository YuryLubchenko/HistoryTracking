using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data;
using HistoryTracking.Audit;
using Microsoft.FeatureManagement;
using WebApp.Data;
using WebApp.Database;
using WebApp.Entities;
using WebApp.Events;
using WebApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
    options
        .UsePostgreSQL(builder.Configuration.GetConnectionString("Default")!)
        .UseDefaultLogging(provider));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<DataConnection>(provider => provider.GetRequiredService<AppDataConnection>());

builder.Services.AddFeatureManagement();

builder.Services.AddAudit(
    options => { options.FeatureToggleName = "AuditEnabled"; },
    configure: b => b.ApplyConfigurationsFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<IEntityChangedHandler<BaseEntity>, AuditSubscriber>();
builder.Services.AddScoped<EntityChangedPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDataConnection>();
    DatabaseInitializer.ApplyScripts(db, app.Environment);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
