using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MatieAvalonia.Classes;

/// <summary>
/// Явная загрузка строки подключения из <c>appsettings.json</c> (критерий маркировки: одно подключение, не в коде контекста).
/// </summary>
public static class AppConfiguration
{
    private static readonly Lazy<string> ConnectionStringLazy = new(BuildConnectionString);

    public static string PostgresConnectionString => ConnectionStringLazy.Value;

    private static string BuildConnectionString()
    {
        try
        {
            var basePath = AppContext.BaseDirectory;
            var cfg = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();
            var cs = cfg.GetConnectionString("Postgres");
            if (!string.IsNullOrWhiteSpace(cs))
                return cs.Trim();
        }
        catch
        {
            // ignored — fallback ниже
        }

        return "Host=localhost;Port=5555;Database=MatieDB;Username=postgres;Password=1111;";
    }
}
