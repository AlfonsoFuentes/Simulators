using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.BackBoneSteps;

namespace Simulator.Server.EndPoints.HCs.BackBoneSteps
{
    public static class ChangeBackBoneStepOrderDownEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.UpdateDown, async (ChangeBackBoneStepOrderDowmRequest Data, IRepository Repository) =>
                {
                    var lastorder = await GetLastOrder(Repository,Data.MaterialId);


                    if (lastorder == Data.Order) return Result.Success(Data.Succesfully);


                    Expression<Func<BackBoneStep, bool>> Criteria = x => x.Id == Data.Id;

                    var row = await Repository.GetAsync(Criteria: Criteria);
                    if (row == null) { return Result.Fail(Data.NotFound); }

                    Criteria = x => x.MaterialId == Data.MaterialId && x.Order == row.Order + 1;

                    var nextRow = await Repository.GetAsync(Criteria: Criteria);

                    if (nextRow == null) { return Result.Fail(Data.NotFound); }

                    await Repository.UpdateAsync(nextRow);
                    await Repository.UpdateAsync(row);

                    nextRow.Order = nextRow.Order - 1;
                    row.Order = row.Order + 1;



                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(GetCacheKeys(row));

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });
            }
            private string[] GetCacheKeys(BackBoneStep row)
            {
                List<string> cacheKeys = [

             
                    .. StaticClass.BackBoneSteps.Cache.Key(row.Id,row.MaterialId)
                ];
                return cacheKeys.Where(key => !string.IsNullOrEmpty(key)).ToArray();
            }
            async Task<int> GetLastOrder(IRepository Repository, Guid MaterialId)
            {
                Expression<Func<BackBoneStep, bool>> Criteria = x => x.MaterialId == MaterialId;
                var rows = await Repository.GetAllAsync(Criteria: Criteria);

                var lastorder = rows.Count > 0 ? rows.Max(x => x.Order) : 0;
                return lastorder;
            }
        }
       



    }


}
