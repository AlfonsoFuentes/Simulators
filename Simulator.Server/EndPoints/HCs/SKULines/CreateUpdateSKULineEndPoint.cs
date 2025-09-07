using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Lines;
using Simulator.Server.EndPoints.HCs.SKUs;
using Simulator.Shared.Models.HCs.SKULines;

namespace Simulator.Server.EndPoints.HCs.SKULines
{
    public static class CreateSKULines
    {
        public static async Task Create(this List<SKULineDTO> LineSKUs, Guid Id, IRepository Repository, List<string> cache)
        {
            foreach (var item in LineSKUs)
            {
                var lineskue = SKULine.Create(Id);
                item.Map(lineskue);
                await Repository.AddAsync(lineskue);
                cache.AddRange(StaticClass.SKULines.Cache.Key(lineskue.Id, Id));
            }
        }
    }
    public static class CreateUpdateSKULineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SKULines.EndPoint.CreateUpdate, async (SKULineDTO Data, IRepository Repository) =>
                {

                    SKULine? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = SKULine.Create(Data.LineId);


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<SKULine>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.SKULines.Cache.Key(row.Id, row.LineId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static SKULine Map(this SKULineDTO request, SKULine row)
        {
            row.LineSpeedValue = request.LineSpeedValue;
            row.LineSpeedUnit = request.LineSpeedUnitName;

            row.SKUId = request.SKUId;
            row.Case_Shift = request.Case_Shift;


            return row;
        }

    }
    public static class DeleteSKULineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SKULines.EndPoint.Delete, async (DeleteSKULineRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<SKULine>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.SKULines.Cache.Key(row.Id, row.LineId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupSKULineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SKULines.EndPoint.DeleteGroup, async (DeleteGroupSKULineRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<SKULine>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.SKULines.Cache.GetAll(Data.LineId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllSKULineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SKULines.EndPoint.GetAll, async (SKULineGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<SKULine>, IIncludableQueryable<SKULine, object>> includes = x => x
                   .Include(y => y.SKU).ThenInclude(x => x.Material)
                   ;
                    Expression<Func<SKULine, bool>> Criteria = x => x.LineId == request.LineId;
                    string CacheKey = StaticClass.SKULines.Cache.GetAll(request.LineId);
                    var rows = await Repository.GetAllAsync<SKULine>(Cache: CacheKey, Includes: includes, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<SKULineResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.SKULines.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    SKULineResponseList response = new SKULineResponseList()
                    {
                        Items = maps
                    };
                    return Result<SKULineResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetSKULineByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SKULines.EndPoint.GetById, async (GetSKULineByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<SKULine>, IIncludableQueryable<SKULine, object>> includes = x => x
                  .Include(y => y.SKU).ThenInclude(x => x.Material)
                  ;
                    Expression<Func<SKULine, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.SKULines.Cache.GetById(request.Id);
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

        public static SKULineDTO Map(this SKULine row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new SKULineDTO()
            {
                Id = row.Id,
                LineSpeedUnitName = row.LineSpeedUnit,
                LineSpeedValue = row.LineSpeedValue,
                SKU = row.SKU == null ? null! : row.SKU.Map(),
                LineId = row.LineId,
                Case_Shift = row.Case_Shift,
                Order = row.Order,
            };
        }

    }
    public static class ValidateSKULinesNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SKULines.EndPoint.Validate, async (ValidateSKULineNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<SKULine, bool>> CriteriaId = null!;
                    Func<SKULine, bool> CriteriaExist = x => Data.Id == null ?
                    x.LineId == Data.LineId && x.SKUId == Data.SKUId : x.Id != Data.Id.Value && x.LineId == Data.LineId && x.SKUId == Data.SKUId;
                    string CacheKey = StaticClass.SKULines.Cache.GetAll(Data.LineId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
