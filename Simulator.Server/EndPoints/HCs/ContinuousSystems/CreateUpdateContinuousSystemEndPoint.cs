using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.ContinuousSystems;

namespace Simulator.Server.EndPoints.HCs.ContinuousSystems
{
    public static class CreateUpdateContinuousSystemEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.ContinuousSystems.EndPoint.CreateUpdate, async (ContinuousSystemDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    ContinuousSystem? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = ContinuousSystem.Create(Data.MainProcessId);
                        await Data.InletConnectors.Create(row.Id, Repository, cache);
                        await Data.OutletConnectors.Create(row.Id, Repository, cache);
                        await Data.MaterialEquipments.Create(row.Id, Repository, cache);
                        await Data.PlannedDownTimes.Create(row.Id, Repository, cache);

                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<ContinuousSystem>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.ContinuousSystems.Cache.Key(row.Id, row.MainProcessId));


                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static ContinuousSystem Map(this ContinuousSystemDTO request, ContinuousSystem row)
        {
            row.FlowValue = request.FlowValue;
            row.FlowUnit = request.FlowUnitName;
            row.ProccesEquipmentType = ProccesEquipmentType.ContinuousSystem;
            row.Name = request.Name;

            row.FocusFactory = request.FocusFactory;

            return row;
        }

    }
    public static class DeleteContinuousSystemEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.ContinuousSystems.EndPoint.Delete, async (DeleteContinuousSystemRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<ContinuousSystem>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.ContinuousSystems.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupContinuousSystemEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.ContinuousSystems.EndPoint.DeleteGroup, async (DeleteGroupContinuousSystemRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<ContinuousSystem>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.ContinuousSystems.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllContinuousSystemEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.ContinuousSystems.EndPoint.GetAll, async (ContinuousSystemGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCContinuousSystem>, IIncludableQueryable<HCContinuousSystem, object>> includes = x => x
                    //.Include(y => y.!);
                    Expression<Func<ContinuousSystem, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.ContinuousSystems.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<ContinuousSystem>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<ContinuousSystemResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.ContinuousSystems.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    ContinuousSystemResponseList response = new ContinuousSystemResponseList()
                    {
                        Items = maps
                    };
                    return Result<ContinuousSystemResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetContinuousSystemByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.ContinuousSystems.EndPoint.GetById, async (GetContinuousSystemByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCContinuousSystem>, IIncludableQueryable<HCContinuousSystem, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<ContinuousSystem, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.ContinuousSystems.Cache.GetById(request.Id);
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

        public static ContinuousSystemDTO Map(this ContinuousSystem row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new ContinuousSystemDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
                FlowValue = row.FlowValue,
                FlowUnitName = row.FlowUnit,
                EquipmentType = row.ProccesEquipmentType,
                Name = row.Name,
                FocusFactory = row.FocusFactory,
                Order = row.Order,
            };
        }

    }
    public static class ValidateContinuousSystemsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.ContinuousSystems.EndPoint.Validate, async (ValidateContinuousSystemNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<ContinuousSystem, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
                    Func<ContinuousSystem, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.ContinuousSystems.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
