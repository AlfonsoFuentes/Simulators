using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.BackBoneSteps;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Materials;


namespace Simulator.Server.EndPoints.HCs.Materials
{
    public static class CreateUpdateMaterialEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.CreateUpdate, async (MaterialDTO Data, IRepository Repository) =>
                {
                    Material? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = Material.Create();
                        foreach (var stepdto in Data.BackBoneSteps.OrderBy(x => x.Order))
                        {
                            var step = row.AddBakBoneStep();
                            stepdto.Map(step);

                            await Repository.AddAsync(step);
                        }

                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<Material>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.Materials.Cache.Key(row.Id, row.FocusFactory)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }
        }


        public static Material Map(this MaterialDTO request, Material row)
        {
            row.SAPName = request.SAPName;
            row.PhysicalState = request.PhysicalState;
            row.M_Number = request.M_Number;
            row.SAPName = request.SAPName;
            row.IsForWashing = request.IsForWashing;
            row.ProductCategory = request.ProductCategory;
            row.CommonName = request.CommonName;
            row.MaterialType = request.MaterialType;
            row.FocusFactory = request.FocusFactory;
            return row;
        }

    }
    public static class DeleteMaterialEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.Delete, async (DeleteMaterialRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<Material>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.Materials.Cache.Key(row.Id, row.FocusFactory)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupMaterialEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.DeleteGroup, async (DeleteGroupMaterialRequest Data, IRepository Repository) =>
                {
                    Guid Id = Guid.NewGuid();
                    FocusFactory FocusFactory = FocusFactory.None;
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<Material>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                            FocusFactory = row.FocusFactory;
                            Id = row.Id;
                        }
                    }


                    List<string> cache = [.. StaticClass.Materials.Cache.Key(Id, FocusFactory)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllMaterialByFocusEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllMaterialByFocus, async (MaterialGetAllByFocusFactory request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                   .Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);
                    string CacheKey = StaticClass.Materials.Cache.GetAllMaterialByFocus(request.FocusFactory);
                    Expression<Func<Material, bool>> Criteria = x => x.FocusFactory == request.FocusFactory;
                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Includes: includes, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    if (request.MaterialType != MaterialType.None) rows = rows.Where(x => x.MaterialType == request.MaterialType).ToList();
                    var maps = rows.OrderBy(x => x.MaterialType).ThenBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);

                });
            }
        }


    }
    public static class GetAllMaterialEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllMaterial, async (MaterialGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                   .Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);
                    string CacheKey = StaticClass.Materials.Cache.GetAllMaterial;

                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Includes: includes);

                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    if (request.MaterialType != MaterialType.None) rows = rows.Where(x => x.MaterialType == request.MaterialType).ToList();
                    var maps = rows.OrderBy(x => x.MaterialType).ThenBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);

                });
            }
        }


    }
    public static class GetAllProductBackBoneEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllProductBackBone, async (ProductBackBoneGetAll request, IQueryRepository Repository) =>
                {

                    //     Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                    //.Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);

                    string CacheKey = StaticClass.Materials.Cache.GetAllProductBackBone(request.FocusFactory);
                    Expression<Func<Material, bool>> Criteria = request.FocusFactory == FocusFactory.None ? null! : x => x.FocusFactory == request.FocusFactory;
                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    rows = rows.Where(x => x.MaterialType == MaterialType.ProductBackBone).ToList();
                    var maps = rows.OrderBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetAllRawMaterialEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllRawMaterial, async (RawMaterialGetAll request, IQueryRepository Repository) =>
                {

                    //     Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                    //.Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);
                    string CacheKey = StaticClass.Materials.Cache.GetAllRawMaterial(request.FocusFactory);

                    Expression<Func<Material, bool>> Criteria = request.FocusFactory == FocusFactory.None ? null! : x => x.FocusFactory == request.FocusFactory;

                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    rows = rows.Where(x => x.MaterialType == MaterialType.RawMaterial || x.MaterialType == MaterialType.RawMaterialBackBone).ToList();
                    var maps = rows.OrderBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetAllRawMaterialSimpleEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllRawMaterialSimple, async (RawMaterialSimpleGetAll request, IQueryRepository Repository) =>
                {

                    //Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                    //    .Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);
                    string CacheKey = StaticClass.Materials.Cache.GetAllRawMaterialSimple(request.FocusFactory);
                    Expression<Func<Material, bool>> Criteria = request.FocusFactory == FocusFactory.None ? null! : x => x.FocusFactory == request.FocusFactory;


                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Criteria: Criteria);
                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    rows = rows.Where(x => x.MaterialType == MaterialType.RawMaterial).ToList();
                    var maps = rows.OrderBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetAllRawMaterialBackBoneEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllRawMaterialBackBone, async (RawMaterialBackBoneGetAll request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                    //   .Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);

                    string CacheKey = StaticClass.Materials.Cache.GetAllRawMaterialBackBone(request.FocusFactory);
                    Expression<Func<Material, bool>> Criteria = request.FocusFactory == FocusFactory.None ? null! : x => x.FocusFactory == request.FocusFactory;


                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Criteria: Criteria);


                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    rows = rows.Where(x => x.MaterialType == MaterialType.RawMaterialBackBone).ToList();
                    var maps = rows.OrderBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);




                });
            }
        }
    }
    public static class GetAllBackBonesEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetAllBackBone, async (BackBoneGetAll request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                    //   .Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);

                    string CacheKey = StaticClass.Materials.Cache.GetAllBackBone(request.FocusFactory);
                    Expression<Func<Material, bool>> Criteria = request.FocusFactory == FocusFactory.None ? null! : x => x.FocusFactory == request.FocusFactory;


                    var rows = await Repository.GetAllAsync<Material>(Cache: CacheKey, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<MaterialResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Materials.ClassLegend));
                    }
                    rows = rows.Where(x => x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone).ToList();
                    var maps = rows.OrderBy(x => x.CommonName).Select(x => x.MapMaterial()).ToList();


                    MaterialResponseList response = new MaterialResponseList()
                    {
                        Items = maps
                    };
                    return Result<MaterialResponseList>.Success(response);




                });
            }
        }
    }
    public static class GetMaterialByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.GetById, async (GetMaterialByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<Material>, IIncludableQueryable<Material, object>> includes = x => x
                    //.Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);
                    Expression<Func<Material, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.Materials.Cache.GetById(request.Id);
                    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria);

                    if (row == null)
                    {
                        return Result.Fail(request.NotFound);
                    }

                    var response = row.MapMaterial();
                    return Result.Success(response);

                });
            }
        }

        public static MaterialDTO MapMaterial(this Material row)
        {
            return new()
            {
                Id = row.Id,
                FocusFactory = row.FocusFactory,
                M_Number = row.M_Number,
                SAPName = row.SAPName,
                PhysicalState = row.PhysicalState,
                ProductCategory = row.ProductCategory,
                IsForWashing = row.IsForWashing,
                MaterialType = row.MaterialType,
                CommonName = row.CommonName,
                SumOfPercentage = row.BackBoneSteps == null || row.BackBoneSteps.Count == 0 ? 0 :
                Math.Round(row.BackBoneSteps.Where(x => x.BackBoneStepType == BackBoneStepType.Add).Sum(x => x.Percentage),2),


            };
        }

    }
    public static class ValidateMaterialsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.ValidateSAPName, async (ValidateMaterialNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Material, bool>> CriteriaId = null!;
                    Func<Material, bool> CriteriaExist = x => Data.Id == null ?
                    x.SAPName.Equals(Data.SapName) : x.Id != Data.Id.Value && x.SAPName.Equals(Data.SapName);
                    string CacheKey = StaticClass.Materials.Cache.GetAll(Data.FocusFactory);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }
    }
    public static class ValidateMaterialsMNumberEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.ValidateMNumber, async (ValidateMaterialMNumberRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Material, bool>> CriteriaId = null!;
                    Func<Material, bool> CriteriaExist = x => Data.Id == null ?
                    x.M_Number.Equals(Data.MNumber) : x.Id != Data.Id.Value && x.M_Number.Equals(Data.MNumber);
                    string CacheKey = StaticClass.Materials.Cache.GetAll(Data.FocusFactory);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }
    }
    public static class ValidateMaterialsCommonNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Materials.EndPoint.ValidateCommonName, async (ValidateMaterialCommonNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Material, bool>> CriteriaId = null!;
                    Func<Material, bool> CriteriaExist = x => Data.Id == null ?
                    x.CommonName.Equals(Data.CommonName) : x.Id != Data.Id.Value && x.CommonName.Equals(Data.CommonName);
                    string CacheKey = StaticClass.Materials.Cache.GetAll(Data.FocusFactory);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }
    }
}