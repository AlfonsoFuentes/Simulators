using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.LinePlanneds;
using Simulator.Server.EndPoints.HCs.MixerPlanneds;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds
{
    public static class CreateUpdateSimulationPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.CreateUpdate, async (SimulationPlannedDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    SimulationPlanned? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = SimulationPlanned.Create(Data.MainProcessId);
                        await Data.PlannedMixers.Create(row.Id, Repository, cache);
                        await Data.PlannedLines.Create(row.Id, Repository, cache);
                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<SimulationPlanned>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.SimulationPlanneds.Cache.Key(row.Id, row.MainProcessId));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static SimulationPlanned Map(this SimulationPlannedDTO request, SimulationPlanned row)
        {

            row.Name = request.Name;
            row.InitDate = request.InitDate;
            row.PlannedHours = request.Hours;
            row.EndDate = request.EndDate;
            row.InitSpam = request.InitSpam;
            row.MaxRestrictionTimeValue = request.MaxRestrictionTimeValue;
            row.MaxRestrictionTimeUnit = request.MaxRestrictionTimeUnit;
            row.OperatorHasNotRestrictionToInitBatch = request.OperatorHasNotRestrictionToInitBatch;

            return row;
        }

    }
    public static class DeleteSimulationPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.Delete, async (DeleteSimulationPlannedRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<SimulationPlanned>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.SimulationPlanneds.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupSimulationPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.DeleteGroup, async (DeleteGroupSimulationPlannedRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<SimulationPlanned>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.SimulationPlanneds.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllSimulationPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.GetAll, async (SimulationPlannedGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCSimulationPlanned>, IIncludableQueryable<HCSimulationPlanned, object>> includes = x => x
                    //.Include(y => y.!);
                    Expression<Func<SimulationPlanned, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.SimulationPlanneds.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<SimulationPlanned>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<SimulationPlannedResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.SimulationPlanneds.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    SimulationPlannedResponseList response = new SimulationPlannedResponseList()
                    {
                        Items = maps
                    };
                    return Result<SimulationPlannedResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetSimulationPlannedByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.GetById, async (GetSimulationPlannedByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCSimulationPlanned>, IIncludableQueryable<HCSimulationPlanned, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<SimulationPlanned, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.SimulationPlanneds.Cache.GetById(request.Id);
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

        public static SimulationPlannedDTO Map(this SimulationPlanned row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new SimulationPlannedDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
                Hours = row.PlannedHours,
                InitDate = row.InitDate,
                InitSpam = row.InitSpam,
                Name = row.Name,
                MaxRestrictionTimeValue = row.MaxRestrictionTimeValue,
                MaxRestrictionTimeUnit = row.MaxRestrictionTimeUnit,
                OperatorHasNotRestrictionToInitBatch = row.OperatorHasNotRestrictionToInitBatch,
                Order = row.Order,
            };
        }

    }
    public static class ValidateSimulationPlannedsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.Validate, async (ValidateSimulationPlannedNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<SimulationPlanned, bool>> CriteriaId = null!;
                    Func<SimulationPlanned, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.SimulationPlanneds.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
