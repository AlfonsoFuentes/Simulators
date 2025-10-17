using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Server.EndPoints.HCs.SKULines;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Lines;

namespace Simulator.Server.EndPoints.HCs.Lines
{
    public static class CreateUpdateLineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Lines.EndPoint.CreateUpdate, async (LineDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    Line? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = Line.Create(Data.MainProcessId);
                        await Data.InletConnectors.Create(row.Id, Repository, cache);
                        await Data.PlannedDownTimes.Create(row.Id, Repository, cache);
                        await Data.LineSKUs.Create(row.Id, Repository, cache);

                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<Line>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.Lines.Cache.Key(row.Id, row.MainProcessId));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static Line Map(this LineDTO request, Line row)
        {
            row.TimeToReviewAUValue = request.TimeToReviewAUValue;
            row.TimeToReviewAUUnit = request.TimeToReviewAUUnitName;
            row.PackageType = request.PackageType;
            row.ProccesEquipmentType = ProccesEquipmentType.Line;
            row.Name = request.Name;
            row.FocusFactory = request.FocusFactory;


            return row;
        }

    }
    public static class DeleteLineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Lines.EndPoint.Delete, async (DeleteLineRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<Line>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.Lines.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupLineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Lines.EndPoint.DeleteGroup, async (DeleteGroupLineRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<Line>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.Lines.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllLineEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Lines.EndPoint.GetAll, async (LineGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCLine>, IIncludableQueryable<HCLine, object>> includes = x => x
                    //.Include(y => y.!);
                    Expression<Func<Line, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.Lines.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<Line>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<LineResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Lines.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    LineResponseList response = new LineResponseList()
                    {
                        Items = maps
                    };
                    return Result<LineResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetLineByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Lines.EndPoint.GetById, async (GetLineByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCLine>, IIncludableQueryable<HCLine, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<Line, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.Lines.Cache.GetById(request.Id);
                    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria/*, Includes: includes*/);

                    if (row == null)
                    {
                        return Result.Fail(request.NotFound);
                    }

                    var response = row.Map();
                    return Result.Success(response);

                });
            }
        }

        public static LineDTO Map(this Line row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new LineDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
                TimeToReviewAUValue = row.TimeToReviewAUValue,
                TimeToReviewAUUnitName = row.TimeToReviewAUUnit,
                PackageType = row.PackageType,
                EquipmentType = row.ProccesEquipmentType,
                Name = row.Name,
                FocusFactory = row.FocusFactory,
                Order = row.Order,
            };
        }

    }
    public static class ValidateLinesNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Lines.EndPoint.Validate, async (ValidateLineNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Line, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
                    Func<Line, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.Lines.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
