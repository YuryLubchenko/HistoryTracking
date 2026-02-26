using LinqToDB.Data;
using WebApp.Data;

namespace WebApp.Database;

public static class DatabaseInitializer
{
    public static void ApplyScripts(AppDataConnection db, IWebHostEnvironment env)
    {
        var scriptsPath = Path.Combine(env.ContentRootPath, "Database", "Scripts");

        if (!Directory.Exists(scriptsPath))
            return;

        var scripts = Directory.GetFiles(scriptsPath, "*.sql")
            .OrderBy(f => f)
            .ToList();

        foreach (var script in scripts)
        {
            var sql = File.ReadAllText(script);
            db.Execute(sql);
        }
    }
}
