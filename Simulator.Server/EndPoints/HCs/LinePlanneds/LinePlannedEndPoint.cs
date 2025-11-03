using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.LinePlanneds;


namespace Simulator.Server.EndPoints.HCs.LinePlanneds
{
    public class LinePlannedEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<LinePlanned>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(LinePlanned)} table");
                }


                return Result.Success($"{nameof(LinePlanned)} table Updated");

            });

            app.MapPost("api/getall/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<LinePlanned>(dto, parentId: $"{dto.SimulationPlannedId}");

                var result = query.Select(x => x.MapToDto<LinePlannedDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<LinePlanned>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<LinePlannedDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<LinePlanned>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(LinePlanned)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(LinePlanned)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/LinePlannedDTO", async (List<LinePlannedDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(LinePlanned)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<LinePlanned>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(LinePlanned)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<LinePlanned>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("LinePlanned moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<LinePlanned>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("LinePlanned moved down successfully");
            });
            // En LinePlannedEndpointExtensions
            app.MapPost("api/validate/LinePlannedDTO", async (LinePlannedDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<LinePlanned>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(LinePlanned)} table, already exists");
            });
        }
    }
    //public static class CreateLinePlanneds
    //{
    //    public static async Task Create(this List<LinePlannedDTO> LinessPlanned, Guid Id, IRepository Repository, List<string> cache)
    //    {
    //        foreach (var item in LinessPlanned)
    //        {
    //            var lineplanned = LinePlanned.Create(Id);
    //            item.Map(lineplanned);
    //            await Repository.AddAsync(lineplanned);
    //            cache.AddRange(StaticClass.LinePlanneds.Cache.Key(lineplanned.Id, Id));
    //            await item.PlannedSKUDTOs.Create(lineplanned.Id, Repository, cache);
    //        }
    //    }
    //}
    //public static class CreateUpdateLinePlannedEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.LinePlanneds.EndPoint.CreateUpdate, async (LinePlannedDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                LinePlanned? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = LinePlanned.Create(Data.SimulationPlannedId);
    //                    await Data.PlannedSKUDTOs.Create(row.Id, Repository, cache);

    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<LinePlanned>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.LinePlanneds.Cache.Key(row.Id, row.SimulationPlannedId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static LinePlanned Map(this LinePlannedDTO request, LinePlanned row)
    //    {
    //        row.ShiftType = request.ShiftType;
    //        row.WIPLevelValue = request.WIPLevelValue;
    //        row.WIPLevelUnit = request.WIPLevelUnitName;
    //        row.LineId = request.LineId;

    //        return row;
    //    }

    //}
    //public static class DeleteLinePlannedEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.LinePlanneds.EndPoint.Delete, async (DeleteLinePlannedRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<LinePlanned>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.LinePlanneds.Cache.Key(row.Id, row.SimulationPlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupLinePlannedEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.LinePlanneds.EndPoint.DeleteGroup, async (DeleteGroupLinePlannedRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<LinePlanned>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.LinePlanneds.Cache.GetAll(Data.SimulationPlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllLinePlannedEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.LinePlanneds.EndPoint.GetAll, async (LinePlannedGetAll request, IQueryRepository Repository) =>
    //            {

    //                Func<IQueryable<LinePlanned>, IIncludableQueryable<LinePlanned, object>> includes = x => x
    //               .Include(y => y.Line)
    //               .Include(x => x.HCSimulationPlanned)
    //               .Include(x => x.PreferedMixers).ThenInclude(x => x.Mixer);
    //                Expression<Func<LinePlanned, bool>> Criteria = x => x.SimulationPlannedId == request.SimulationPlannedId;
    //                string CacheKey = StaticClass.LinePlanneds.Cache.GetAll(request.SimulationPlannedId);
    //                var rows = await Repository.GetAllAsync<LinePlanned>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (rows == null)
    //                {
    //                    return Result<LinePlannedResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.LinePlanneds.ClassLegend));
    //                }

    //                var maps = rows.Select(x => x.Map()).ToList();


    //                LinePlannedResponseList response = new LinePlannedResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<LinePlannedResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetLinePlannedByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.LinePlanneds.EndPoint.GetById, async (GetLinePlannedByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                Func<IQueryable<LinePlanned>, IIncludableQueryable<LinePlanned, object>> includes = x => x
    //               .Include(y => y.Line)
    //               .Include(x => x.HCSimulationPlanned);
    //                Expression<Func<LinePlanned, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.LinePlanneds.Cache.GetById(request.Id);
    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (row == null)
    //                {
    //                    return Result.Fail(request.NotFound);
    //                }

    //                var response = row.Map();
    //                return Result.Success(response);

    //            });
    //        }
    //    }

    //    public static LinePlannedDTO Map(this LinePlanned row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new LinePlannedDTO()
    //        {
    //            MainProcesId = row.HCSimulationPlanned == null ? Guid.Empty : row.HCSimulationPlanned.MainProcessId,
    //            Id = row.Id,
    //            LineDTO = row.Line == null ? null! : row.Line.MapToDto<LineDTO>(),
    //            WIPLevelValue = row.WIPLevelValue,
    //            WIPLevelUnitName = row.WIPLevelUnit,
    //            ShiftType = row.ShiftType,
    //            SimulationPlannedId = row.SimulationPlannedId,
    //            Order = row.Order,
    //            PreferedMixerDTOs = row.PreferedMixers == null || row.PreferedMixers.Count == 0 ? new() : row.PreferedMixers.Select(x => x.Map()).ToList(),
    //        };
    //    }

    //}
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
