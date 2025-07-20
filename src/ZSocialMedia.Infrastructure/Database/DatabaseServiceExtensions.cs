using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ZSocialMedia.Infrastructure.Database;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IHostEnvironment environment
        )
    {
        services.AddDbContextPool<AppDbContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connectionString = BuildConnectionString(dbOptions);

            var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);

                npgsqlOptions.CommandTimeout(30);
            });

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging(dbOptions.EnableSensitiveDataLogging);
                options.EnableDetailedErrors(dbOptions.EnableDetailedErrors);
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
            else
            {
                options.LogTo(
                    filter: (eventId, level) => level >= LogLevel.Warning,
                    logger: (eventData) => logger.LogWarning("Database event: {EventData}", eventData.ToString()));
            }
        });

        return services;
    }

    private static string BuildConnectionString(DatabaseOptions dbOptions)
    {
        var builder = new NpgsqlConnectionStringBuilder(dbOptions.ConnectionString)
        {
            MinPoolSize = dbOptions.ConnectionPool.MinPoolSize,
            MaxPoolSize = dbOptions.ConnectionPool.MaxPoolSize,
            ConnectionIdleLifetime = dbOptions.ConnectionPool.ConnectionIdleLifetime,
            Timeout = dbOptions.ConnectionPool.ConnectionTimeout,
            // Multiplexing = true,
            TcpKeepAlive = true,
            TcpKeepAliveTime = 600,
            TcpKeepAliveInterval = 30,
            ApplicationName = "SocialX-API",
            MaxAutoPrepare = 100,
        };

        return builder.ConnectionString;
    }
}
