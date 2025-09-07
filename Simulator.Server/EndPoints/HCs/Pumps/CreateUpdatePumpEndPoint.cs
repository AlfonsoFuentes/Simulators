using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Pumps;

namespace Simulator.Server.EndPoints.HCs.Pumps
{

    public static class CreateUpdatePumpEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Pumps.EndPoint.CreateUpdate, async (PumpDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    Pump? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = Pump.Create(Data.MainProcessId);
                        await Data.PlannedDownTimes.Create(row.Id, Repository, cache);
                        await Data.InletConnectors.Create(row.Id, Repository, cache);
                        await Data.OutletConnectors.Create(row.Id, Repository, cache);


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<Pump>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.Pumps.Cache.Key(row.Id, row.MainProcessId));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static Pump Map(this PumpDTO request, Pump row)
        {
            row.FlowValue = request.FlowValue;
            row.FlowUnit = request.FlowUnitName;
            row.IsForWashing = request.IsForWashing;
            row.Name = request.Name;

            row.ProccesEquipmentType = ProccesEquipmentType.Pump;

            return row;
        }

    }
    public static class DeletePumpEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Pumps.EndPoint.Delete, async (DeletePumpRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<Pump>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.Pumps.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupPumpEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Pumps.EndPoint.DeleteGroup, async (DeleteGroupPumpRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<Pump>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.Pumps.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllPumpEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Pumps.EndPoint.GetAll, async (PumpGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCPump>, IIncludableQueryable<HCPump, object>> includes = x => x
                    //.Include(y => y.!);
                    Expression<Func<Pump, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.Pumps.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<Pump>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<PumpResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Pumps.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    PumpResponseList response = new PumpResponseList()
                    {
                        Items = maps
                    };
                    return Result<PumpResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetPumpByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Pumps.EndPoint.GetById, async (GetPumpByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCPump>, IIncludableQueryable<HCPump, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<Pump, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.Pumps.Cache.GetById(request.Id);
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

        public static PumpDTO Map(this Pump row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new PumpDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
                FlowValue = row.FlowValue,
                FlowUnitName = row.FlowUnit,
                IsForWashing = row.IsForWashing,
                Name = row.Name,
                EquipmentType = row.ProccesEquipmentType,
                Order = row.Order,
            };
        }

    }
    public static class ValidatePumpsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Pumps.EndPoint.Validate, async (ValidatePumpNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Pump, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
                    Func<Pump, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.Pumps.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
