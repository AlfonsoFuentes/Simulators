using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.Interfaces.Database;
using System.Linq.Expressions;

namespace Simulator.Server.Repositories
{
    //public interface IIgnoreQueryFilterRepository
    //{


    //    Task<bool> AnyAsync<T>(string Cache,
    //          Func<T, bool> CriteriaExist,
    //        Expression<Func<T, bool>> CriteriaId = null!,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!) where T : class, IEntity;
    //    Task<List<T>> GetAllAsync<T>(string Cache, Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //        Expression<Func<T, bool>> Criteria = null!, Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity;

    //    Task<T?> GetAsync<T>(string Cache, Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //        Expression<Func<T, bool>> Criteria = null!, Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity;
    //}
    //public class IgnoreQueryFilterRepository : IIgnoreQueryFilterRepository
    //{
    //    public IAppDbContext Context { get; set; }


    //    public IgnoreQueryFilterRepository(IAppDbContext context)
    //    {
    //        Context = context;
    //    }
    //    public async Task<T?> GetAsync<T>(string Cache,
    //     Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //     Expression<Func<T, bool>> Criteria = null!,
    //    Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity
    //    {
    //        var rows = await Context.GetOrAddCacheAsync(Cache, async () =>
    //        {

    //            var query = Context.Set<T>()
    //            .IgnoreQueryFilters()
    //            .Where(x => !x.IsDeleted)
    //              .AsNoTracking()
    //             .AsSplitQuery()
    //             .AsQueryable();


    //            if (Includes != null)
    //            {
    //                query = Includes(query);
    //            }
    //            if (Criteria != null)
    //            {

    //                query = query.Where(Criteria);
    //            }

    //            if (OrderBy != null)
    //            {
    //                query = query.OrderBy(OrderBy);
    //            }
    //            return await query.FirstOrDefaultAsync();
    //        });
    //        return rows;
    //    }

    //    public async Task<List<T>> GetAllAsync<T>(string Cache,
    //       Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //       Expression<Func<T, bool>> Criteria = null!,
    //      Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity
    //    {

    //        var rows = await Context.GetOrAddCacheAsync(Cache, async () =>
    //        {


    //            var query = Context.Set<T>()
    //            .IgnoreQueryFilters()
    //            .Where(x => !x.IsDeleted)
    //                 .AsNoTracking()
    //                .AsSplitQuery()
    //                .AsQueryable();

    //            if (Includes != null)
    //            {
    //                query = Includes(query);
    //            }
    //            if (Criteria != null)
    //            {
    //                query = query.Where(Criteria);
    //            }

    //            if (OrderBy != null)
    //            {
    //                query = query.OrderBy(OrderBy);
    //            }
    //            try
    //            {
    //                return await query.ToListAsync();
    //            }
    //            catch (Exception ex)
    //            {
    //                string message = ex.Message;
    //            }
    //            return null!;

    //        }
    //        );
    //        return rows;

    //    }



    //    public async Task<bool> AnyAsync<T>(string Cache,
    //          Func<T, bool> CriteriaExist,
    //        Expression<Func<T, bool>> CriteriaId = null!,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!) where T : class, IEntity
    //    {
    //        var query = await Context.GetOrAddCacheAsync(Cache, async () =>
    //        {

    //            var query = Context.Set<T>()
    //            .IgnoreQueryFilters()
    //            .Where(x => !x.IsDeleted)
    //             .AsNoTracking()
    //            .AsSplitQuery()
    //            .AsQueryable();

    //            if (Includes != null)
    //            {
    //                query = Includes(query);
    //            }
    //            if (CriteriaId != null)
    //            {
    //                query = query.Where(CriteriaId);
    //            }
    //            return await query.ToListAsync();

    //        });
    //        bool result = query.Any(CriteriaExist);

    //        return result;
    //    }
    //}
}
