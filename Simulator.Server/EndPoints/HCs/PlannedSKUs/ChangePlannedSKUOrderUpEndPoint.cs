using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.PlannedSKUs;

namespace Simulator.Server.EndPoints.HCs.PlannedSKUs
{
    //public static class ChangePlannedSKUOrderUpEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.UpdateUp, async (ChangePlannedSKUOrderUpRequest Data, IRepository Repository) =>
    //            {


    //                var row = await Repository.GetByIdAsync<PlannedSKU>(Data.Id);

    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                if (row.Order == 0)
    //                {
    //                    Expression<Func<PlannedSKU, bool>> Criteria2 = x => x.LinePlannedId == Data.LinePlannedId;
    //                    var rows = await Repository.GetAllAsync(Criteria: Criteria2);
    //                    int order = 0;
    //                    foreach (var item in rows)
    //                    {
    //                        order++;
    //                        item.Order = order;
                           
    //                        await Repository.UpdateAsync(item);

    //                    }
    //                    var result2 = await Repository.Context.SaveChangesAndRemoveCacheAsync(GetCacheKeys(row));
    //                        return Result.EndPointResult(result2,
    //                       Data.Succesfully,
    //                       Data.Fail);
    //                }
    //                if (row.Order == 1) { return Result.Success(Data.Succesfully); }

    //                Expression<Func<PlannedSKU, bool>> Criteria = x => x.LinePlannedId == Data.LinePlannedId && x.Order == row.Order - 1;

    //                var previousRow = await Repository.GetAsync(Criteria: Criteria);

    //                if (previousRow == null) { return Result.Fail(Data.NotFound); }

    //                await Repository.UpdateAsync(previousRow);
    //                await Repository.UpdateAsync(row);

    //                row.Order = row.Order - 1;
    //                previousRow.Order = row.Order + 1;

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(GetCacheKeys(row));

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });
    //        }
    //        private string[] GetCacheKeys(PlannedSKU row)
    //        {
    //            List<string> cacheKeys = [
                     
    //            .. StaticClass.PlannedSKUs.Cache.Key(row.Id, row.LinePlannedId)
    //            ];
    //            return cacheKeys.Where(key => !string.IsNullOrEmpty(key)).ToArray();
    //        }
    //    }



    //}

}
