using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using WebApp.Audit;
using WebApp.Audit.Events;
using WebApp.Audit.Repositories;
using WebApp.Audit.Services;
using WebApp.Data;
using WebApp.Database;
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

builder.Services.AddScoped<IHistoryContext, HistoryContext>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<EntityChangedPublisher>(provider =>
{
    var publisher = new EntityChangedPublisher();
    var auditLogService = provider.GetRequiredService<IAuditLogService>();
    publisher.Subscribe(auditLogService);
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
