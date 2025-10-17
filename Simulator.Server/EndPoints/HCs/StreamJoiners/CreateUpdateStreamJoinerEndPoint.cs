using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.StreamJoiners;

namespace Simulator.Server.EndPoints.HCs.StreamJoiners
{

    public static class CreateUpdateStreamJoinerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.StreamJoiners.EndPoint.CreateUpdate, async (StreamJoinerDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    StreamJoiner? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = StreamJoiner.Create(Data.MainProcessId);
                        await Data.PlannedDownTimes.Create(row.Id, Repository, cache);
                        await Data.InletConnectors.Create(row.Id, Repository, cache);
                        await Data.OutletConnectors.Create(row.Id, Repository, cache);


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<StreamJoiner>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.StreamJoiners.Cache.Key(row.Id, row.MainProcessId));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static StreamJoiner Map(this StreamJoinerDTO request, StreamJoiner row)
        {
           
         
            row.Name = request.Name;

            row.ProccesEquipmentType = ProccesEquipmentType.StreamJoiner;
            row.FocusFactory = request.FocusFactory;
            return row;
        }

    }
    public static class DeleteStreamJoinerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.StreamJoiners.EndPoint.Delete, async (DeleteStreamJoinerRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<StreamJoiner>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.StreamJoiners.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupStreamJoinerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.StreamJoiners.EndPoint.DeleteGroup, async (DeleteGroupStreamJoinerRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<StreamJoiner>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.StreamJoiners.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllStreamJoinerEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.StreamJoiners.EndPoint.GetAll, async (StreamJoinerGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCStreamJoiner>, IIncludableQueryable<HCStreamJoiner, object>> includes = x => x
                    //.Include(y => y.!);
                    Expression<Func<StreamJoiner, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.StreamJoiners.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<StreamJoiner>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<StreamJoinerResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.StreamJoiners.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    StreamJoinerResponseList response = new StreamJoinerResponseList()
                    {
                        Items = maps
                    };
                    return Result<StreamJoinerResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetStreamJoinerByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.StreamJoiners.EndPoint.GetById, async (GetStreamJoinerByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCStreamJoiner>, IIncludableQueryable<HCStreamJoiner, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<StreamJoiner, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.StreamJoiners.Cache.GetById(request.Id);
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

        public static StreamJoinerDTO Map(this StreamJoiner row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new StreamJoinerDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
               
           
                Name = row.Name,
                EquipmentType = row.ProccesEquipmentType,
                Order = row.Order,
                FocusFactory = row.FocusFactory,
            };
        }

    }
    public static class ValidateStreamJoinersNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.StreamJoiners.EndPoint.Validate, async (ValidateStreamJoinerNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<StreamJoiner, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
                    Func<StreamJoiner, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.StreamJoiners.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
