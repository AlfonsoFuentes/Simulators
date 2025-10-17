using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Server.EndPoints.HCs.SKUs
{
    public static class CreateUpdateSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.CreateUpdate, async (SKUDTO Data, IRepository Repository) =>
                {

                    SKU? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = SKU.Create();


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<SKU>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.SKUs.Cache.Key(row.Id, row.FocusFactory)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static SKU Map(this SKUDTO request, SKU row)
        {
            row.SkuCode = request.SkuCode;
            row.SizeValue = request.SizeValue;
            row.SizeUnit = request.SizeUnitName;
            row.WeigthUnit = request.WeigthUnitName;
            row.WeigthValue = request.WeigthValue;
            row.EA_Case = request.EA_Case;
            row.ProductCategory = request.ProductCategory;
            row.MaterialId = request.BackBone!.Id;
            row.PackageType = request.PackageType;
            row.Name = request.Name;
            row.FocusFactory = request.FocusFactory;


            return row;
        }

    }
    public static class DeleteSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.Delete, async (DeleteSKURequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<SKU>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.SKUs.Cache.Key(row.Id, row.FocusFactory)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.DeleteGroup, async (DeleteGroupSKURequest Data, IRepository Repository) =>
                {
                    FocusFactory focusFactory = FocusFactory.None;
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<SKU>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                            focusFactory = row.FocusFactory;
                        }
                    }


                    List<string> cache = [StaticClass.SKUs.Cache.GetAll(focusFactory)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllSKUEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.GetAll, async (SKUGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<SKU>, IIncludableQueryable<SKU, object>> includes = x => x
                    .Include(y => y.Material!);
                    Expression<Func<SKU, bool>> Criteria = request.FocusFactory == Shared.Enums.HCEnums.Enums.FocusFactory.None ? null! :
                    x => x.FocusFactory == request.FocusFactory;
                    string CacheKey = StaticClass.SKUs.Cache.GetAll(request.FocusFactory);
                    var rows = await Repository.GetAllAsync(Cache: CacheKey, Includes: includes);

                    if (rows == null)
                    {
                        return Result<SKUResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.SKUs.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    SKUResponseList response = new SKUResponseList()
                    {
                        Items = maps
                    };
                    return Result<SKUResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetSKUByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.GetById, async (GetSKUByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<SKU>, IIncludableQueryable<SKU, object>> includes = x => x
                    .Include(y => y.Material!);
                    Expression<Func<SKU, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.SKUs.Cache.GetById(request.Id);
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

        public static SKUDTO Map(this SKU row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new SKUDTO()
            {
                Id = row.Id,

                Name = row.Name,
                EA_Case = row.EA_Case,
                ProductCategory = row.ProductCategory,
                BackBone = row.Material == null ? null! : row.Material.MapMaterial(),
                PackageType = row.PackageType,
                SizeValue = row.SizeValue,
                SizeUnitName = row.SizeUnit,
                WeigthValue = row.WeigthValue,
                WeigthUnitName = row.WeigthUnit,

                SkuCode = row.SkuCode,
                FocusFactory = row.FocusFactory,
                Order = row.Order,
            };
        }

    }
    public static class ValidateSKUsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.Validate, async (ValidateSKUNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<SKU, bool>> CriteriaId = null!;
                    Func<SKU, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.SKUs.Cache.GetAll(Data.FocusFactory);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
    public static class ValidateSKUsCodeEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.HCSKUs.EndPoint.ValidateSKUCode, async (ValidateSKUCodeRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<SKU, bool>> CriteriaId = null!;
                    Func<SKU, bool> CriteriaExist = x => Data.Id == null ?
                    x.SkuCode.Equals(Data.SkuCode) : x.Id != Data.Id.Value && x.SkuCode.Equals(Data.SkuCode);
                    string CacheKey = StaticClass.SKUs.Cache.GetAll(Data.FocusFactory);

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
