using HeartBeat.Infrastructure.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace HeartBeat.Infrastructure.Caching;

public class CachePatternService : ICachePatternService
{
    private readonly IDistributedCacheService _cache;
    private readonly ILogger<CachePatternService> _logger;
    public CachePatternService(
        IDistributedCacheService cache,
        IOptions<CacheConfiguration> config,
        ILogger<CachePatternService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    #region Cache-Aside Pattern

    public async Task<T?> GetWithCacheAsideAsync<T>(
        string key,
        Func<Task<T?>> dataFactory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // 1. Check cache first
            var cached = await _cache.GetAsync<T>(key, cancellationToken);
            if (cached != null)
            {
                _logger.LogDebug("Cache hit for cache-aside pattern: {Key}", key);
                return cached;
            }

            // 2. Cache miss - load from data source
            _logger.LogDebug("Cache miss for cache-aside pattern: {Key}, loading from data source", key);
            var data = await dataFactory();

            if (data != null)
            {
                // 3. Store in cache for future requests
                await _cache.SetAsync(key, data, expiration, cancellationToken);
                _logger.LogDebug("Stored data in cache for cache-aside pattern: {Key}", key);
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache-aside pattern for key: {Key}", key);
            // Fallback to data source if cache fails
            return await dataFactory();
        }
    }

    #endregion

    #region Write-Through Pattern

    public async Task SetWithWriteThroughAsync<T>(
        string key,
        T value,
        Func<Task> dbWriter,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // 1. Write to database first (ensures consistency)
            await dbWriter();
            _logger.LogDebug("Database write completed for write-through pattern: {Key}", key);

            // 2. Write to cache
            await _cache.SetAsync(key, value, expiration, cancellationToken);
            _logger.LogDebug("Cache write completed for write-through pattern: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in write-through pattern for key: {Key}", key);

            // If cache write fails, ensure data is still in DB
            try
            {
                await dbWriter();
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Database write also failed for key: {Key}", key);
                throw;
            }

            // Don't fail the operation if only cache write fails
        }
    }

    #endregion
   
    #region Bulk Operations

    public async Task<Dictionary<string, T?>> GetBulkWithCacheAsideAsync<T>(
        Dictionary<string, Func<Task<T?>>> keyFactoryMap,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var result = new Dictionary<string, T?>();
        var uncachedKeys = new List<string>();

        // 1. Check cache for all keys
        var cachedData = await _cache.GetManyAsync<T>(keyFactoryMap.Keys, cancellationToken);

        foreach (var kvp in keyFactoryMap)
        {
            if (cachedData.TryGetValue(kvp.Key, out var cached) && cached != null)
            {
                result[kvp.Key] = cached;
            }
            else
            {
                uncachedKeys.Add(kvp.Key);
            }
        }

        _logger.LogDebug("Bulk cache operation: {CacheHits} hits, {CacheMisses} misses",
            result.Count, uncachedKeys.Count);

        // 2. Load uncached data
        if (uncachedKeys.Any())
        {
            var uncachedTasks = uncachedKeys.Select(async key =>
            {
                var data = await keyFactoryMap[key]();
                return new { Key = key, Data = data };
            });

            var uncachedResults = await Task.WhenAll(uncachedTasks);
            var dataToCache = new Dictionary<string, T>();

            foreach (var item in uncachedResults)
            {
                result[item.Key] = item.Data;
                if (item.Data != null)
                {
                    dataToCache[item.Key] = item.Data;
                }
            }

            // 3. Cache the loaded data
            if (dataToCache.Any())
            {
                await _cache.SetManyAsync(dataToCache, expiration, cancellationToken);
            }
        }

        return result;
    }

    #endregion
}