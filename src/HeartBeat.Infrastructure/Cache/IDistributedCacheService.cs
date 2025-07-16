namespace HeartBeat.Infrastructure.Cache;
public interface IDistributedCacheService
{
    #region String Operations

    /// <summary>
    /// Get value by key as string
    /// </summary>
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set string value with optional expiration
    /// </summary>
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    #endregion

    #region Generic Object Operations

    /// <summary>
    /// Get and deserialize object from cache
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Serialize and store object in cache
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Get or create pattern - fetch from cache or execute factory function
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    #endregion

    #region Cache Management

    /// <summary>
    /// Remove item from cache
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove multiple items by pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get TTL (time to live) for a key
    /// </summary>
    Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extend expiration time for existing key
    /// </summary>
    Task ExtendExpirationAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);

    #endregion

    #region Hash Operations (for complex objects)

    /// <summary>
    /// Set field in hash
    /// </summary>
    Task HashSetAsync(string key, string field, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get field from hash
    /// </summary>
    Task<string?> HashGetAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all fields from hash
    /// </summary>
    Task<Dictionary<string, string?>> HashGetAllAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete field from hash
    /// </summary>
    Task HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default);

    #endregion

    #region List Operations (for feeds/timelines)

    /// <summary>
    /// Add item to the left (beginning) of list
    /// </summary>
    Task ListLeftPushAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add item to the right (end) of list
    /// </summary>
    Task ListRightPushAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list range (for pagination)
    /// </summary>
    Task<string[]> ListRangeAsync(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list length
    /// </summary>
    Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trim list to specified range
    /// </summary>
    Task ListTrimAsync(string key, long start, long stop, CancellationToken cancellationToken = default);

    #endregion

    #region Set Operations (for followers/following)

    /// <summary>
    /// Add member to set
    /// </summary>
    Task SetAddAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove member from set
    /// </summary>
    Task SetRemoveAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if member exists in set
    /// </summary>
    Task<bool> SetContainsAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all members from set
    /// </summary>
    Task<string[]> SetMembersAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get set cardinality (count)
    /// </summary>
    Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default);

    #endregion

    #region Batch Operations

    /// <summary>
    /// Get multiple keys at once
    /// </summary>
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Set multiple key-value pairs at once
    /// </summary>
    Task SetManyAsync<T>(Dictionary<string, T> keyValuePairs, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Remove multiple keys at once
    /// </summary>
    Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    #endregion

    #region Utility

    /// <summary>
    /// Generate cache key with prefix
    /// </summary>
    string BuildKey(params string[] segments);

    /// <summary>
    /// Get cache statistics/info
    /// </summary>
    Task<Dictionary<string, object>> GetInfoAsync(CancellationToken cancellationToken = default);

    #endregion
}