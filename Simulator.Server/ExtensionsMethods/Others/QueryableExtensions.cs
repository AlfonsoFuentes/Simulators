using Microsoft.EntityFrameworkCore;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.Interfaces.Specifications;

namespace Simulator.Server.ExtensionsMethods.Others
{
    public static class QueryableExtensions
    {

        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
            return secondaryResult.Where(spec.Criteria);
        }
    }
}