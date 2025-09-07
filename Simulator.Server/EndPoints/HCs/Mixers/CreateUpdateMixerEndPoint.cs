﻿using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Mixers;

namespace Simulator.Server.EndPoints.HCs.Mixers
{
    public static class CreateUpdateMixerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Mixers.EndPoint.CreateUpdate, async (MixerDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new List<string>();
                    Mixer? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = Mixer.Create(Data.MainProcessId);
                        await Data.InletConnectors.Create(row.Id, Repository, cache);
                        await Data.OutletConnectors.Create(row.Id, Repository, cache);
                        await Data.MaterialEquipments.Create(row.Id, Repository, cache);
                        await Data.PlannedDownTimes.Create(row.Id, Repository, cache);



                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<Mixer>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.Mixers.Cache.Key(row.Id, row.MainProcessId));


                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static Mixer Map(this MixerDTO request, Mixer row)
        {


            row.Name = request.Name;

            row.ProccesEquipmentType = ProccesEquipmentType.Mixer;

            return row;
        }

    }
    public static class DeleteMixerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Mixers.EndPoint.Delete, async (DeleteMixerRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<Mixer>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.Mixers.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupMixerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Mixers.EndPoint.DeleteGroup, async (DeleteGroupMixerRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<Mixer>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.Mixers.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllMixerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Mixers.EndPoint.GetAll, async (MixerGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCMixer>, IIncludableQueryable<HCMixer, object>> includes = x => x
                    //.Include(y => y.!);
                    Expression<Func<Mixer, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.Mixers.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<Mixer>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<MixerResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Mixers.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    MixerResponseList response = new MixerResponseList()
                    {
                        Items = maps
                    };
                    return Result<MixerResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetMixerByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Mixers.EndPoint.GetById, async (GetMixerByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCMixer>, IIncludableQueryable<HCMixer, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<Mixer, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.Mixers.Cache.GetById(request.Id);
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

        public static MixerDTO Map(this Mixer row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new MixerDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
                EquipmentType = row.ProccesEquipmentType,
                Name = row.Name,

                Order = row.Order,
            };
        }

    }
    public static class ValidateMixersNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Mixers.EndPoint.Validate, async (ValidateMixerNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Mixer, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
                    Func<Mixer, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.Mixers.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
