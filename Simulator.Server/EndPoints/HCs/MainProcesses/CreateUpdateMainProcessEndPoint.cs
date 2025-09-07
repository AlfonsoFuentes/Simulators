﻿using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.MainProcesss;

namespace Simulator.Server.EndPoints.HCs.MainProcesss
{
    public static class CopyAndPasteMainProcessEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.CopyAndPaste, async (CopyAndPasteMainProcessDTO Data, IRepository Repository) =>
                {
                    List<string> cache = new();
                    ProcessFlowDiagram? row = ProcessFlowDiagram.Create();
                    await Repository.AddAsync(row);
                    row.Name = Data.NewName;


                    cache.AddRange(StaticClass.MainProcesss.Cache.Key(row.Id));

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


    }
    public static class CreateUpdateMainProcessEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.CreateUpdate, async (MainProcessDTO Data, IRepository Repository) =>
                {

                    ProcessFlowDiagram? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = ProcessFlowDiagram.Create();


                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        row = await Repository.GetByIdAsync<ProcessFlowDiagram>(Data.Id);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.MainProcesss.Cache.Key(row.Id)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }

        }


        public static ProcessFlowDiagram Map(this MainProcessDTO request, ProcessFlowDiagram row)
        {

            row.Name = request.Name;



            return row;
        }

    }
    public static class DeleteMainProcessEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.Delete, async (DeleteMainProcessRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<ProcessFlowDiagram>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.MainProcesss.Cache.Key(row.Id)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupMainProcessEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.DeleteGroup, async (DeleteGroupMainProcessRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<ProcessFlowDiagram>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    List<string> cache = [StaticClass.MainProcesss.Cache.GetAll];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllMainProcessEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.GetAll, async (MainProcessGetAll request, IQueryRepository Repository) =>
                {

                    // Func<IQueryable<HCMainProcess>, IIncludableQueryable<HCMainProcess, object>> includes = x => x
                    //.Include(y => y.!);

                    string CacheKey = StaticClass.MainProcesss.Cache.GetAll;
                    var rows = await Repository.GetAllAsync<ProcessFlowDiagram>(Cache: CacheKey);

                    if (rows == null)
                    {
                        return Result<MainProcessResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.MainProcesss.ClassLegend));
                    }

                    var maps = rows.Select(x => x.Map()).ToList();


                    MainProcessResponseList response = new MainProcessResponseList()
                    {
                        Items = maps
                    };
                    return Result<MainProcessResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetMainProcessByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.GetById, async (GetMainProcessByIdRequest request, IQueryRepository Repository) =>
                {
                    //Func<IQueryable<HCMainProcess>, IIncludableQueryable<HCMainProcess, object>> includes = x => x
                    //.Include(y => y.RawMaterial!);
                    Expression<Func<ProcessFlowDiagram, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.MainProcesss.Cache.GetById(request.Id);
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

        public static MainProcessDTO Map(this ProcessFlowDiagram row)
        {
            //Se debe crear relacion to base equipment para mapear estos equipos
            return new MainProcessDTO()
            {
                Id = row.Id,

                Name = row.Name,

                Order = row.Order,
            };
        }

    }
    public static class ValidateMainProcesssNameEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.MainProcesss.EndPoint.Validate, async (ValidateMainProcessNameRequest Data, IQueryRepository Repository) =>
                {
                    Expression<Func<ProcessFlowDiagram, bool>> CriteriaId = null!;
                    Func<ProcessFlowDiagram, bool> CriteriaExist = x => Data.Id == null ?
                    x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
                    string CacheKey = StaticClass.MainProcesss.Cache.GetAll;

                    return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
                });


            }
        }



    }
}
