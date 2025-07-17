namespace ZSocialMedia.Infrastructure.Cache;

public class RedisConfiguration
{
    public const string SectionName = "Redis";

    /// <summary>
    /// Redis connection string (e.g., "localhost:6379")
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Database number to use (0-15)
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    public int ConnectTimeout { get; set; } = 5;

    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    public int CommandTimeout { get; set; } = 5;

    /// <summary>
    /// Number of retry attempts for failed operations
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Enable detailed logging for debugging
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// SSL/TLS configuration
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Password for Redis authentication
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Application name for connection identification
    /// </summary>
    public string ApplicationName { get; set; } = "SocialXMedia";

    /// <summary>
    /// Pool size for connection multiplexer
    /// </summary>
    public int PoolSize { get; set; } = 5;

    /// <summary>
    /// Keep alive interval in seconds
    /// </summary>
    public int KeepAlive { get; set; } = 60;

    /// <summary>
    /// Maximum number of connection attempts
    /// </summary>
    public int ConnectRetry { get; set; } = 3;
}


public class CacheConfiguration
{
    public const string SectionName = "Cache";

    /// <summary>
    /// Default cache expiration time in minutes
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Sliding expiration time in minutes
    /// </summary>
    public int SlidingExpirationMinutes { get; set; } = 10;

    /// <summary>
    /// Key prefix for all cache entries
    /// </summary>
    public string KeyPrefix { get; set; } = "";

    /// <summary>
    /// Key separator character
    /// </summary>
    public string KeySeparator { get; set; } = ":";

    /// <summary>
    /// Enable cache compression for large values
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Minimum size in bytes to trigger compression
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024;

    /// <summary>
    /// Maximum cache value size in bytes (1MB default)
    /// </summary>
    public int MaxValueSize { get; set; } = 1024 * 1024;

    /// <summary>
    /// Cache-specific expiration settings
    /// </summary>
    public CacheExpirationSettings Expiration { get; set; } = new();
}

public class CacheExpirationSettings
{
    /// <summary>
    /// User profile cache expiration (minutes)
    /// </summary>
    public int UserProfile { get; set; } = 30;

    /// <summary>
    /// User session cache expiration (minutes)
    /// </summary>
    public int UserSession { get; set; } = 60 * 24; // 24 hours

    /// <summary>
    /// Feed data cache expiration (minutes)
    /// </summary>
    public int FeedData { get; set; } = 5;

    /// <summary>
    /// Popular content cache expiration (minutes)
    /// </summary>
    public int PopularContent { get; set; } = 60;

    /// <summary>
    /// Search results cache expiration (minutes)
    /// </summary>
    public int SearchResults { get; set; } = 15;

    /// <summary>
    /// Media metadata cache expiration (minutes)
    /// </summary>
    public int MediaMetadata { get; set; } = 60 * 6; // 6 hours
}