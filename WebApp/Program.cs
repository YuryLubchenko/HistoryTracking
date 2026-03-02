using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data;
using HistoryTracking.Audit;
using Microsoft.FeatureManagement;
using WebApp.Data;
using WebApp.Database;
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
    builder.Configuration.GetSection("Audit"),
    configure: b => b.ApplyConfigurationsFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<AuditSubscriber>();
builder.Services.AddScoped(provider =>
{
    var publisher = new EntityChangedPublisher();
    var auditSubscriber = provider.GetRequiredService<AuditSubscriber>();
    publisher.Subscribe(auditSubscriber);
    return publisher;
});

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
