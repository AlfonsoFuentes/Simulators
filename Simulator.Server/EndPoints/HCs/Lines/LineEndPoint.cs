using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Server.EndPoints.HCs.SKULines;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Lines;

namespace Simulator.Server.EndPoints.HCs.Lines
{
    public class LineEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Line>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(Line)} table");
                }


                return Result.Success($"{nameof(Line)} table Updated");

            });

            app.MapPost("api/getall/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Line>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => x.MapToDto<LineDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Line>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<LineDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Line>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(Line)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(Line)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/LineDTO", async (List<LineDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(Line)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Line>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(Line)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Line>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Line moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Line>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Line moved down successfully");
            });
            // En LineEndpointExtensions
            app.MapPost("api/validate/LineDTO", async (LineDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Line>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(Line)} table, already exists");
            });
        }
    }
}
    //public static class CreateUpdateLineEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Lines.EndPoint.CreateUpdate, async (LineDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                Line? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = Line.Create(Data.MainProcessId);
    //                    await Data.InletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.PlannedDownTimes.Create(row.Id, Repository, cache);
    //                    await Data.LineSKUs.Create(row.Id, Repository, cache);

    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<Line>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.Lines.Cache.Key(row.Id, row.MainProcessId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    //    public static Line Map(this LineDTO request, Line row)
    //    //    {
    //    //        row.TimeToReviewAUValue = request.TimeToReviewAUValue;
    //    //        row.TimeToReviewAUUnit = request.TimeToReviewAUUnitName;
    //    //        row.PackageType = request.PackageType;
    //    //        row.ProccesEquipmentType = ProccesEquipmentType.Line;
    //    //        row.Name = request.Name;
    //    //        row.FocusFactory = request.FocusFactory;


    //    //        return row;
    //    //    }

    //    //}
    //    //public static class DeleteLineEndPoint
    //    //{
    //    //    public class EndPoint : IEndPoint
    //    //    {
    //    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //    //        {
    //    //            app.MapPost(StaticClass.Lines.EndPoint.Delete, async (DeleteLineRequest Data, IRepository Repository) =>
    //    //            {
    //    //                var row = await Repository.GetByIdAsync<Line>(Data.Id);
    //    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //    //                await Repository.RemoveAsync(row);

    //    //                List<string> cache = [.. StaticClass.Lines.Cache.Key(row.Id, row.MainProcessId)];

    //    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //    //                return Result.EndPointResult(result,
    //    //                    Data.Succesfully,
    //    //                    Data.Fail);

    //    //            });
    //    //        }
    //    //    }




    //    //}
    //    //public static class DeleteGroupLineEndPoint
    //    //{
    //    //    public class EndPoint : IEndPoint
    //    //    {
    //    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //    //        {
    //    //            app.MapPost(StaticClass.Lines.EndPoint.DeleteGroup, async (DeleteGroupLineRequest Data, IRepository Repository) =>
    //    //            {
    //    //                foreach (var rowItem in Data.SelecteItems)
    //    //                {
    //    //                    var row = await Repository.GetByIdAsync<Line>(rowItem.Id);
    //    //                    if (row != null)
    //    //                    {
    //    //                        await Repository.RemoveAsync(row);
    //    //                    }
    //    //                }


    //    //                List<string> cache = [StaticClass.Lines.Cache.GetAll(Data.MainProcessId)];

    //    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //    //                return Result.EndPointResult(result,
    //    //                    Data.Succesfully,
    //    //                    Data.Fail);

    //    //            });
    //    //        }
    //    //    }




    //    //}
    //    //public static class GetAllLineEndPoint
    //    //{
    //    //    public class EndPoint : IEndPoint
    //    //    {
    //    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //    //        {
    //    //            app.MapPost(StaticClass.Lines.EndPoint.GetAll, async (LineGetAll request, IQueryRepository Repository) =>
    //    //            {

    //    //                // Func<IQueryable<HCLine>, IIncludableQueryable<HCLine, object>> includes = x => x
    //    //                //.Include(y => y.!);
    //    //                Expression<Func<Line, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //    //                string CacheKey = StaticClass.Lines.Cache.GetAll(request.MainProcessId);
    //    //                var rows = await Repository.GetAllAsync<Line>(Cache: CacheKey, Criteria: Criteria);

    //    //                if (rows == null)
    //    //                {
    //    //                    return Result<LineResponseList>.Fail(
    //    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Lines.ClassLegend));
    //    //                }

    //    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //    //                LineResponseList response = new LineResponseList()
    //    //                {
    //    //                    Items = maps
    //    //                };
    //    //                return Result<LineResponseList>.Success(response);

    //    //            });
    //    //        }
    //    //    }
    //    //}
    //    //public static class GetLineByIdEndPoint
    //    //{
    //    //    public class EndPoint : IEndPoint
    //    //    {
    //    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //    //        {
    //    //            app.MapPost(StaticClass.Lines.EndPoint.GetById, async (GetLineByIdRequest request, IQueryRepository Repository) =>
    //    //            {
    //    //                //Func<IQueryable<HCLine>, IIncludableQueryable<HCLine, object>> includes = x => x
    //    //                //.Include(y => y.RawMaterial!);
    //    //                Expression<Func<Line, bool>> Criteria = x => x.Id == request.Id;

    //    //                string CacheKey = StaticClass.Lines.Cache.GetById(request.Id);
    //    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria/*, Includes: includes*/);

    //    //                if (row == null)
    //    //                {
    //    //                    return Result.Fail(request.NotFound);
    //    //                }

    //    //                var response = row.Map();
    //    //                return Result.Success(response);

    //    //            });
    //    //        }
    //    //    }

    //    //    public static LineDTO Map(this Line row)
    //    //    {
    //    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //    //        return new LineDTO()
    //    //        {
    //    //            Id = row.Id,
    //    //            MainProcessId = row.MainProcessId,
    //    //            TimeToReviewAUValue = row.TimeToReviewAUValue,
    //    //            TimeToReviewAUUnitName = row.TimeToReviewAUUnit,
    //    //            PackageType = row.PackageType,
    //    //            EquipmentType = row.ProccesEquipmentType,
    //    //            Name = row.Name,
    //    //            FocusFactory = row.FocusFactory,
    //    //            Order = row.Order,
    //    //        };
    //    //    }

    //    //}
    //    //public static class ValidateLinesNameEndPoint
    //    //{
    //    //    public class EndPoint : IEndPoint
    //    //    {
    //    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //    //        {
    //    //            app.MapPost(StaticClass.Lines.EndPoint.Validate, async (ValidateLineNameRequest Data, IQueryRepository Repository) =>
    //    //            {
    //    //                Expression<Func<Line, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
    //    //                Func<Line, bool> CriteriaExist = x => Data.Id == null ?
    //    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //    //                string CacheKey = StaticClass.Lines.Cache.GetAll(Data.MainProcessId);

    //    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //    //            });


    //    //        }
    //    //    }



    //    //}
    //}
