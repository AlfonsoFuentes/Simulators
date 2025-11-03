
using Simulator.Server.Databases.Contracts;
using Simulator.Server.Interfaces.Database;
using Simulator.Shared.Intefaces;

namespace Simulator.Server.Repositories
{
    public interface IServerCrudService
    {
        Task<int> DeleteAsync<T>(IDto dto) where T : class, IEntity, ISoftDeletable, IQueryHandler<T>;

        Task<List<T>> GetAllAsync<T>(IDto dto, string querysuffix = null!, string parentId = null!) where T : class, IEntity, IQueryHandler<T>; // 👈 aquí;
        Task<T?> GetById<T>(IDto dto, string querysuffix = null!, string parentId = null!) where T : class, IEntity, IQueryHandler<T>; // 👈 aquí;
        Task<int> SaveAsync<T>(IDto dto) where T : class, IEntity, IMapper, IQueryHandler<T>, ICreator<T>; // 👈 aquí;
   
        Task<int> DeleteGroupAsync<T>(List<IDto> dtos) where T : class, IEntity, ISoftDeletable, IQueryHandler<T>;
        Task<bool> OrderUpAsync<T>(IDto dto) where T : class, IEntity, IQueryHandler<T>;
        Task<bool> OrderDownAsync<T>(IDto dto) where T : class, IEntity, IQueryHandler<T>;
        Task<bool> ValidateAsync<T>(IDto dto, string querysuffix = null!, string parentId = null!) where T : class, IEntity, IValidationRule<T>, IQueryHandler<T>;
    }
    public class ServerCrudService : IServerCrudService
    {
        private readonly IAppDbContext _context;
        private readonly ICache _cache;
        private readonly ILogger<ServerCrudService> _logger;

        public ServerCrudService(IAppDbContext context, ILogger<ServerCrudService> logger, ICache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        // 👇 Consulta para lectura (sin tracking)
        private IQueryable<T> GetOrderedQuery<T>(IDto dto)
            where T : class, IEntity, IQueryHandler<T>
        {
            var query = _context.Set<T>().AsNoTracking().AsSplitQuery().AsQueryable();
            var criteria = T.GetFilterBy(dto);
            var result= criteria != null ? query.Where(criteria) : query;

            return result;
        }

        // 👇 Consulta para escritura (con tracking)
        private IQueryable<T> GetOrderedTrackedQuery<T>(IDto dto)
            where T : class, IEntity, IQueryHandler<T>
        {
            var query = _context.Set<T>();
            var criteria = T.GetFilterBy(dto);
            return criteria != null ? query.Where(criteria) : query;
        }

        public async Task<List<T>> GetAllAsync<T>(IDto dto, string querysuffix = null!, string parentId = null!) where T : class, IEntity, IQueryHandler<T>
        {
            var entityType = typeof(T).Name;
            var suffix = string.IsNullOrEmpty(querysuffix) ? string.Empty : $"-{querysuffix}";
            var cacheKeyBase = $"getall{suffix}";

            _logger.LogInformation("GetAllAsync started for {EntityType} with parentId: {ParentId}, suffix: {Suffix}",
                entityType, parentId, suffix);

            try
            {
                var rows = await _cache.GetOrAddCacheAsync(async () =>
                {
                    var query = GetOrderedQuery<T>(dto); // 👈 usa el método

                    var includes = T.GetIncludesBy(dto);

                    var orderBy = T.GetOrderBy(dto);
                    if (includes != null)
                        query = includes(query);

                    if (orderBy != null)
                        query = query.OrderBy(orderBy);
                    var result = await query.ToListAsync();
                    _logger.LogInformation("Loaded {Count} records for {EntityType} from database", result.Count, entityType);
                    return result;
                }, cacheKeyBase, parentId);

                _logger.LogInformation("GetAllAsync completed for {EntityType}. Returned {Count} records",
                    entityType, rows?.Count ?? 0);
                return rows ?? new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for {EntityType} with parentId: {ParentId}",
                    entityType, parentId);
                throw;
            }
        }

        public async Task<T?> GetById<T>(IDto dto, string querysuffix = null!, string parentId = null!)
            where T : class, IEntity, IQueryHandler<T>
        {
            if (dto.Id == Guid.Empty)
            {
                _logger.LogWarning("GetByIdAsync called with empty ID for {EntityType}", typeof(T).Name);
                return null;
            }

            var entityType = typeof(T).Name;
            var suffix = string.IsNullOrEmpty(querysuffix) ? string.Empty : $"-{querysuffix}";
            var cacheKeyBase = $"getbyid-{dto.Id}{suffix}";

            _logger.LogInformation("GetByIdAsync started for {EntityType} with ID {Id}, parentId: {ParentId}, suffix: {Suffix}",
                entityType, dto.Id, parentId, suffix);

            try
            {
                var entity = await _cache.GetOrAddCacheAsync(async () =>
                {
                    var query = GetOrderedQuery<T>(dto); // 👈 usa el método

                    var includes = T.GetIncludesBy(dto);
                    if (includes != null)
                        query = includes(query);

                    var result = await query.FirstOrDefaultAsync(x => x.Id == dto.Id);

                    if (result == null)
                    {
                        _logger.LogWarning("Entity {EntityType} with ID {Id} not found in database", entityType, dto.Id);
                        throw new KeyNotFoundException();
                    }

                    _logger.LogInformation("Loaded {EntityType} with ID {Id} from database", entityType, dto.Id);
                    return result;
                }, cacheKeyBase, parentId);

                _logger.LogInformation("GetByIdAsync completed for {EntityType} with ID {Id}", entityType, dto.Id);
                return entity;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync for {EntityType} with ID {Id}, parentId: {ParentId}",
                    entityType, dto.Id, parentId);
                throw;
            }
        }

        public async Task<int> SaveAsync<T>(IDto dto) where T : class, IEntity, IMapper, IQueryHandler<T> , ICreator<T>
        {
            T? entity = null;
            int result = 0;

            if (dto.IsCreated)
            {
                // Update
                var query = _context.Set<T>().Where(x => x.Id == dto.Id);
                var includes = T.GetIncludesBy(dto);
                if (includes != null) query = includes(query);
                entity = await query.FirstOrDefaultAsync();
                if (entity == null) return 0;
            }
            else
            {
                // Add
                entity = T.Create(dto);
                entity.CreatedOn = DateTime.Now;

                
                var maxOrder = await GetOrderedQuery<T>(dto).MaxAsync(x => (int?)x.Order) ?? 0;
                entity.Order = maxOrder + 1;

                await _context.Set<T>().AddAsync(entity);
            }

            if (entity != null)
            {
                entity.MapFromDto(dto);
                result = await _context.SaveChangesAsync();
                if (result > 0) _cache.InvalidateCache<T>();
            }

            return result;
        }

        public async Task<int> DeleteAsync<T>(IDto dto) where T : class, IEntity, ISoftDeletable, IQueryHandler<T>
        {
            if (dto.Id == Guid.Empty) return 0;

            var entity = await _context.Set<T>().FindAsync(dto.Id);
            if (entity == null) return 0;

            entity.IsDeleted = true;
            entity.DeletedOnUtc = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await ReorderAsync<T>(dto);
                _cache.InvalidateCache<T>();
            }
            return result;
        }

        public async Task<int> DeleteGroupAsync<T>(List<IDto> dtos) where T : class, IEntity, ISoftDeletable, IQueryHandler<T>
        {
            if (dtos == null || dtos.Count == 0) return 0;

            var ids = dtos.Select(d => d.Id).ToList();
            var entities = await _context.Set<T>().Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.DeletedOnUtc = DateTime.UtcNow;
            }

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await ReorderAsync<T>(dtos[0]);
                _cache.InvalidateCache<T>();
            }
            return result;
        }

        private async Task ReorderAsync<T>(IDto dto) where T : class, IEntity, IQueryHandler<T>
        {
            // 👇 Usa GetOrderedTrackedQuery (con tracking)
            var query = GetOrderedTrackedQuery<T>(dto);
            var entities = await query.OrderBy(x => x.Order).ToListAsync();

            for (int i = 0; i < entities.Count; i++)
                entities[i].Order = i + 1;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> OrderUpAsync<T>(IDto dto) where T : class, IEntity, IQueryHandler<T>
        {
            if (dto.Id == Guid.Empty) return false;

            var entity = await _context.Set<T>().FindAsync(dto.Id);
            if (entity == null || entity.Order <= 1)
                return false; // 👈 ya está en la primera posición

            var neighbor = await GetOrderedTrackedQuery<T>(dto)
                .Where(x => x.Order == entity.Order - 1)
                .FirstOrDefaultAsync();

            if (neighbor == null) return false;

            (entity.Order, neighbor.Order) = (neighbor.Order, entity.Order);
            var result = await _context.SaveChangesAsync();
            if (result > 0) _cache.InvalidateCache<T>();
            return result > 0;
        }

        public async Task<bool> OrderDownAsync<T>(IDto dto) where T : class, IEntity, IQueryHandler<T>
        {
            if (dto.Id == Guid.Empty) return false;

            var entity = await _context.Set<T>().FindAsync(dto.Id);
            if (entity == null) return false;

            // 👇 Verificar que no esté en la última posición
            var maxOrder = await GetOrderedQuery<T>(dto).MaxAsync(x => x.Order);
            if (entity.Order >= maxOrder)
                return false; // 👈 ya está en la última posición

            var neighbor = await GetOrderedTrackedQuery<T>(dto)
                .Where(x => x.Order == entity.Order + 1)
                .FirstOrDefaultAsync();

            if (neighbor == null) return false;

            (entity.Order, neighbor.Order) = (neighbor.Order, entity.Order);
            var result = await _context.SaveChangesAsync();
            if (result > 0) _cache.InvalidateCache<T>();
            return result > 0;
        }
        public async Task<bool> ValidateAsync<T>(IDto dto, string querysuffix = null!, string parentId = null!) 
            where T : class, IEntity, Databases.Contracts.IValidationRule<T>, IQueryHandler<T>
        {
            if (dto is not IValidationRequest request)
                return true;

            try
            {
                var rows = await GetAllAsync<T>(dto, querysuffix, parentId);
                var existCriteria = T.GetExistCriteria(dto, request.ValidationKey);

                if (existCriteria != null)
                {
                    var exists = rows.AsQueryable().Where(existCriteria).Any();
                    return !exists;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation error for {EntityType} key {Key}", typeof(T).Name, request.ValidationKey);
                return false;
            }
        }
    }
}
