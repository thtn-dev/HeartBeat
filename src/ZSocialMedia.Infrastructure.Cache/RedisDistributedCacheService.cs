using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.IO.Compression;
using System.Text.Json;
using System.Text;
using ZSocialMedia.Infrastructure.Caching.Abstractions;

namespace ZSocialMedia.Infrastructure.Caching;

public class RedisDistributedCacheService : IDistributedCacheService, IDisposable
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDistributedCache _distributedCache;
    private readonly CacheConfiguration _cacheConfig;
    private readonly ILogger<RedisDistributedCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisDistributedCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        IDistributedCache distributedCache,
        IOptions<CacheConfiguration> cacheConfig,
        ILogger<RedisDistributedCacheService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _distributedCache = distributedCache;
        _cacheConfig = cacheConfig.Value;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    #region String Operations

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            var value = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (value != null)
            {
                _logger.LogDebug("Cache hit for key: {Key}", cacheKey);
                return DecompressIfNeeded(value);
            }

            _logger.LogDebug("Cache miss for key: {Key}", cacheKey);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting string from cache with key: {Key}", key);
            return null;
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            var compressedValue = CompressIfNeeded(value);
            var options = CreateDistributedCacheEntryOptions(expiration);

            await _distributedCache.SetStringAsync(cacheKey, compressedValue, options, cancellationToken);
            _logger.LogDebug("Set string in cache with key: {Key}, expiration: {Expiration}", cacheKey, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting string in cache with key: {Key}", key);
            throw;
        }
    }

    #endregion

    #region Generic Object Operations

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = await GetStringAsync(key, cancellationToken);
            if (json == null) return null;

            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            _logger.LogDebug("Deserialized object of type {Type} from cache with key: {Key}", typeof(T).Name, key);
            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing object from cache with key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await SetStringAsync(key, json, expiration, cancellationToken);
            _logger.LogDebug("Serialized and cached object of type {Type} with key: {Key}", typeof(T).Name, key);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error serializing object for cache with key: {Key}", key);
            throw;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for get-or-set with key: {Key}", key);
            return cached;
        }

        _logger.LogDebug("Cache miss for get-or-set, executing factory for key: {Key}", key);
        var value = await factory();
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    #endregion

    #region Cache Management

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogDebug("Removed from cache with key: {Key}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from cache with key: {Key}", key);
            throw;
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: BuildKey(pattern));

            var batch = _database.CreateBatch();
            var tasks = keys.Select(key => batch.KeyDeleteAsync(key)).ToArray();
            batch.Execute();

            await Task.WhenAll(tasks);
            _logger.LogDebug("Removed keys matching pattern: {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys by pattern: {Pattern}", pattern);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            return await _database.KeyExistsAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking key existence: {Key}", key);
            return false;
        }
    }

    public async Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            return await _database.KeyTimeToLiveAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expiration for key: {Key}", key);
            return null;
        }
    }

    public async Task ExtendExpirationAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.KeyExpireAsync(cacheKey, expiration);
            _logger.LogDebug("Extended expiration for key: {Key} to {Expiration}", cacheKey, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending expiration for key: {Key}", key);
            throw;
        }
    }

    #endregion

    #region Hash Operations

    public async Task HashSetAsync(string key, string field, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.HashSetAsync(cacheKey, field, value);
            _logger.LogDebug("Set hash field {Field} for key: {Key}", field, cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting hash field {Field} for key: {Key}", field, key);
            throw;
        }
    }

    public async Task<string?> HashGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            return await _database.HashGetAsync(cacheKey, field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash field {Field} for key: {Key}", field, key);
            return null;
        }
    }

    public async Task<Dictionary<string, string?>> HashGetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            var hashEntries = await _database.HashGetAllAsync(cacheKey);
            return hashEntries.ToDictionary(x => (string)x.Name!, x => (string?)x.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all hash fields for key: {Key}", key);
            return new Dictionary<string, string?>();
        }
    }

    public async Task HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.HashDeleteAsync(cacheKey, field);
            _logger.LogDebug("Deleted hash field {Field} for key: {Key}", field, cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hash field {Field} for key: {Key}", field, key);
            throw;
        }
    }

    #endregion

    #region List Operations

    public async Task ListLeftPushAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.ListLeftPushAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing to left of list with key: {Key}", key);
            throw;
        }
    }

    public async Task ListRightPushAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.ListRightPushAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing to right of list with key: {Key}", key);
            throw;
        }
    }

    public async Task<string[]> ListRangeAsync(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            var values = await _database.ListRangeAsync(cacheKey, start, stop);
            return values.Select(x => (string)x!).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list range for key: {Key}", key);
            return Array.Empty<string>();
        }
    }

    public async Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            return await _database.ListLengthAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list length for key: {Key}", key);
            return 0;
        }
    }

    public async Task ListTrimAsync(string key, long start, long stop, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.ListTrimAsync(cacheKey, start, stop);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trimming list for key: {Key}", key);
            throw;
        }
    }

    #endregion

    #region Set Operations

    public async Task SetAddAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.SetAddAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to set with key: {Key}", key);
            throw;
        }
    }

    public async Task SetRemoveAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            await _database.SetRemoveAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from set with key: {Key}", key);
            throw;
        }
    }

    public async Task<bool> SetContainsAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            return await _database.SetContainsAsync(cacheKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking set membership for key: {Key}", key);
            return false;
        }
    }

    public async Task<string[]> SetMembersAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            var values = await _database.SetMembersAsync(cacheKey);
            return values.Select(x => (string)x!).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting set members for key: {Key}", key);
            return Array.Empty<string>();
        }
    }

    public async Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = BuildKey(key);
            return await _database.SetLengthAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting set length for key: {Key}", key);
            return 0;
        }
    }

    #endregion

    #region Batch Operations

    public async Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class
    {
        var tasks = keys.Select(async key => new { Key = key, Value = await GetAsync<T>(key, cancellationToken) });
        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(x => x.Key, x => x.Value);
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> keyValuePairs, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var tasks = keyValuePairs.Select(kvp => SetAsync(kvp.Key, kvp.Value, expiration, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var tasks = keys.Select(key => RemoveAsync(key, cancellationToken));
        await Task.WhenAll(tasks);
    }

    #endregion

    #region Utility

    public string BuildKey(params string[] segments)
    {
        var keySegments = new List<string> { _cacheConfig.KeyPrefix };
        keySegments.AddRange(segments);
        return string.Join(_cacheConfig.KeySeparator, keySegments);
    }

    public async Task<Dictionary<string, object>> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var info = await server.InfoAsync();

            return new Dictionary<string, object>
            {
                ["redis_version"] = info.First(x => x.Key == "redis_version"),
                ["used_memory"] = info.First(x => x.Key == "used_memory"),
                ["used_memory_human"] = info.First(x => x.Key == "used_memory_human"),
                ["connected_clients"] = info.First(x => x.Key == "connected_clients"),
                ["total_commands_processed"] = info.First(x => x.Key == "total_commands_processed"),
                ["keyspace_hits"] = info.First(x => x.Key == "keyspace_hits"),
                ["keyspace_misses"] = info.First(x => x.Key == "keyspace_misses")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Redis info");
            return new Dictionary<string, object>();
        }
    }

    #endregion

    #region Helper Methods

    private DistributedCacheEntryOptions CreateDistributedCacheEntryOptions(TimeSpan? expiration)
    {
        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheConfig.DefaultExpirationMinutes);
        }

        // Set sliding expiration for frequently accessed items
        options.SlidingExpiration = TimeSpan.FromMinutes(_cacheConfig.SlidingExpirationMinutes);

        return options;
    }

    private string CompressIfNeeded(string value)
    {
        if (!_cacheConfig.EnableCompression || value.Length < _cacheConfig.CompressionThreshold)
        {
            return value;
        }

        try
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using var inputStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(outputStream, CompressionLevel.Fastest);

            inputStream.CopyTo(gzipStream);
            gzipStream.Close();

            var compressedBytes = outputStream.ToArray();
            var compressedString = Convert.ToBase64String(compressedBytes);

            // Only use compression if it actually reduces size
            if (compressedString.Length < value.Length)
            {
                _logger.LogDebug("Compressed value from {OriginalSize} to {CompressedSize} bytes",
                    value.Length, compressedString.Length);
                return $"GZIP:{compressedString}";
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to compress value, using original");
            return value;
        }
    }

    private string DecompressIfNeeded(string value)
    {
        if (!value.StartsWith("GZIP:"))
        {
            return value;
        }

        try
        {
            var compressedString = value.Substring(5); // Remove "GZIP:" prefix
            var compressedBytes = Convert.FromBase64String(compressedString);

            using var inputStream = new MemoryStream(compressedBytes);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            gzipStream.CopyTo(outputStream);
            var decompressedBytes = outputStream.ToArray();

            return Encoding.UTF8.GetString(decompressedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decompress value");
            return value;
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        _connectionMultiplexer?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}
