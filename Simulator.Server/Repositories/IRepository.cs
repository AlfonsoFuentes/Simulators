using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.Interfaces.Database;
using System.Linq.Expressions;

namespace Simulator.Server.Repositories
{
    public interface IRepository
    {
        IAppDbContext Context { get; set; }
        Task AddAsync<T>(T entity) where T : class, IAuditableEntity;
        Task<T?> GetByIdAsync<T>(Guid Id) where T : class, IAuditableEntity;
        Task RemoveAsync<T>(T entity) where T : class, IAuditableEntity;
        Task RemoveRangeAsync<T>(List<T> entities) where T : class, IAuditableEntity;
        Task UpdateAsync<T>(T entity) where T : class, IAuditableEntity;
        Task<T?> GetAsync<T>(
           Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
           Expression<Func<T, bool>> Criteria = null!,
           Expression<Func<T, object>> OrderBy = null!) where T : class, IAuditableEntity;
        Task<List<T>> GetAllAsync<T>(Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
            Expression<Func<T, bool>> Criteria = null!,
            Expression<Func<T, object>> OrderBy = null!) where T : class, IAuditableEntity;
        Task<int> GetLastOrderAsync<TEntity, TParent>(Guid parentId)
            where TEntity : class, IAuditableEntity
            where TParent : class, IAuditableEntity<Guid>;

        Task<List<T>> ExecuteQueryAsync<T>(string sql, object parameters = null!) where T : class, IAuditableEntity;
    }
    public class Repository : IRepository
    {
        public IAppDbContext Context { get; set; }

        public Repository(IAppDbContext context)
        {
            Context = context;
        }

        public Task UpdateAsync<T>(T entity) where T : class, IAuditableEntity
        {
            Context.Set<T>().Update(entity);

            return Task.CompletedTask;

        }

        public async Task AddAsync<T>(T entity) where T : class, IAuditableEntity
        {
            await Context.Set<T>().AddAsync(entity);
        }

        public Task RemoveAsync<T>(T entity) where T : class, IAuditableEntity
        {
            Context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        public Task RemoveRangeAsync<T>(List<T> entities) where T : class, IAuditableEntity
        {
            Context.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }
        public async Task<T?> GetByIdAsync<T>(Guid Id) where T : class, IAuditableEntity
        {
            var result = await Context.Set<T>().FindAsync(Id);
            return result;
        }


        public async Task<T?> GetAsync<T>(
            Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
            Expression<Func<T, bool>> Criteria = null!,
           Expression<Func<T, object>> OrderBy = null!) where T : class, IAuditableEntity
        {
            var query = Context.Set<T>()
               .AsQueryable();


            if (Includes != null)
            {
                query = Includes(query);
            }
            if (Criteria != null)
            {
                query = query.Where(Criteria);
            }

            if (OrderBy != null)
            {
                query = query.OrderBy(OrderBy);
            }
            try
            {
                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                string exm = ex.Message;
            }
            return null;
        }
        public async Task<List<T>> GetAllAsync<T>(
            Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
            Expression<Func<T, bool>> Criteria = null!,
           Expression<Func<T, object>> OrderBy = null!) where T : class, IAuditableEntity
        {
            var query = Context.Set<T>()
                .AsQueryable();

            if (Includes != null)
            {
                query = Includes(query);
            }
            if (Criteria != null)
            {
                query = query.Where(Criteria);
            }

            if (OrderBy != null)
            {
                query = query.OrderBy(OrderBy);
            }
            return await query.ToListAsync();
        }

        public async Task<int> GetLastOrderAsync<TEntity, TParent>(Guid parentId)
              where TEntity : class, IAuditableEntity
              where TParent : class, IAuditableEntity<Guid>
        {
            try
            {
                var items = await Context.Set<TParent>()
                     .Include(p => EF.Property<IEnumerable<TEntity>>(p, typeof(TEntity).Name + "s"))
                     .AsNoTracking()
                     .AsSplitQuery()
                     .Where(p => p.Id == parentId)
                     .SelectMany(p => EF.Property<IEnumerable<TEntity>>(p, typeof(TEntity).Name + "s"))
                     .ToListAsync();


                return items == null || items.Count == 0 ? 1 : items.Max(x => x.Order) + 1;
            }
            catch (Exception ex)
            {
                string message = ex.Message;

            }
            return 1;

        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string sql, object parameters = null!) where T : class, IAuditableEntity
        {
            return await Context.Set<T>()
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }
    }
}
