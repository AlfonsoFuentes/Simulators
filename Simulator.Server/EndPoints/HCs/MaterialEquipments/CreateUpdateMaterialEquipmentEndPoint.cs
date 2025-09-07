using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Models.HCs.MaterialEquipments;

namespace Simulator.Server.EndPoints.HCs.MaterialEquipments
{
    public static class CreateMaterialEquipment
    {
        public static async Task Create(this List<MaterialEquipmentDTO> MaterialEquipments, Guid Id, IRepository Repository, List<string> cache)
        {
            foreach (var materialequpment in MaterialEquipments)
            {
                var matequipm = MaterialEquipment.Create(Id, materialequpment.MainProcessId);
                materialequpment.Map(matequipm);


                await Repository.AddAsync(matequipm);
                cache.AddRange(StaticClass.MaterialEquipments.Cache.Key(matequipm.Id, Id, materialequpment.MainProcessId));
            }
        }
    }
    public static class CreateUpdateMaterialEquipmentEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MaterialEquipments.EndPoint.CreateUpdate, async (MaterialEquipmentDTO Data, IRepository Repository) =>
                {

                    MaterialEquipment? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        if (Data.ProccesEquipmentId != Guid.Empty && Data.MaterialId != Guid.Empty)
                        {
                            row = MaterialEquipment.Create(Data.ProccesEquipmentId, Data.MainProcessId);


                            await Repository.AddAsync(row);
                        }

                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<MaterialEquipment>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        if (Data.ProccesEquipmentId != Guid.Empty && Data.MaterialId != Guid.Empty)
                        {
                            await Repository.UpdateAsync(row);
                        }
                         
                    }
                    if (row == null) { return Result.Fail(Data.NotFound); }

                    Data.Map(row);
                    List<string> cache = [.. StaticClass.MaterialEquipments.Cache.Key(row.Id, row.ProccesEquipmentId, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static MaterialEquipment Map(this MaterialEquipmentDTO request, MaterialEquipment row)
        {
            row.MaterialId = request.MaterialId;

            row.CapacityValue = request.CapacityValue;
            row.CapacityUnit = request.CapacityUnitName;
            row.IsMixer = request.IsMixer;
            return row;
        }

    }
    public static class DeleteMaterialEquipmentEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MaterialEquipments.EndPoint.Delete, async (DeleteMaterialEquipmentRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<MaterialEquipment>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.MaterialEquipments.Cache.Key(row.Id, row.ProccesEquipmentId, row.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupMaterialEquipmentEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MaterialEquipments.EndPoint.DeleteGroup, async (DeleteGroupMaterialEquipmentRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<MaterialEquipment>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.MaterialEquipments.Cache.GetAllByMaterial(Data.EquipmentId), StaticClass.MaterialEquipments.Cache.GetAll(Data.MainProcessId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllMaterialEquipmentEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MaterialEquipments.EndPoint.GetAll, async (MaterialEquipmentGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<MaterialEquipment>, IIncludableQueryable<MaterialEquipment, object>> includes = x => x
                   .Include(y => y.Material);
                    Expression<Func<MaterialEquipment, bool>> Criteria = x => x.ProccesEquipmentId == request.EquipmentId;
                    string CacheKey = StaticClass.MaterialEquipments.Cache.GetAllByMaterial(request.EquipmentId);
                    var rows = await Repository.GetAllAsync<MaterialEquipment>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

                    if (rows == null)
                    {
                        return Result<MaterialEquipmentResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.MaterialEquipments.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    MaterialEquipmentResponseList response = new MaterialEquipmentResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialEquipmentResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetMaterialEquipmentByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MaterialEquipments.EndPoint.GetById, async (GetMaterialEquipmentByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<MaterialEquipment>, IIncludableQueryable<MaterialEquipment, object>> includes = x => x
                    .Include(y => y.ProccesEquipment!)
                    .Include(y => y.Material!);
                    Expression<Func<MaterialEquipment, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.MaterialEquipments.Cache.GetById(request.Id);
                    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria, Includes: includes);

                    if (row == null)
                    {
                        return Result.Fail(request.NotFound);
                    }

                    var response = row.Map();
                    return Result.Success(response);

                });
            }
        }

        public static MaterialEquipmentDTO Map(this MaterialEquipment row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new MaterialEquipmentDTO()
            {
                Id = row.Id,
                Material = row.Material == null ? null! : row.Material.MapMaterial(),
                ProccesEquipmentId = row.ProccesEquipmentId,
                CapacityValue = row.CapacityValue,
                CapacityUnitName = row.CapacityUnit,
                IsMixer = row.IsMixer,
                Order = row.Order,
                MainProcessId = row.MainProcessId,
            };
        }

    }
    public static class ValidateMaterialEquipmentsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MaterialEquipments.EndPoint.Validate, async (ValidateMaterialEquipmentNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<MaterialEquipment, bool>> CriteriaId = null!;
                    Func<MaterialEquipment, bool> CriteriaExist = x => Data.Id == null ?
                    x.MaterialId == Data.MaterialId && x.ProccesEquipmentId == Data.EquipmentId : x.Id != Data.Id.Value && x.MaterialId == Data.MaterialId && x.ProccesEquipmentId == Data.EquipmentId;
                    string CacheKey = StaticClass.MaterialEquipments.Cache.GetAllByMaterial(Data.EquipmentId);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
