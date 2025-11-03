using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.PlannedSKUs;

namespace Simulator.Server.EndPoints.HCs.PlannedSKUs
{
    //public static class ChangePlannedSKUOrderDownEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.UpdateDown, async (ChangePlannedSKUOrderDowmRequest Data, IRepository Repository) =>
    //            {
    //                var lastorder = await GetLastOrder(Repository,Data.LinePlannedId);


    //                if (lastorder == Data.Order) return Result.Success(Data.Succesfully);


    //                Expression<Func<PlannedSKU, bool>> Criteria = x => x.Id == Data.Id;

    //                var row = await Repository.GetAsync(Criteria: Criteria);
    //                if (row == null) { return Result.Fail(Data.NotFound); }

    //                Criteria = x => x.LinePlannedId == Data.LinePlannedId && x.Order == row.Order + 1;

    //                var nextRow = await Repository.GetAsync(Criteria: Criteria);

    //                if (nextRow == null) { return Result.Fail(Data.NotFound); }

    //                await Repository.UpdateAsync(nextRow);
    //                await Repository.UpdateAsync(row);

    //                nextRow.Order = nextRow.Order - 1;
    //                row.Order = row.Order + 1;



    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(GetCacheKeys(row));

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });
    //        }
    //        private string[] GetCacheKeys(PlannedSKU row)
    //        {
    //            List<string> cacheKeys = [

             
    //                .. StaticClass.PlannedSKUs.Cache.Key(row.Id,row.LinePlannedId)
    //            ];
    //            return cacheKeys.Where(key => !string.IsNullOrEmpty(key)).ToArray();
    //        }
    //        async Task<int> GetLastOrder(IRepository Repository, Guid MaterialId)
    //        {
    //            Expression<Func<PlannedSKU, bool>> Criteria = x => x.LinePlannedId == MaterialId;
    //            var rows = await Repository.GetAllAsync(Criteria: Criteria);

    //            var lastorder = rows.Count > 0 ? rows.Max(x => x.Order) : 0;
    //            return lastorder;
    //        }
    //    }
       



    //}


}
