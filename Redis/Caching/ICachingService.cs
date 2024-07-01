using FClub.Core.Ioc;

namespace FClub.Core.Services.Caching;

public interface ICachingService : IScopedDependency
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task SetAsync(string key, object data, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}