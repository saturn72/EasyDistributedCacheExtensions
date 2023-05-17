namespace Microsoft.Extensions.Caching.Distributed
{

    public static class DistributedCacheExtensionsClass
    {
        private static DistributedCacheEntryOptions DefaultOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
            SlidingExpiration = TimeSpan.FromHours(2),
        };

        public static async Task<(bool, T?)> TryGetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            var value = await cache.GetAsync(key, cancellationToken);
            if (value == null)
            {
                return (false, default);
            }
            try
            {
                return (true, JsonSerializer.Deserialize<T>(value));
            }
            catch { }

            return (false, default);
        }

        public static async Task<T?> GetAsync<T>(
            this IDistributedCache cache,
            string key,
            Func<Task<T>> acquire,
            DistributedCacheEntryOptions options = default,
            CancellationToken cancellationToken = default)
        {
            var value = await cache.GetAsync(key, cancellationToken);
            if (value != default)
                return JsonSerializer.Deserialize<T>(value);

            var a = await acquire();
            if (a == null)
                return default;

            var bytes = JsonSerializer.SerializeToUtf8Bytes(a);
            await cache.SetAsync(key, bytes, options ?? DefaultOptions, cancellationToken);
            return a;
        }

        public static Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default) =>
            GetOrDefaultAsync<T>(cache, key, default, cancellationToken);

        public static async Task<T?> GetOrDefaultAsync<T>(this IDistributedCache cache, string key, T? defaultValue = default, CancellationToken cancellationToken = default)
        {
            var value = await cache.GetAsync(key, cancellationToken);
            if (value == null)
            {
                return defaultValue;
            }
            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {

            }
            return default;
        }
    }
}