using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace HeartBeat.Infrastructure.Database;

public static class DatabaseServiceExtensions
{
    // <summary>
    /// Registers database services with dynamic connection pooling
    /// This method configures Entity Framework, connection pooling, and health checks
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with connection pooling
        services.AddDbContextPool<AppDbContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            // Build connection string with dynamic pool parameters
            var connectionString = BuildConnectionString(dbOptions);

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);

                npgsqlOptions.CommandTimeout(30);
            });
            options.EnableSensitiveDataLogging(dbOptions.EnableSensitiveDataLogging);
            options.EnableDetailedErrors(dbOptions.EnableDetailedErrors);
            options.LogTo(Console.WriteLine, LogLevel.Information);
            //// Configure logging based on environment
            //if (environment.IsDevelopment())
            //{
            //    options.EnableSensitiveDataLogging(dbOptions.EnableSensitiveDataLogging);
            //    options.EnableDetailedErrors(dbOptions.EnableDetailedErrors);
            //    options.LogTo(Console.WriteLine, LogLevel.Information);
            //}
            //else
            //{
            //    // Production logging - only warnings and errors
            //    options.LogTo(
            //        filter: (eventId, level) => level >= LogLevel.Warning,
            //        logger: (eventData) => logger.LogWarning(eventData.ToString()));
            //}
        },
        // Configure pool size for DbContext pooling
        poolSize: 128); // This is separate from database connection pooling

        return services;
    }

    /// <summary>
    /// Builds optimized connection string with dynamic pool parameters
    /// </summary>
    private static string BuildConnectionString(DatabaseOptions dbOptions)
    {
        var builder = new NpgsqlConnectionStringBuilder(dbOptions.ConnectionString)
        {
            // Dynamic connection pool settings
            MinPoolSize = dbOptions.ConnectionPool.MinPoolSize,
            MaxPoolSize = dbOptions.ConnectionPool.MaxPoolSize,
            ConnectionIdleLifetime = dbOptions.ConnectionPool.ConnectionIdleLifetime,
            ConnectionPruningInterval = 10, // Check every 10 seconds for idle connections

            // Performance optimizations for social network workload
            Timeout = dbOptions.ConnectionPool.ConnectionTimeout,
            CommandTimeout = 30,

            // Connection reliability

            // Enable multiplexing for better connection efficiency
            Multiplexing = true,

            // TCP keepalive settings for long-running connections
            TcpKeepAlive = true,
            TcpKeepAliveTime = 600, // 10 minutes
            TcpKeepAliveInterval = 30, // 30 seconds

            // Application name for monitoring
            ApplicationName = "SocialX-API",

            // Enable automatic prepare for repeated queries
            MaxAutoPrepare = 100,
            AutoPrepareMinUsages = 2
        };

        return builder.ConnectionString;
    }

}
