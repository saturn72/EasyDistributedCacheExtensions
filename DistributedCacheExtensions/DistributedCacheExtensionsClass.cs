namespace Microsoft.Extensions.Caching.Distributed
{

    public static class DistributedCacheExtensionsClass
    {
        public static async Task<(bool, T?)> TryGetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            var value = await cache.GetAsync(key, token);
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

        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            var value = await cache.GetAsync(key, token);
            if (value == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(value);
        }

        public static async Task<T?> GetOrDefaultAsync<T>(this IDistributedCache cache, string key, T? defaultValue = default, CancellationToken token = default)
        {
            var value = await cache.GetAsync(key, token);
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