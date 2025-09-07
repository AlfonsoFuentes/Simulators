using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;

namespace Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes
{
    public static class CreatePlannedDownTimes
    {
        public static async Task Create(this List<EquipmentPlannedDownTimeDTO> PlannedDownTimes, Guid Id, IRepository Repository, List<string> cache)
        {
            foreach (var item in PlannedDownTimes)
            {
                var rowplanned = EquipmentPlannedDownTime.Create(Id);

                item.Map(rowplanned);
                await Repository.AddAsync(rowplanned);
                cache.AddRange(StaticClass.EquipmentPlannedDownTimes.Cache.Key(rowplanned.Id, Id));
            }
        }
    }
    public static class CreateUpdateEquipmentPlannedDownTimeEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.CreateUpdate, async (EquipmentPlannedDownTimeDTO Data, IRepository Repository) =>
                {

                    EquipmentPlannedDownTime? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = EquipmentPlannedDownTime.Create(Data.BaseEquipmentId);


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<EquipmentPlannedDownTime>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.EquipmentPlannedDownTimes.Cache.Key(row.Id,row.BaseEquipmentId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static EquipmentPlannedDownTime Map(this EquipmentPlannedDownTimeDTO request, EquipmentPlannedDownTime row)
        {
            row.StartTime = request.StartTime;
            row.EndTime = request.EndTime;
            row.Name = request.Name;



            return row;
        }

    }
    public static class DeleteEquipmentPlannedDownTimeEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.Delete, async (DeleteEquipmentPlannedDownTimeRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<EquipmentPlannedDownTime>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.EquipmentPlannedDownTimes.Cache.Key(row.Id, row.BaseEquipmentId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupEquipmentPlannedDownTimeEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.DeleteGroup, async (DeleteGroupEquipmentPlannedDownTimeRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<EquipmentPlannedDownTime>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.EquipmentPlannedDownTimes.Cache.GetAll(Data.EquipmentId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllEquipmentPlannedDownTimeEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.GetAll, async (EquipmentPlannedDownTimeGetAll request, IQueryRepository Repository) =>
                {

                    
                    Expression<Func<EquipmentPlannedDownTime, bool>> Criteria = x => x.BaseEquipmentId == request.EquipmentId;
                    string CacheKey = StaticClass.EquipmentPlannedDownTimes.Cache.GetAll(request.EquipmentId);
                    var rows = await Repository.GetAllAsync<EquipmentPlannedDownTime>(Cache: CacheKey,Criteria:Criteria);

                    if (rows == null)
                    {
                        return Result<EquipmentPlannedDownTimeResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.EquipmentPlannedDownTimes.ClassLegend));
                    }

                    var maps = rows.OrderBy(x=>x.StartTime).Select(x => x.Map()).ToList();


                    EquipmentPlannedDownTimeResponseList response = new EquipmentPlannedDownTimeResponseList()
                    {
                        Items = maps
                    };
                    return Result<EquipmentPlannedDownTimeResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetEquipmentPlannedDownTimeByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.GetById, async (GetEquipmentPlannedDownTimeByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCEquipmentPlannedDownTime>, IIncludableQueryable<HCEquipmentPlannedDownTime, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<EquipmentPlannedDownTime, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.EquipmentPlannedDownTimes.Cache.GetById(request.Id);
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

        public static EquipmentPlannedDownTimeDTO Map(this EquipmentPlannedDownTime row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new EquipmentPlannedDownTimeDTO()
            {
                Id = row.Id,
                BaseEquipmentId = row.BaseEquipmentId,
                StartTime = row.StartTime,
                EndTime = row.EndTime,
                Name = row.Name,

                Order = row.Order,
            };
        }

    }
    //public static class ValidateEquipmentPlannedDownTimesNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.Validate, async (ValidateEquipmentPlannedDownTimeNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<EquipmentPlannedDownTime, bool>> CriteriaId = null!;
    //                Func<EquipmentPlannedDownTime, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.EquipmentPlannedDownTimes.Cache.GetAll;

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
