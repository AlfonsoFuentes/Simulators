using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.LinePlanneds;
using Simulator.Server.EndPoints.HCs.SKUs;
using Simulator.Shared.Models.HCs.PlannedSKUs;

namespace Simulator.Server.EndPoints.HCs.PlannedSKUs
{
    public static class CreateSKUPlanneds
    {
        public static async Task Create(this List<PlannedSKUDTO> SKUsPlanned, Guid Id, IRepository Repository, List<string> cache)
        {
            foreach (var item in SKUsPlanned)
            {
                var skuplanned = PlannedSKU.Create(Id);
                item.Map(skuplanned);
                await Repository.AddAsync(skuplanned);
                cache.AddRange(StaticClass.PlannedSKUs.Cache.Key(skuplanned.Id, Id));
            }
        }
    }
    public static class CreateUpdatePlannedSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.PlannedSKUs.EndPoint.CreateUpdate, async (PlannedSKUDTO Data, IRepository Repository) =>
                {

                    PlannedSKU? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = PlannedSKU.Create(Data.LinePlannedId);


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<PlannedSKU>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.PlannedSKUs.Cache.Key(row.Id, row.LinePlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static PlannedSKU Map(this PlannedSKUDTO request, PlannedSKU row)
        {
            row.SKUId = request.SKU!.Id;

            row.TimeToChangeSKUValue = request.TimeToChangeSKUValue;
            row.TimeToChangeSKUUnit = request.TimeToChangeSKUUnitName;
            row.PlannedCases = request.PlannedCases;

            row.LineSpeedUnit = request.LineSpeedUnitName;
            row.LineSpeedValue = request.LineSpeedValue;

            return row;
        }

    }
    public static class DeletePlannedSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.PlannedSKUs.EndPoint.Delete, async (DeletePlannedSKURequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<PlannedSKU>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.PlannedSKUs.Cache.Key(row.Id, row.LinePlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupPlannedSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.PlannedSKUs.EndPoint.DeleteGroup, async (DeleteGroupPlannedSKURequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<PlannedSKU>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.PlannedSKUs.Cache.GetAll(Data.LinePlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllPlannedSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.PlannedSKUs.EndPoint.GetAll, async (PlannedSKUGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<PlannedSKU>, IIncludableQueryable<PlannedSKU, object>> includes = x => x
                   .Include(y => y.SKU)
                   .Include(x => x.LinePlanned); ;
                    Expression<Func<PlannedSKU, bool>> Criteria = x => x.LinePlannedId == request.LinePlannedId;
                    string CacheKey = StaticClass.PlannedSKUs.Cache.GetAll(request.LinePlannedId);
                    var rows = await Repository.GetAllAsync<PlannedSKU>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

                    if (rows == null)
                    {
                        return Result<PlannedSKUResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.PlannedSKUs.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();


                    PlannedSKUResponseList response = new PlannedSKUResponseList()
                    {
                        Items = maps
                    };
                    return Result<PlannedSKUResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetPlannedSKUByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.PlannedSKUs.EndPoint.GetById, async (GetPlannedSKUByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<PlannedSKU>, IIncludableQueryable<PlannedSKU, object>> includes = x => x
                  .Include(y => y.SKU).ThenInclude(x => x.SKULines)
                  .Include(x => x.LinePlanned);
                    Expression<Func<PlannedSKU, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.PlannedSKUs.Cache.GetById(request.Id);
                    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria, Includes: includes);

                    if (row == null)
                    {
                        return Result.Fail(request.NotFound);
                    }

                    var response = row.Map();
                    return Result.Success(response);

                });
            }
        }

        public static PlannedSKUDTO Map(this PlannedSKU row)
        {
            var skulines = row.SKU == null ? null! : row.SKU.SKULines;

            PlannedSKUDTO result = new PlannedSKUDTO()
            {
                Id = row.Id,
                PlannedCases = row.PlannedCases,
                //Case_Shift = row.SKU.SKULines.Case_Shift,
                TimeToChangeSKUUnitName = row.TimeToChangeSKUUnit,
                TimeToChangeSKUValue = row.TimeToChangeSKUValue,
                LinePlannedId = row.LinePlannedId,
                SKU = row.SKU == null ? null! : row.SKU.Map(),
                LineId = row.LinePlanned == null ? Guid.Empty : row.LinePlanned.LineId,
                Order = row.Order,
                LineSpeedUnitName = row.LineSpeedUnit,
                LineSpeedValue = row.LineSpeedValue,
            };
            if (skulines != null && skulines.Any())
            {
                var case_shift = skulines.FirstOrDefault(x => x.LineId == result.LineId);
                result.Case_Shift = case_shift == null ? 0 : case_shift.Case_Shift;
            }
            return result;
        }

    }
    //public static class ValidatePlannedSKUsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.Validate, async (ValidatePlannedSKUNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<HCPlannedSKU, bool>> CriteriaId = null!;
    //                Func<HCPlannedSKU, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.PlannedSKUs.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
