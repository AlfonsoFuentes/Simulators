using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.Interfaces.Database;
using System.Linq.Expressions;

namespace Simulator.Server.Repositories
{
    //public interface IQueryRepository
    //{
    //    IAppDbContext Context { get; set; }

    //    Task<bool> AnyAsync<T>(string Cache,
    //          Func<T, bool> CriteriaExist,
    //        Expression<Func<T, bool>> CriteriaId = null!,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!) where T : class, IEntity;
    //    Task<List<T>> GetAllAsync<T>(string Cache, Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //        Expression<Func<T, bool>> Criteria = null!, Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity;
    //    Task<List<T>> GetAllToValidateAsync<T>(string Cache, Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!, Func<T, bool> Criteria = null!,
    //        Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity;
    //    Task<T?> GetAsync<T>(string Cache, Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //        Expression<Func<T, bool>> Criteria = null!, Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity;
    //}
    //public class QueryRepository : IQueryRepository
    //{
    //    public IAppDbContext Context { get; set; }

     
    //    public QueryRepository(IAppDbContext context)
    //    {
    //        Context = context;
         
    //    }
    //    public async Task<T?> GetAsync<T>(string Cache,
    //     Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //     Expression<Func<T, bool>> Criteria = null!,
    //    Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity
    //    {
    //        try
    //        {
    //            var cache = $"{Cache}";
    //            var rows = await Context.GetOrAddCacheAsync(cache, async () =>
    //            {

    //                var query = Context.Set<T>()

    //                  .AsNoTracking()
    //                 .AsSplitQuery()
    //                 .AsQueryable();


    //                if (Includes != null)
    //                {
    //                    query = Includes(query);
    //                }
    //                if (Criteria != null)
    //                {

    //                    query = query.Where(Criteria);
    //                }

    //                if (OrderBy != null)
    //                {
    //                    query = query.OrderBy(OrderBy);
    //                }
    //                return await query.FirstOrDefaultAsync();
    //            });
    //            return rows;
    //        }
    //        catch (Exception ex)
    //        {
    //            string e = ex.Message;
    //        }
    //        return null!;

    //    }

    //    public async Task<List<T>> GetAllAsync<T>(string Cache,
    //       Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //       Expression<Func<T, bool>> Criteria = null!,
    //      Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity
    //    {

    //        var cache = $"{Cache}";
    //        var rows = await Context.GetOrAddCacheAsync(cache, async () =>
    //        {


    //            var query = Context.Set<T>()

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

    //    public async Task<List<T>> GetAllToValidateAsync<T>(string Cache,
    //     Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!,
    //     Func<T, bool> Criteria = null!,
    //    Expression<Func<T, object>> OrderBy = null!) where T : class, IEntity
    //    {

    //        var cache = $"{Cache}";
    //        List<T> rows = await Context.GetOrAddCacheAsync(cache, async () =>
    //        {
    //            var query = Context.Set<T>()

    //                 .AsNoTracking()
    //                .AsSplitQuery()
    //                .AsQueryable();

    //            if (Includes != null)
    //            {
    //                query = Includes(query);
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
    //        });
    //        if (Criteria != null)
    //        {
    //            rows = rows.Where(Criteria).ToList();
    //        }


    //        return rows;

    //    }

    //    public async Task<bool> AnyAsync<T>(string Cache,
    //          Func<T, bool> CriteriaExist,
    //        Expression<Func<T, bool>> CriteriaId = null!,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>> Includes = null!) where T : class, IEntity
    //    {
    //        var cache = $"{Cache}";


    //        var query = await Context.GetOrAddCacheAsync(cache, async () =>
    //        {

    //            var query = Context.Set<T>()

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
