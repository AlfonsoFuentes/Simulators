using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.ContinuousSystems;

namespace Simulator.Server.EndPoints.HCs.ContinuousSystems
{
    public class ContinuousSystemEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<ContinuousSystem>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(ContinuousSystem)} table");
                }


                return Result.Success($"{nameof(ContinuousSystem)} table Updated");

            });

            app.MapPost("api/getall/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<ContinuousSystem>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => x.MapToDto<ContinuousSystemDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<ContinuousSystem>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<ContinuousSystemDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<ContinuousSystem>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(ContinuousSystem)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(ContinuousSystem)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/ContinuousSystemDTO", async (List<ContinuousSystemDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(ContinuousSystem)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<ContinuousSystem>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(ContinuousSystem)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<ContinuousSystem>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("ContinuousSystem moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<ContinuousSystem>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("ContinuousSystem moved down successfully");
            });
            // En ContinuousSystemEndpointExtensions
            app.MapPost("api/validate/ContinuousSystemDTO", async (ContinuousSystemDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<ContinuousSystem>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(ContinuousSystem)} table, already exists");
            });
        }
    }
    //public static class CreateUpdateContinuousSystemEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ContinuousSystems.EndPoint.CreateUpdate, async (ContinuousSystemDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                ContinuousSystem? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = ContinuousSystem.Create(Data.MainProcessId);
    //                    await Data.InletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.OutletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.MaterialEquipments.Create(row.Id, Repository, cache);
    //                    await Data.PlannedDownTimes.Create(row.Id, Repository, cache);

    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<ContinuousSystem>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.ContinuousSystems.Cache.Key(row.Id, row.MainProcessId));


    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static ContinuousSystem Map(this ContinuousSystemDTO request, ContinuousSystem row)
    //    {
    //        row.FlowValue = request.FlowValue;
    //        row.FlowUnit = request.FlowUnitName;
    //        row.ProccesEquipmentType = ProccesEquipmentType.ContinuousSystem;
    //        row.Name = request.Name;

    //        row.FocusFactory = request.FocusFactory;

    //        return row;
    //    }

    //}
    //public static class DeleteContinuousSystemEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ContinuousSystems.EndPoint.Delete, async (DeleteContinuousSystemRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<ContinuousSystem>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.ContinuousSystems.Cache.Key(row.Id, row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupContinuousSystemEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ContinuousSystems.EndPoint.DeleteGroup, async (DeleteGroupContinuousSystemRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<ContinuousSystem>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.ContinuousSystems.Cache.GetAll(Data.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllContinuousSystemEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ContinuousSystems.EndPoint.GetAll, async (ContinuousSystemGetAll request, IQueryRepository Repository) =>
    //            {

    //                // Func<IQueryable<HCContinuousSystem>, IIncludableQueryable<HCContinuousSystem, object>> includes = x => x
    //                //.Include(y => y.!);
    //                Expression<Func<ContinuousSystem, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //                string CacheKey = StaticClass.ContinuousSystems.Cache.GetAll(request.MainProcessId);
    //                var rows = await Repository.GetAllAsync<ContinuousSystem>(Cache: CacheKey, Criteria: Criteria);

    //                if (rows == null)
    //                {
    //                    return Result<ContinuousSystemResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.ContinuousSystems.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //                ContinuousSystemResponseList response = new ContinuousSystemResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<ContinuousSystemResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetContinuousSystemByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ContinuousSystems.EndPoint.GetById, async (GetContinuousSystemByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                //Func<IQueryable<HCContinuousSystem>, IIncludableQueryable<HCContinuousSystem, object>> includes = x => x
    //                //.Include(y => y.RawMaterial!);
    //                Expression<Func<ContinuousSystem, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.ContinuousSystems.Cache.GetById(request.Id);
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

    //    public static ContinuousSystemDTO Map(this ContinuousSystem row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new ContinuousSystemDTO()
    //        {
    //            Id = row.Id,
    //            MainProcessId = row.MainProcessId,
    //            FlowValue = row.FlowValue,
    //            FlowUnitName = row.FlowUnit,
    //            EquipmentType = row.ProccesEquipmentType,
    //            Name = row.Name,
    //            FocusFactory = row.FocusFactory,
    //            Order = row.Order,
    //        };
    //    }

    //}
    //public static class ValidateContinuousSystemsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ContinuousSystems.EndPoint.Validate, async (ValidateContinuousSystemNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<ContinuousSystem, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
    //                Func<ContinuousSystem, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.ContinuousSystems.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
