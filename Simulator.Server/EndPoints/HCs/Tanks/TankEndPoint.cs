using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Tanks;

namespace Simulator.Server.EndPoints.HCs.Tanks
{
    public class TankEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Tank>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(Tank)} table");
                }


                return Result.Success($"{nameof(Tank)} table Updated");

            });

            app.MapPost("api/getall/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Tank>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => x.MapToDto<TankDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Tank>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<TankDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Tank>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(Tank)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(Tank)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/TankDTO", async (List<TankDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(Tank)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Tank>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(Tank)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Tank>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Tank moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Tank>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Tank moved down successfully");
            });
            // En TankEndpointExtensions
            app.MapPost("api/validate/TankDTO", async (TankDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Tank>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(Tank)} table, already exists");
            });
        }
    }
    //public static class CreateUpdateTankEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Tanks.EndPoint.CreateUpdate, async (TankDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new List<string>();

    //                Tank? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = Tank.Create(Data.MainProcessId);

    //                    await Data.InletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.OutletConnectors.Create(row.Id, Repository, cache);
    //                    await Data.MaterialEquipments.Create(row.Id, Repository, cache);



    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<Tank>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    Expression<Func<MaterialEquipment, bool>> Criteria = x => x.ProccesEquipmentId == Data.Id;



    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                cache.AddRange(StaticClass.Tanks.Cache.Key(row.Id, row.MainProcessId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static Tank Map(this TankDTO request, Tank row)
    //    {
    //        row.CapacityValue = request.CapacityValue;
    //        row.CapacityUnit = request.CapacityUnitName;
    //        row.Name = request.Name;
    //        row.MaxLevelUnit = request.MaxLevelUnitName;
    //        row.MaxLevelValue = request.MaxLevelValue;
    //        row.MinLevelValue = request.MinLevelValue;
    //        row.MinLevelUnit = request.MinLevelUnitName;
    //        row.LoLoLevelUnit = request.LoLoLevelUnitName;
    //        row.LoLoLevelValue = request.LoLoLevelValue;
    //        row.IsStorageForOneFluid = request.IsStorageForOneFluid;
    //        row.TankCalculationType = request.TankCalculationType;
    //        row.FluidStorage = request.FluidStorage;
    //        row.InitialLevelValue = request.InitialLevelValue;
    //        row.InitialLevelUnit = request.InitialLevelUnitName;
    //        row.FocusFactory = request.FocusFactory;
    //        row.ProccesEquipmentType = ProccesEquipmentType.Tank;
    //        return row;
    //    }

    //}
    //public static class DeleteTankEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Tanks.EndPoint.Delete, async (DeleteTankRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<Tank>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.Tanks.Cache.Key(row.Id, row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupTankEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Tanks.EndPoint.DeleteGroup, async (DeleteGroupTankRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<Tank>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.Tanks.Cache.GetAll(Data.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllTankEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Tanks.EndPoint.GetAll, async (TankGetAll request, IQueryRepository Repository) =>
    //            {

    //                //  Func<IQueryable<Tank>, IIncludableQueryable<Tank, object>> includes = x => x
    //                //.Include(y => y.Materials).ThenInclude(x => x.Material);
    //                Expression<Func<Tank, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //                string CacheKey = StaticClass.Tanks.Cache.GetAll(request.MainProcessId);
    //                var rows = await Repository.GetAllAsync<Tank>(Cache: CacheKey, Criteria: Criteria);

    //                if (rows == null)
    //                {
    //                    return Result<TankResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Tanks.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //                TankResponseList response = new TankResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<TankResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetTankByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Tanks.EndPoint.GetById, async (GetTankByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                // Func<IQueryable<Tank>, IIncludableQueryable<Tank, object>> includes = x => x
    //                //.Include(y => y.Materials).ThenInclude(x => x.Material);
    //                Expression<Func<Tank, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.Tanks.Cache.GetById(request.Id);
    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria);

    //                if (row == null)
    //                {
    //                    return Result.Fail(request.NotFound);
    //                }

    //                var response = row.Map();
    //                return Result.Success(response);

    //            });
    //        }
    //    }

    //    public static TankDTO Map(this Tank row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new TankDTO()
    //        {
    //            Id = row.Id,
    //            MainProcessId = row.MainProcessId,
    //            CapacityValue = row.CapacityValue,
    //            CapacityUnitName = row.CapacityUnit,
    //            MaxLevelValue = row.MaxLevelValue,
    //            MaxLevelUnitName = row.MaxLevelUnit,
    //            MinLevelUnitName = row.MinLevelUnit,
    //            MinLevelValue = row.MinLevelValue,
    //            LoLoLevelUnitName = row.LoLoLevelUnit,
    //            LoLoLevelValue = row.LoLoLevelValue,
    //            IsStorageForOneFluid = row.IsStorageForOneFluid,
    //            FluidStorage = row.FluidStorage,
    //            TankCalculationType = row.TankCalculationType,
    //            InitialLevelUnitName = row.InitialLevelUnit,
    //            InitialLevelValue = row.InitialLevelValue,
    //            Name = row.Name,

    //            FocusFactory = row.FocusFactory,
    //            Order = row.Order,
    //            EquipmentType = ProccesEquipmentType.Tank,


    //        };
    //    }

    //}
    //public static class ValidateTanksNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Tanks.EndPoint.Validate, async (ValidateTankNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<Tank, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
    //                Func<Tank, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.Tanks.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
