using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Operators;

namespace Simulator.Server.EndPoints.HCs.Operators
{
    public class OperatorEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Operator>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(Operator)} table");
                }


                return Result.Success($"{nameof(Operator)} table Updated");

            });

            app.MapPost("api/getall/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Operator>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => x.MapToDto<OperatorDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Operator>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<OperatorDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Operator>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(Operator)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(Operator)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/OperatorDTO", async (List<OperatorDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(Operator)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Operator>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(Operator)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Operator>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Operator moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Operator>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Operator moved down successfully");
            });
            // En OperatorEndpointExtensions
            app.MapPost("api/validate/OperatorDTO", async (OperatorDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Operator>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(Operator)} table, already exists");
            });
        }
    }
    //public static class CreateUpdateOperatorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Operators.EndPoint.CreateUpdate, async (OperatorDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new List<string>();
    //                Operator? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = Operator.Create(Data.MainProcessId);
    //                    await Data.InletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.OutletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.MaterialEquipments.Create(row.Id, Repository, cache);
    //                    await Data.PlannedDownTimes.Create(row.Id, Repository, cache);

    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<Operator>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.Operators.Cache.Key(row.Id, row.MainProcessId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static Operator Map(this OperatorDTO request, Operator row)
    //    {

    //        row.Name = request.Name;
    //        row.ProccesEquipmentType = ProccesEquipmentType.Operator;
    //        row.FocusFactory = request.FocusFactory;

    //        return row;
    //    }

    //}
    //public static class DeleteOperatorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Operators.EndPoint.Delete, async (DeleteOperatorRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<Operator>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.Operators.Cache.Key(row.Id, row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupOperatorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Operators.EndPoint.DeleteGroup, async (DeleteGroupOperatorRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<Operator>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.Operators.Cache.GetAll(Data.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllOperatorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Operators.EndPoint.GetAll, async (OperatorGetAll request, IQueryRepository Repository) =>
    //            {

    //                // Func<IQueryable<HCOperator>, IIncludableQueryable<HCOperator, object>> includes = x => x
    //                //.Include(y => y.!);
    //                Expression<Func<Operator, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //                string CacheKey = StaticClass.Operators.Cache.GetAll(request.MainProcessId);
    //                var rows = await Repository.GetAllAsync<Operator>(Cache: CacheKey, Criteria: Criteria);

    //                if (rows == null)
    //                {
    //                    return Result<OperatorResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Operators.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //                OperatorResponseList response = new OperatorResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<OperatorResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetOperatorByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Operators.EndPoint.GetById, async (GetOperatorByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                //Func<IQueryable<HCOperator>, IIncludableQueryable<HCOperator, object>> includes = x => x
    //                //.Include(y => y.RawMaterial!);
    //                Expression<Func<Operator, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.Operators.Cache.GetById(request.Id);
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

    //    public static OperatorDTO Map(this Operator row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new OperatorDTO()
    //        {
    //            Id = row.Id,
    //            MainProcessId = row.MainProcessId,
    //            FocusFactory = row.FocusFactory,
    //            Name = row.Name,
    //            EquipmentType = row.ProccesEquipmentType,
    //            Order = row.Order,
    //        };
    //    }

    //}
    //public static class ValidateOperatorsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Operators.EndPoint.Validate, async (ValidateOperatorNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<Operator, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
    //                Func<Operator, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.Operators.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
