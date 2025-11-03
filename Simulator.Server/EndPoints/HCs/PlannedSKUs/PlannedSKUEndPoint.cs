using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Server.EndPoints.HCs.PlannedSKUs
{
    public class PlannedSKUEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<PlannedSKU>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(PlannedSKU)} table");
                }


                return Result.Success($"{nameof(PlannedSKU)} table Updated");

            });

            app.MapPost("api/getall/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<PlannedSKU>(dto, parentId: $"{dto.LinePlannedId}");

                var result = query.Select(x => x.MapToDto<PlannedSKUDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<PlannedSKU>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<PlannedSKUDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<PlannedSKU>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(PlannedSKU)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(PlannedSKU)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/PlannedSKUDTO", async (List<PlannedSKUDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(PlannedSKU)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<PlannedSKU>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(PlannedSKU)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<PlannedSKU>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("PlannedSKU moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<PlannedSKU>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("PlannedSKU moved down successfully");
            });
            // En PlannedSKUEndpointExtensions
            app.MapPost("api/validate/PlannedSKUDTO", async (PlannedSKUDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<PlannedSKU>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(PlannedSKU)} table, already exists");
            });
        }
    }
    //public static class CreateSKUPlanneds
    //{
    //    public static async Task Create(this List<PlannedSKUDTO> SKUsPlanned, Guid Id, IRepository Repository, List<string> cache)
    //    {
    //        foreach (var item in SKUsPlanned)
    //        {
    //            var skuplanned = PlannedSKU.Create(Id);
    //            item.Map(skuplanned);
    //            await Repository.AddAsync(skuplanned);
    //            cache.AddRange(StaticClass.PlannedSKUs.Cache.Key(skuplanned.Id, Id));
    //        }
    //    }
    //}
    //public static class CreateUpdatePlannedSKUEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.CreateUpdate, async (PlannedSKUDTO Data, IRepository Repository) =>
    //            {
    //                var lastorder = await GetLastOrder(Repository, Data.LinePlannedId);
    //                PlannedSKU? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = PlannedSKU.Create(Data.LinePlannedId);
    //                    row.Order = lastorder + 1;
    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<PlannedSKU>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                List<string> cache = [.. StaticClass.PlannedSKUs.Cache.Key(row.Id, row.LinePlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }
    //        async Task<int> GetLastOrder(IRepository Repository, Guid LinePlannedId)
    //        {
    //            Expression<Func<PlannedSKU, bool>> Criteria = x => x.LinePlannedId == LinePlannedId;
    //            var rows = await Repository.GetAllAsync(Criteria: Criteria);

    //            var lastorder = rows.Count > 0 ? rows.Max(x => x.Order) : 0;
    //            return lastorder;
    //        }
    //    }


    //    public static PlannedSKU Map(this PlannedSKUDTO request, PlannedSKU row)
    //    {
    //        row.SKUId = request.SKU!.Id;

    //        row.TimeToChangeSKUValue = request.TimeToChangeSKUValue;
    //        row.TimeToChangeSKUUnit = request.TimeToChangeSKUUnitName;
    //        row.PlannedCases = request.PlannedCases;

    //        row.LineSpeedUnit = request.LineSpeedUnitName;
    //        row.LineSpeedValue = request.LineSpeedValue;

    //        return row;
    //    }

    //}
    //public static class DeletePlannedSKUEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.Delete, async (DeletePlannedSKURequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<PlannedSKU>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.PlannedSKUs.Cache.Key(row.Id, row.LinePlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupPlannedSKUEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.DeleteGroup, async (DeleteGroupPlannedSKURequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<PlannedSKU>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.PlannedSKUs.Cache.GetAll(Data.LinePlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllPlannedSKUEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.GetAll, async (PlannedSKUGetAll request, IQueryRepository Repository) =>
    //            {

    //                Func<IQueryable<PlannedSKU>, IIncludableQueryable<PlannedSKU, object>> includes = x => x

    //               .Include(x => x.LinePlanned)
    //               .Include(y => y.SKU).ThenInclude(x => x.SKULines);
    //                Expression<Func<PlannedSKU, bool>> Criteria = x => x.LinePlannedId == request.LinePlannedId;
    //                string CacheKey = StaticClass.PlannedSKUs.Cache.GetAll(request.LinePlannedId);
    //                var rows = await Repository.GetAllAsync<PlannedSKU>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (rows == null)
    //                {
    //                    return Result<PlannedSKUResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.PlannedSKUs.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();


    //                PlannedSKUResponseList response = new PlannedSKUResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<PlannedSKUResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetPlannedSKUByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.GetById, async (GetPlannedSKUByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                Func<IQueryable<PlannedSKU>, IIncludableQueryable<PlannedSKU, object>> includes = x => x
    //                .Include(x => x.LinePlanned)
    //              .Include(y => y.SKU).ThenInclude(x => x.SKULines)
    //              ;
    //                Expression<Func<PlannedSKU, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.PlannedSKUs.Cache.GetById(request.Id);
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

    //    public static PlannedSKUDTO Map(this PlannedSKU row)
    //    {


    //        PlannedSKUDTO result = new PlannedSKUDTO()
    //        {
    //            Id = row.Id,
    //            PlannedCases = row.PlannedCases,
    //            //Case_Shift = row.SKU.SKULines.Case_Shift,
    //            TimeToChangeSKUUnitName = row.TimeToChangeSKUUnit,
    //            TimeToChangeSKUValue = row.TimeToChangeSKUValue,
    //            LinePlannedId = row.LinePlannedId,
    //            SKU = row.SKU == null ? null! : row.SKU.MapToDto<SKUDTO>(),
    //            LineId = row.LinePlanned == null ? Guid.Empty : row.LinePlanned.LineId,
    //            Order = row.Order,
    //            LineSpeedUnitName = row.LineSpeedUnit,
    //            LineSpeedValue = row.LineSpeedValue,

    //        };
    //        var skulines = row.SKU == null ? null! : row.SKU.SKULines;
    //        if (skulines != null && skulines.Any())
    //        {
    //            var case_shift = skulines.FirstOrDefault(x => x.LineId == result.LineId);
    //            result.Case_Shift = case_shift == null ? 0 : case_shift.Case_Shift;
    //        }
    //        return result;
    //    }

    //}
    //public static class ValidatePlannedSKUsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PlannedSKUs.EndPoint.Validate, async (ValidatePlannedSKUNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<HCPlannedSKU, bool>> CriteriaId = null!;
    //                Func<HCPlannedSKU, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.PlannedSKUs.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
