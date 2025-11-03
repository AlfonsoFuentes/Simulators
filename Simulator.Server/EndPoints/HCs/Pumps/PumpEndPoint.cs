using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Pumps;

namespace Simulator.Server.EndPoints.HCs.Pumps
{
    public class PumpEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Pump>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(Pump)} table");
                }


                return Result.Success($"{nameof(Pump)} table Updated");

            });

            app.MapPost("api/getall/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Pump>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => x.MapToDto<PumpDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Pump>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<PumpDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Pump>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(Pump)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(Pump)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/PumpDTO", async (List<PumpDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(Pump)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Pump>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(Pump)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Pump>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Pump moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Pump>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Pump moved down successfully");
            });
            // En PumpEndpointExtensions
            app.MapPost("api/validate/PumpDTO", async (PumpDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Pump>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(Pump)} table, already exists");
            });
        }
    }
    //public static class CreateUpdatePumpEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Pumps.EndPoint.CreateUpdate, async (PumpDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                Pump? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = Pump.Create(Data.MainProcessId);
    //                    await Data.PlannedDownTimes.Create(row.Id, Repository, cache);
    //                    await Data.InletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.OutletConnectors.Create(row.Id, Repository, cache);


    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<Pump>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.Pumps.Cache.Key(row.Id, row.MainProcessId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static Pump Map(this PumpDTO request, Pump row)
    //    {
    //        row.FlowValue = request.FlowValue;
    //        row.FlowUnit = request.FlowUnitName;
    //        row.IsForWashing = request.IsForWashing;
    //        row.Name = request.Name;

    //        row.ProccesEquipmentType = ProccesEquipmentType.Pump;
    //        row.FocusFactory = request.FocusFactory;
    //        return row;
    //    }

    //}
    //public static class DeletePumpEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Pumps.EndPoint.Delete, async (DeletePumpRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<Pump>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.Pumps.Cache.Key(row.Id, row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupPumpEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Pumps.EndPoint.DeleteGroup, async (DeleteGroupPumpRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<Pump>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.Pumps.Cache.GetAll(Data.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllPumpEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Pumps.EndPoint.GetAll, async (PumpGetAll request, IQueryRepository Repository) =>
    //            {

    //                // Func<IQueryable<HCPump>, IIncludableQueryable<HCPump, object>> includes = x => x
    //                //.Include(y => y.!);
    //                Expression<Func<Pump, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //                string CacheKey = StaticClass.Pumps.Cache.GetAll(request.MainProcessId);
    //                var rows = await Repository.GetAllAsync<Pump>(Cache: CacheKey, Criteria: Criteria);

    //                if (rows == null)
    //                {
    //                    return Result<PumpResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Pumps.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //                PumpResponseList response = new PumpResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<PumpResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetPumpByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Pumps.EndPoint.GetById, async (GetPumpByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                //Func<IQueryable<HCPump>, IIncludableQueryable<HCPump, object>> includes = x => x
    //                //.Include(y => y.RawMaterial!);
    //                Expression<Func<Pump, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.Pumps.Cache.GetById(request.Id);
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

    //    public static PumpDTO Map(this Pump row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new PumpDTO()
    //        {
    //            Id = row.Id,
    //            MainProcessId = row.MainProcessId,
    //            FlowValue = row.FlowValue,
    //            FlowUnitName = row.FlowUnit,
    //            IsForWashing = row.IsForWashing,
    //            Name = row.Name,
    //            EquipmentType = row.ProccesEquipmentType,
    //            Order = row.Order,
    //            FocusFactory = row.FocusFactory,
    //        };
    //    }

    //}
    //public static class ValidatePumpsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Pumps.EndPoint.Validate, async (ValidatePumpNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<Pump, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
    //                Func<Pump, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.Pumps.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
