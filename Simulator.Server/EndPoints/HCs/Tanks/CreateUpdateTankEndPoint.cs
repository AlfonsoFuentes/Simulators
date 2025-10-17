using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Conectors;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Tanks;

namespace Simulator.Server.EndPoints.HCs.Tanks
{
    public static class CreateUpdateTankEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Tanks.EndPoint.CreateUpdate, async (TankDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new List<string>();

                    Tank? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = Tank.Create(Data.MainProcessId);

                        await Data.InletConnectors.Create(row.Id, Repository, cache);
                        await Data.OutletConnectors.Create(row.Id, Repository, cache);
                        await Data.MaterialEquipments.Create(row.Id, Repository, cache);



                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<Tank>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        Expression<Func<MaterialEquipment, bool>> Criteria = x => x.ProccesEquipmentId == Data.Id;



                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    cache.AddRange(StaticClass.Tanks.Cache.Key(row.Id, row.MainProcessId));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static Tank Map(this TankDTO request, Tank row)
        {
            row.CapacityValue = request.CapacityValue;
            row.CapacityUnit = request.CapacityUnitName;
            row.Name = request.Name;
            row.MaxLevelUnit = request.MaxLevelUnitName;
            row.MaxLevelValue = request.MaxLevelValue;
            row.MinLevelValue = request.MinLevelValue;
            row.MinLevelUnit = request.MinLevelUnitName;
            row.LoLoLevelUnit = request.LoLoLevelUnitName;
            row.LoLoLevelValue = request.LoLoLevelValue;
            row.IsStorageForOneFluid = request.IsStorageForOneFluid;
            row.TankCalculationType = request.TankCalculationType;
            row.FluidStorage = request.FluidStorage;
            row.InitialLevelValue = request.InitialLevelValue;
            row.InitialLevelUnit = request.InitialLevelUnitName;
            row.FocusFactory = request.FocusFactory;
            row.ProccesEquipmentType = ProccesEquipmentType.Tank;
            return row;
        }

    }
    public static class DeleteTankEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Tanks.EndPoint.Delete, async (DeleteTankRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<Tank>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.Tanks.Cache.Key(row.Id, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupTankEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Tanks.EndPoint.DeleteGroup, async (DeleteGroupTankRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<Tank>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.Tanks.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllTankEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Tanks.EndPoint.GetAll, async (TankGetAll request, IQueryRepository Repository) =>
                {

                    //  Func<IQueryable<Tank>, IIncludableQueryable<Tank, object>> includes = x => x
                    //.Include(y => y.Materials).ThenInclude(x => x.Material);
                    Expression<Func<Tank, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.Tanks.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<Tank>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<TankResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Tanks.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    TankResponseList response = new TankResponseList()
                    {
                        Items = maps
                    };
                    return Result<TankResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetTankByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Tanks.EndPoint.GetById, async (GetTankByIdRequest request, IQueryRepository Repository) =>
                {
                    // Func<IQueryable<Tank>, IIncludableQueryable<Tank, object>> includes = x => x
                    //.Include(y => y.Materials).ThenInclude(x => x.Material);
                    Expression<Func<Tank, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.Tanks.Cache.GetById(request.Id);
                    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria);

                    if (row == null)
                    {
                        return Result.Fail(request.NotFound);
                    }

                    var response = row.Map();
                    return Result.Success(response);

                });
            }
        }

        public static TankDTO Map(this Tank row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new TankDTO()
            {
                Id = row.Id,
                MainProcessId = row.MainProcessId,
                CapacityValue = row.CapacityValue,
                CapacityUnitName = row.CapacityUnit,
                MaxLevelValue = row.MaxLevelValue,
                MaxLevelUnitName = row.MaxLevelUnit,
                MinLevelUnitName = row.MinLevelUnit,
                MinLevelValue = row.MinLevelValue,
                LoLoLevelUnitName = row.LoLoLevelUnit,
                LoLoLevelValue = row.LoLoLevelValue,
                IsStorageForOneFluid = row.IsStorageForOneFluid,
                FluidStorage = row.FluidStorage,
                TankCalculationType = row.TankCalculationType,
                InitialLevelUnitName = row.InitialLevelUnit,
                InitialLevelValue = row.InitialLevelValue,
                Name = row.Name,

                FocusFactory = row.FocusFactory,
                Order = row.Order,
                EquipmentType = ProccesEquipmentType.Tank,


            };
        }

    }
    public static class ValidateTanksNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Tanks.EndPoint.Validate, async (ValidateTankNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Tank, bool>> CriteriaId = x => x.MainProcessId == Data.MainProcessId;
                    Func<Tank, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.Tanks.Cache.GetAll(Data.MainProcessId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
