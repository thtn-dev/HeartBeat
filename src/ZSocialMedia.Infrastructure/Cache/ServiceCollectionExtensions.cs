using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ZSocialMedia.Infrastructure.Cache;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Redis distributed caching services to the DI container
    /// </summary>
    public static IServiceCollection AddRedisDistributedCache(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var redisConfig = serviceProvider.GetRequiredService<IOptions<RedisConfiguration>>().Value;

        // Register Redis connection multiplexer
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IConnectionMultiplexer>>();
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { redisConfig.ConnectionString },
                ConnectTimeout = redisConfig.ConnectTimeout * 1000, // Convert to milliseconds
                ConnectRetry = redisConfig.ConnectRetry,
                KeepAlive = redisConfig.KeepAlive,
                DefaultDatabase = redisConfig.Database,
                ClientName = redisConfig.ApplicationName,
                Ssl = redisConfig.UseSsl,
                AbortOnConnectFail = false, // Important for resilience
                ReconnectRetryPolicy = new ExponentialRetry(5000), // 5 second base delay
                SocketManager = SocketManager.ThreadPool
            };

            // Add password if provided
            if (!string.IsNullOrEmpty(redisConfig.Password))
            {
                configurationOptions.Password = redisConfig.Password;
            }

            // Enable logging if configured
            if (redisConfig.EnableLogging)
            {
                configurationOptions.ChannelPrefix = RedisChannel.Literal(redisConfig.ApplicationName);
            }

            try
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);

                // Log connection events
                connectionMultiplexer.ConnectionFailed += (sender, args) =>
                {
                    logger.LogError("Redis connection failed: {EndPoint} - {FailureType}",
                        args.EndPoint, args.FailureType);
                };

                connectionMultiplexer.ConnectionRestored += (sender, args) =>
                {
                    logger.LogInformation("Redis connection restored: {EndPoint}", args.EndPoint);
                };

                connectionMultiplexer.ErrorMessage += (sender, args) =>
                {
                    logger.LogError("Redis error: {Message} on {EndPoint}", args.Message, args.EndPoint);
                };

                logger.LogInformation("Redis connection established successfully");
                return connectionMultiplexer;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to connect to Redis at {ConnectionString}",
                    redisConfig.ConnectionString);
                throw;
            }
        });

        // Register Microsoft's distributed cache implementation
        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () =>
                Task.FromResult(services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>());
            options.InstanceName = redisConfig.ApplicationName;
        });

        // Register our enhanced cache service
        services.AddScoped<IDistributedCacheService, RedisDistributedCacheService>();


        return services;
    }
}
