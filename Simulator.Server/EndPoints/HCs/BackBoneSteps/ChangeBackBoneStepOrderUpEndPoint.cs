using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.BackBoneSteps;

namespace Simulator.Server.EndPoints.HCs.BackBoneSteps
{
    public static class ChangeBackBoneStepOrderUpEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.UpdateUp, async (ChangeBackBoneStepOrderUpRequest Data, IRepository Repository) =>
                {


                    var row = await Repository.GetByIdAsync<BackBoneStep>(Data.Id);

                    if (row == null) { return Result.Fail(Data.NotFound); }
                    if (row.Order == 0)
                    {
                        Expression<Func<BackBoneStep, bool>> Criteria2 = x => x.MaterialId == Data.MaterialId;
                        var rows = await Repository.GetAllAsync(Criteria: Criteria2);
                        int order = 0;
                        foreach (var item in rows)
                        {
                            order++;
                            item.Order = order;
                           
                            await Repository.UpdateAsync(item);

                        }
                        var result2 = await Repository.Context.SaveChangesAndRemoveCacheAsync(GetCacheKeys(row));
                            return Result.EndPointResult(result2,
                           Data.Succesfully,
                           Data.Fail);
                    }
                    if (row.Order == 1) { return Result.Success(Data.Succesfully); }

                    Expression<Func<BackBoneStep, bool>> Criteria = x => x.MaterialId == Data.MaterialId && x.Order == row.Order - 1;

                    var previousRow = await Repository.GetAsync(Criteria: Criteria);

                    if (previousRow == null) { return Result.Fail(Data.NotFound); }

                    await Repository.UpdateAsync(previousRow);
                    await Repository.UpdateAsync(row);

                    row.Order = row.Order - 1;
                    previousRow.Order = row.Order + 1;

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(GetCacheKeys(row));

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });
            }
            private string[] GetCacheKeys(BackBoneStep row)
            {
                List<string> cacheKeys = [
                     
                .. StaticClass.BackBoneSteps.Cache.Key(row.Id, row.MaterialId)
                ];
                return cacheKeys.Where(key => !string.IsNullOrEmpty(key)).ToArray();
            }
        }



    }

}
