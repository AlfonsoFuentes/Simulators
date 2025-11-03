using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.StreamJoiners;

namespace Simulator.Server.EndPoints.HCs.StreamJoiners
{
    public class StreamJoinerEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<StreamJoiner>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(StreamJoiner)} table");
                }


                return Result.Success($"{nameof(StreamJoiner)} table Updated");

            });

            app.MapPost("api/getall/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<StreamJoiner>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => x.MapToDto<StreamJoinerDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<StreamJoiner>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<StreamJoinerDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<StreamJoiner>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(StreamJoiner)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(StreamJoiner)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/StreamJoinerDTO", async (List<StreamJoinerDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(StreamJoiner)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<StreamJoiner>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(StreamJoiner)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<StreamJoiner>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("StreamJoiner moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<StreamJoiner>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("StreamJoiner moved down successfully");
            });
            // En StreamJoinerEndpointExtensions
            app.MapPost("api/validate/StreamJoinerDTO", async (StreamJoinerDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<StreamJoiner>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(StreamJoiner)} table, already exists");
            });
        }
    }
    //public static class CreateUpdateStreamJoinerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.StreamJoiners.EndPoint.CreateUpdate, async (StreamJoinerDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                StreamJoiner? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = StreamJoiner.Create(Data.MainProcessId);
    //                    await Data.PlannedDownTimes.Create(row.Id, Repository, cache);
    //                    await Data.InletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.OutletConnectors.Create(row.Id, Repository, cache);


    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<StreamJoiner>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.StreamJoiners.Cache.Key(row.Id, row.MainProcessId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static StreamJoiner Map(this StreamJoinerDTO request, StreamJoiner row)
    //    {
           
         
    //        row.Name = request.Name;

    //        row.ProccesEquipmentType = ProccesEquipmentType.StreamJoiner;
    //        row.FocusFactory = request.FocusFactory;
    //        return row;
    //    }

    //}
    //public static class DeleteStreamJoinerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.StreamJoiners.EndPoint.Delete, async (DeleteStreamJoinerRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<StreamJoiner>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.StreamJoiners.Cache.Key(row.Id, row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupStreamJoinerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.StreamJoiners.EndPoint.DeleteGroup, async (DeleteGroupStreamJoinerRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<StreamJoiner>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.StreamJoiners.Cache.GetAll(Data.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllStreamJoinerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.StreamJoiners.EndPoint.GetAll, async (StreamJoinerGetAll request, IQueryRepository Repository) =>
    //            {

    //                // Func<IQueryable<HCStreamJoiner>, IIncludableQueryable<HCStreamJoiner, object>> includes = x => x
    //                //.Include(y => y.!);
    //                Expression<Func<StreamJoiner, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //                string CacheKey = StaticClass.StreamJoiners.Cache.GetAll(request.MainProcessId);
    //                var rows = await Repository.GetAllAsync<StreamJoiner>(Cache: CacheKey, Criteria: Criteria);

    //                if (rows == null)
    //                {
    //                    return Result<StreamJoinerResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.StreamJoiners.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //                StreamJoinerResponseList response = new StreamJoinerResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<StreamJoinerResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetStreamJoinerByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.StreamJoiners.EndPoint.GetById, async (GetStreamJoinerByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                //Func<IQueryable<HCStreamJoiner>, IIncludableQueryable<HCStreamJoiner, object>> includes = x => x
    //                //.Include(y => y.RawMaterial!);
    //                Expression<Func<StreamJoiner, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.StreamJoiners.Cache.GetById(request.Id);
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

    //    public static StreamJoinerDTO Map(this StreamJoiner row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new StreamJoinerDTO()
    //        {
    //            Id = row.Id,
    //            MainProcessId = row.MainProcessId,
               
           
    //            Name = row.Name,
    //            EquipmentType = row.ProccesEquipmentType,
    //            Order = row.Order,
    //            FocusFactory = row.FocusFactory,
    //        };
    //    }

    //}
    //public static class ValidateStreamJoinersNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.StreamJoiners.EndPoint.Validate, async (ValidateStreamJoinerNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<StreamJoiner, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
    //                Func<StreamJoiner, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.StreamJoiners.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
