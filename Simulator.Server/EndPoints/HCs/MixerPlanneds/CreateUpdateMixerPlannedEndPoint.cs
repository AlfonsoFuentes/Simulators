using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.BackBoneSteps;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Server.EndPoints.HCs.Mixers;
using Simulator.Server.EndPoints.HCs.ProccesEquipments;
using Simulator.Shared.Models.HCs.MixerPlanneds;

namespace Simulator.Server.EndPoints.HCs.MixerPlanneds
{
    public static class CreateMixerPlanneds
    {
        public static async Task Create(this List<MixerPlannedDTO> MixersPlanned, Guid Id, IRepository Repository, List<string> cache)
        {
            foreach (var item in MixersPlanned)
            {
                var mixerplanned = MixerPlanned.Create(Id);
                item.Map(mixerplanned);
                await Repository.AddAsync(mixerplanned);
                cache.AddRange(StaticClass.MixerPlanneds.Cache.Key(mixerplanned.Id, Id));
            }
        }
    }
    public static class CreateUpdateMixerPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MixerPlanneds.EndPoint.CreateUpdate, async (MixerPlannedDTO Data, IRepository Repository) =>
                {

                    MixerPlanned? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = MixerPlanned.Create(Data.SimulationPlannedId);


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<MixerPlanned>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.MixerPlanneds.Cache.Key(row.Id, row.SimulationPlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static MixerPlanned Map(this MixerPlannedDTO request, MixerPlanned row)
        {
            row.BackBoneStepId = request.BackBoneStep == null ? null : request.BackBoneStep.Id;
            row.BackBoneId = request.BackBone.Id;
            row.MixerLevelUnit = request.MixerLevelUnitName;
            row.MixerLevelValue = request.MixerLevelValue;
            row.ProducingToId = request.ProducingTo == null ? null : request.ProducingTo.Id;
            row.MixerId = request.MixerId;
            row.CurrentMixerState = request.CurrentMixerState;
            row.MixerCapacityValue = request.CapacityValue;
            row.MixerCapacityUnit = request.CapacityUnitName;

            return row;
        }

    }
    public static class DeleteMixerPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MixerPlanneds.EndPoint.Delete, async (DeleteMixerPlannedRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<MixerPlanned>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.MixerPlanneds.Cache.Key(row.Id, row.SimulationPlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupMixerPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MixerPlanneds.EndPoint.DeleteGroup, async (DeleteGroupMixerPlannedRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<MixerPlanned>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.MixerPlanneds.Cache.GetAll(Data.SimulationPlannedId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllMixerPlannedEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MixerPlanneds.EndPoint.GetAll, async (MixerPlannedGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<MixerPlanned>, IIncludableQueryable<MixerPlanned, object>> includes = x => x
                     .Include(x => x.SimulationPlanned)
                     .Include(y => y.Mixer!)
                     .Include(x => x.BackBone!)
                     .Include(x => x.BackBoneStep!)
                     .Include(x => x.ProducingTo!);
                    Expression<Func<MixerPlanned, bool>> Criteria = x => x.SimulationPlannedId == request.SimulationPlannedId;
                    string CacheKey = StaticClass.MixerPlanneds.Cache.GetAll(request.SimulationPlannedId);
                    var rows = await Repository.GetAllAsync<MixerPlanned>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

                    if (rows == null)
                    {
                        return Result<MixerPlannedResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.MixerPlanneds.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    MixerPlannedResponseList response = new MixerPlannedResponseList()
                    {
                        Items = maps
                    };
                    return Result<MixerPlannedResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetMixerPlannedByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MixerPlanneds.EndPoint.GetById, async (GetMixerPlannedByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<MixerPlanned>, IIncludableQueryable<MixerPlanned, object>> includes = x => x
                    .Include(x => x.SimulationPlanned)
                    .Include(y => y.Mixer!)
                    .Include(x => x.BackBone!)
                    .Include(x => x.BackBoneStep!)
                    .Include(x => x.ProducingTo!);
                    Expression<Func<MixerPlanned, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.MixerPlanneds.Cache.GetById(request.Id);
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

        public static MixerPlannedDTO Map(this MixerPlanned row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new MixerPlannedDTO()
            {
                Id = row.Id,
                MixerDTO = row.Mixer == null ? null! : row.Mixer.Map(),
                MixerLevelValue = row.MixerLevelValue,
                MixerLevelUnitName = row.MixerLevelUnit,
                CurrentMixerState = row.CurrentMixerState,
                CapacityValue = row.MixerCapacityValue,
                CapacityUnitName = row.MixerCapacityUnit,
                ProducingTo = row.ProducingTo == null ? null! : row.ProducingTo!.Map(),
                BackBone = row.BackBone == null ? null! : row.BackBone.MapMaterial(),
                BackBoneStep = row.BackBoneStep == null ? null! : row.BackBoneStep.Map(),
                Order = row.Order,
                SimulationPlannedId = row.SimulationPlannedId,
                MainProcesId = row.SimulationPlanned == null ? Guid.Empty : row.SimulationPlanned.MainProcessId,
            };
        }

    }

}
