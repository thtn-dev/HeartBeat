namespace HeartBeat.Infrastructure.Caching.Enums;

public enum CachePattern
{
    /// <summary>
    /// Cache-aside: Check cache first, if miss load from DB and cache
    /// </summary>
    CacheAside,

    /// <summary>
    /// Write-through: Write to cache and DB simultaneously
    /// </summary>
    WriteThrough,

    /// <summary>
    /// Write-behind: Write to cache immediately, DB write is async
    /// </summary>
    WriteBehind,

    /// <summary>
    /// Write-around: Write directly to DB, bypass cache
    /// </summary>
    WriteAround,

    /// <summary>
    /// Refresh-ahead: Proactively refresh cache before expiration
    /// </summary>
    RefreshAhead
}

public enum CacheInvalidationStrategy
{
    /// <summary>
    /// Remove specific cache entry
    /// </summary>
    DirectInvalidation,

    /// <summary>
    /// Tag-based invalidation for related entries
    /// </summary>
    TagBasedInvalidation,

    /// <summary>
    /// Time-based expiration
    /// </summary>
    TimeBasedExpiration,

    /// <summary>
    /// Event-driven invalidation
    /// </summary>
    EventDrivenInvalidation,

    /// <summary>
    /// Manual invalidation by pattern
    /// </summary>
    PatternBasedInvalidation
}

public enum CacheWarmingStrategy
{
    /// <summary>
    /// Warm cache during application startup
    /// </summary>
    ApplicationStartup,

    /// <summary>
    /// Warm cache when user first logs in
    /// </summary>
    UserLogin,

    /// <summary>
    /// Warm cache based on usage patterns
    /// </summary>
    PredictiveWarming,

    /// <summary>
    /// Warm cache during off-peak hours
    /// </summary>
    ScheduledWarming,

    /// <summary>
    /// Warm cache on demand when cache miss occurs
    /// </summary>
    OnDemandWarming
}