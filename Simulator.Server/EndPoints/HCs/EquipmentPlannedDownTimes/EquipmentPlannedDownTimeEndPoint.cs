using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;

namespace Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes
{
    public class EquipmentPlannedDownTimeEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<EquipmentPlannedDownTime>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(EquipmentPlannedDownTime)} table");
                }


                return Result.Success($"{nameof(EquipmentPlannedDownTime)} table Updated");

            });

            app.MapPost("api/getall/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<EquipmentPlannedDownTime>(dto, parentId: $"{dto.BaseEquipmentId}");

                var result = query.Select(x => x.MapToDto<EquipmentPlannedDownTimeDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<EquipmentPlannedDownTime>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<EquipmentPlannedDownTimeDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<EquipmentPlannedDownTime>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(EquipmentPlannedDownTime)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(EquipmentPlannedDownTime)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/EquipmentPlannedDownTimeDTO", async (List<EquipmentPlannedDownTimeDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(EquipmentPlannedDownTime)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<EquipmentPlannedDownTime>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(EquipmentPlannedDownTime)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<EquipmentPlannedDownTime>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("EquipmentPlannedDownTime moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<EquipmentPlannedDownTime>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("EquipmentPlannedDownTime moved down successfully");
            });
            // En EquipmentPlannedDownTimeEndpointExtensions
            app.MapPost("api/validate/EquipmentPlannedDownTimeDTO", async (EquipmentPlannedDownTimeDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<EquipmentPlannedDownTime>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(EquipmentPlannedDownTime)} table, already exists");
            });
        }
    }
    //public static class CreatePlannedDownTimes
    //{
    //    public static async Task Create(this List<EquipmentPlannedDownTimeDTO> PlannedDownTimes, Guid Id, IRepository Repository, List<string> cache)
    //    {
    //        foreach (var item in PlannedDownTimes)
    //        {
    //            var rowplanned = EquipmentPlannedDownTime.Create(Id);

    //            item.Map(rowplanned);
    //            await Repository.AddAsync(rowplanned);
    //            cache.AddRange(StaticClass.EquipmentPlannedDownTimes.Cache.Key(rowplanned.Id, Id));
    //        }
    //    }
    //}
    //public static class CreateUpdateEquipmentPlannedDownTimeEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.CreateUpdate, async (EquipmentPlannedDownTimeDTO Data, IRepository Repository) =>
    //            {

    //                EquipmentPlannedDownTime? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = EquipmentPlannedDownTime.Create(Data.BaseEquipmentId);


    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<EquipmentPlannedDownTime>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                List<string> cache = [.. StaticClass.EquipmentPlannedDownTimes.Cache.Key(row.Id,row.BaseEquipmentId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static EquipmentPlannedDownTime Map(this EquipmentPlannedDownTimeDTO request, EquipmentPlannedDownTime row)
    //    {
    //        row.StartTime = request.StartTime;
    //        row.EndTime = request.EndTime;
    //        row.Name = request.Name;



    //        return row;
    //    }

    //}
    //public static class DeleteEquipmentPlannedDownTimeEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.Delete, async (DeleteEquipmentPlannedDownTimeRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<EquipmentPlannedDownTime>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.EquipmentPlannedDownTimes.Cache.Key(row.Id, row.BaseEquipmentId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupEquipmentPlannedDownTimeEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.DeleteGroup, async (DeleteGroupEquipmentPlannedDownTimeRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<EquipmentPlannedDownTime>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.EquipmentPlannedDownTimes.Cache.GetAll(Data.EquipmentId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllEquipmentPlannedDownTimeEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.GetAll, async (EquipmentPlannedDownTimeGetAll request, IQueryRepository Repository) =>
    //            {

                    
    //                Expression<Func<EquipmentPlannedDownTime, bool>> Criteria = x => x.BaseEquipmentId == request.EquipmentId;
    //                string CacheKey = StaticClass.EquipmentPlannedDownTimes.Cache.GetAll(request.EquipmentId);
    //                var rows = await Repository.GetAllAsync<EquipmentPlannedDownTime>(Cache: CacheKey,Criteria:Criteria);

    //                if (rows == null)
    //                {
    //                    return Result<EquipmentPlannedDownTimeResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.EquipmentPlannedDownTimes.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x=>x.StartTime).Select(x => x.Map()).ToList();


    //                EquipmentPlannedDownTimeResponseList response = new EquipmentPlannedDownTimeResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<EquipmentPlannedDownTimeResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetEquipmentPlannedDownTimeByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.EquipmentPlannedDownTimes.EndPoint.GetById, async (GetEquipmentPlannedDownTimeByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                //Func<IQueryable<HCEquipmentPlannedDownTime>, IIncludableQueryable<HCEquipmentPlannedDownTime, object>> includes = x => x
    //                //.Include(y => y.RawMaterial!);
    //                Expression<Func<EquipmentPlannedDownTime, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.EquipmentPlannedDownTimes.Cache.GetById(request.Id);
    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria/*, Includes: includes*/);

    //                if (row == null)
    //                {
    //                    return Result.Fail(request.NotFound);
    //                }

    //                var response = row.Map();
    //                return Result.Success(response);

    //            });
    //        }
    //    }

    //    public static EquipmentPlannedDownTimeDTO Map(this EquipmentPlannedDownTime row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new EquipmentPlannedDownTimeDTO()
    //        {
    //            Id = row.Id,
    //            BaseEquipmentId = row.BaseEquipmentId,
    //            StartTime = row.StartTime,
    //            EndTime = row.EndTime,
    //            Name = row.Name,

    //            Order = row.Order,
    //        };
    //    }

    //}
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
