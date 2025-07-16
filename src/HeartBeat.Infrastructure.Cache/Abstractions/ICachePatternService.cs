namespace HeartBeat.Infrastructure.Caching.Abstractions;
public interface ICachePatternService
{
    // Cache-aside pattern
    Task<T?> GetWithCacheAsideAsync<T>(string key, Func<Task<T?>> dataFactory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    // Write-through pattern
    Task SetWithWriteThroughAsync<T>(string key, T value, Func<Task> dbWriter, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    // Bulk operations
    Task<Dictionary<string, T?>> GetBulkWithCacheAsideAsync<T>(Dictionary<string, Func<Task<T?>>> keyFactoryMap, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
}