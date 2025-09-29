using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Lines;
using Simulator.Server.EndPoints.HCs.PlannedSKUs;
using Simulator.Server.EndPoints.HCs.PreferedMixers;
using Simulator.Shared.Models.HCs.LinePlanneds;


namespace Simulator.Server.EndPoints.HCs.LinePlanneds
{
    public static class CreateLinePlanneds
    {
        public static async Task Create(this List<LinePlannedDTO> LinessPlanned, Guid Id, IRepository Repository, List<string> cache)
        {
            foreach (var item in LinessPlanned)
            {
                var lineplanned = LinePlanned.Create(Id);
                item.Map(lineplanned);
                await Repository.AddAsync(lineplanned);
                cache.AddRange(StaticClass.LinePlanneds.Cache.Key(lineplanned.Id, Id));
                await item.PlannedSKUDTOs.Create(lineplanned.Id, Repository, cache);
            }
        }
    }
    public static class CreateUpdateLinePlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.LinePlanneds.EndPoint.CreateUpdate, async (LinePlannedDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    LinePlanned? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = LinePlanned.Create(Data.SimulationPlannedId);
                        await Data.PlannedSKUDTOs.Create(row.Id, Repository, cache);

                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<LinePlanned>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.LinePlanneds.Cache.Key(row.Id, row.SimulationPlannedId));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static LinePlanned Map(this LinePlannedDTO request, LinePlanned row)
        {
            row.ShiftType = request.ShiftType;
            row.WIPLevelValue = request.WIPLevelValue;
            row.WIPLevelUnit = request.WIPLevelUnitName;
            row.LineId = request.LineId;

            return row;
        }

    }
    public static class DeleteLinePlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.LinePlanneds.EndPoint.Delete, async (DeleteLinePlannedRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<LinePlanned>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.LinePlanneds.Cache.Key(row.Id, row.SimulationPlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupLinePlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.LinePlanneds.EndPoint.DeleteGroup, async (DeleteGroupLinePlannedRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<LinePlanned>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.LinePlanneds.Cache.GetAll(Data.SimulationPlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllLinePlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.LinePlanneds.EndPoint.GetAll, async (LinePlannedGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<LinePlanned>, IIncludableQueryable<LinePlanned, object>> includes = x => x
                   .Include(y => y.Line)
                   .Include(x => x.HCSimulationPlanned)
                   .Include(x => x.PreferedMixers).ThenInclude(x => x.Mixer);  
                    Expression<Func<LinePlanned, bool>> Criteria = x => x.SimulationPlannedId == request.SimulationPlannedId;
                    string CacheKey = StaticClass.LinePlanneds.Cache.GetAll(request.SimulationPlannedId);
                    var rows = await Repository.GetAllAsync<LinePlanned>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

                    if (rows == null)
                    {
                        return Result<LinePlannedResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.LinePlanneds.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    LinePlannedResponseList response = new LinePlannedResponseList()
                    {
                        Items = maps
                    };
                    return Result<LinePlannedResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetLinePlannedByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.LinePlanneds.EndPoint.GetById, async (GetLinePlannedByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<LinePlanned>, IIncludableQueryable<LinePlanned, object>> includes = x => x
                   .Include(y => y.Line)
                   .Include(x => x.HCSimulationPlanned);
                    Expression<Func<LinePlanned, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.LinePlanneds.Cache.GetById(request.Id);
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

        public static LinePlannedDTO Map(this LinePlanned row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new LinePlannedDTO()
            {
                MainProcesId = row.HCSimulationPlanned == null ? Guid.Empty : row.HCSimulationPlanned.MainProcessId,
                Id = row.Id,
                LineDTO = row.Line == null ? null! : row.Line.Map(),
                WIPLevelValue = row.WIPLevelValue,
                WIPLevelUnitName = row.WIPLevelUnit,
                ShiftType = row.ShiftType,
                SimulationPlannedId = row.SimulationPlannedId,
                Order = row.Order,
                PreferedMixerDTOs = row.PreferedMixers == null || row.PreferedMixers.Count == 0 ? new() : row.PreferedMixers.Select(x => x.Map()).ToList(),
            };
        }

    }
    //public static class ValidateLinePlannedsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.LinePlanneds.EndPoint.Validate, async (ValidateLinePlannedNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<HCLinePlanned, bool>> CriteriaId = null!;
    //                Func<HCLinePlanned, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.LinePlanneds.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
