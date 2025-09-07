using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Server.EndPoints.HCs.Washouts
{
    public static class CreateUpdateWashoutEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Washouts.EndPoint.CreateUpdate, async (WashoutDTO Data, IRepository Repository) =>
                {

                    Washout? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = Washout.Create();


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<Washout>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.Washouts.Cache.Key(row.Id)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static Washout Map(this WashoutDTO request, Washout row)
        {
            row.ProductCategoryNext = request.ProductCategoryNext;
            row.ProductCategoryCurrent = request.ProductCategoryCurrent;
            row.LineWashoutTimeUnit = request.LineWashoutUnitName;
            row.LineWashoutTimeValue = request.LineWashoutValue;
            row.MixerWashoutTimeUnit = request.MixerWashoutUnitName;
            row.MixerWashoutTimeValue = request.MixerWashoutValue;



            return row;
        }

    }
    public static class DeleteWashoutEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Washouts.EndPoint.Delete, async (DeleteWashoutRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<Washout>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.Washouts.Cache.Key(row.Id)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupWashoutEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Washouts.EndPoint.DeleteGroup, async (DeleteGroupWashoutRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<Washout>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.Washouts.Cache.GetAll];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllWashoutEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Washouts.EndPoint.GetAll, async (WashoutGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCWashout>, IIncludableQueryable<HCWashout, object>> includes = x => x
                    //.Include(y => y.!);
                    //Expression<Func<HCWashout, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.Washouts.Cache.GetAll;
                    var rows = await Repository.GetAllAsync<Washout>(Cache: CacheKey/*, Criteria: Criteria*/);

                    if (rows == null)
                    {
                        return Result<WashoutResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Washouts.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    WashoutResponseList response = new WashoutResponseList()
                    {
                        Items = maps
                    };
                    return Result<WashoutResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetWashoutByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Washouts.EndPoint.GetById, async (GetWashoutByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCWashout>, IIncludableQueryable<HCWashout, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<Washout, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.Washouts.Cache.GetById(request.Id);
                    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria/*, Includes: includes*/);

                    if (row == null)
                    {
                        return Result.Fail(request.NotFound);
                    }

                    var response = row.Map();
                    return Result.Success(response);

                });
            }
        }

        public static WashoutDTO Map(this Washout row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new WashoutDTO()
            {
                Id = row.Id,
                LineWashoutValue = row.LineWashoutTimeValue,
                LineWashoutUnitName = row.LineWashoutTimeUnit,
                MixerWashoutValue=row.MixerWashoutTimeValue ,
                MixerWashoutUnitName=row.MixerWashoutTimeUnit,
                ProductCategoryCurrent=row.ProductCategoryCurrent,
                ProductCategoryNext=row.ProductCategoryNext,
                 
                Order = row.Order,
            };
        }

    }
    public static class ValidateWashoutsNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.Washouts.EndPoint.Validate, async (ValidateWashoutRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<Washout, bool>> CriteriaId = null!;
                    Func<Washout, bool> CriteriaExist = x => Data.Id == null ?
                    x.ProductCategoryCurrent==Data.Current &&x.ProductCategoryNext==Data.Next : 
                    x.Id != Data.Id.Value && x.ProductCategoryCurrent == Data.Current && x.ProductCategoryNext == Data.Next;
                    string CacheKey = StaticClass.Washouts.Cache.GetAll;

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
