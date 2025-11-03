using LazyCache;
using Simulator.Server.Databases.Contracts;
using System.Collections.Concurrent;

namespace Simulator.Server.Repositories
{
    public interface ICache
    {
        string _tenantId { get; }
        Task<T> GetOrAddCacheAsync<T>(Func<Task<T>> addItemFactory, string key, string parentId = null!) where T : class; // ✅ solo "class"

        void InvalidateCache<T>();
    }
    public class Cache : ICache
    {
        private readonly IAppCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Cache> _logger;

        // Propiedad calculada bajo demanda
        public string _tenantId => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type.Contains("email"))?.Value ?? "default";

        // Thread-safe collection
        private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();

        public Cache(IAppCache cache, IHttpContextAccessor httpContextAccessor, ILogger<Cache> logger)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }


        public void InvalidateCache<T>()
        {
            var className = typeof(T).Name;
            var keysToRemove = _cacheKeys.Keys
                .Where(k => k.Contains(className))
                .ToList();

            _logger.LogInformation(
                "Invalidating cache for type: {TypeName}. Found {KeyCount} matching keys.",
                className, keysToRemove.Count);

            foreach (var key in keysToRemove)
            {
                _cacheKeys.TryRemove(key, out _);
                _cache.Remove(key);
                _logger.LogInformation("Cache key invalidated: {Key}", key);
            }

            _logger.LogInformation(
                "Invalidation complete. Remaining tracked keys: {Count}",
                _cacheKeys.Count);
        }
        public async Task<T> GetOrAddCacheAsync<T>(Func<Task<T>> addItemFactory, string key, string parentId = null!) where T : class // ✅ solo "class"
        {
            // 1. Determinar el tipo de entidad real (ej: Father, no List<Father>)
            var entityType = GetEntityTypeFromCacheType<T>();
            var className = entityType.Name;
            var isTenanted = typeof(ITennant).IsAssignableFrom(entityType);
            var tenantId = isTenanted ? _tenantId : "(not tenanted)";

            // 2. Construir clave final
            var finalKey = isTenanted
                ? $"{key}-{className}-{_tenantId}"
                : $"{key}-{className}";
            finalKey = string.IsNullOrEmpty(parentId) ? finalKey : $"{finalKey}-{parentId}";
            // 3. Log antes de operar
            _logger.LogInformation(
                "Cache request | CacheType: {CacheType}, EntityType: {EntityType}, KeyBase: {KeyBase}, FinalKey: {FinalKey}, Tenant: {Tenant}",
                typeof(T).Name, className, key, finalKey, tenantId);

            // 4. Registrar clave para invalidación futura
            _cacheKeys.TryAdd(finalKey, 0);
            _logger.LogInformation("Tracked cache keys count: {Count}", _cacheKeys.Count);

            try
            {
                var result = await _cache.GetOrAddAsync(finalKey, addItemFactory);
                _logger.LogInformation("Cache MISS → Loaded and cached for key: {FinalKey}", finalKey);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache error for key: {FinalKey}", finalKey);
                throw;
            }
        }

        // Método auxiliar (sin logs, es interno)
        private Type GetEntityTypeFromCacheType<T>()
        {
            var type = typeof(T);

            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                if (genericDefinition == typeof(List<>) ||
                    genericDefinition == typeof(IList<>) ||
                    genericDefinition == typeof(IEnumerable<>) ||
                    genericDefinition == typeof(IReadOnlyList<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }

            // Si no es una colección, asumimos que T es la entidad misma
            return type;
        }
    }
}
