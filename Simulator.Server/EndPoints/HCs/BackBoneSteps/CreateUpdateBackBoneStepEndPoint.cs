using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Models.HCs.BackBoneSteps;

namespace Simulator.Server.EndPoints.HCs.BackBoneSteps
{
    public static class CreateUpdateBackBoneStepEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.CreateUpdate, async (BackBoneStepDTO Data, IRepository Repository) =>
                {
                    List<string> cacheMaterial = new();
                    var lastorder = await GetLastOrder(Repository, Data.MaterialId);
                    BackBoneStep? row = null;
                    if (Data.Id == Guid.Empty)
                    {
                        row = BackBoneStep.Create(Data.MaterialId);
                        row.Order = lastorder + 1;

                        await Repository.AddAsync(row);
                    }
                    else
                    {
                        Expression<Func<BackBoneStep, bool>> Criteria = x => x.Id == Data.Id;
                        Func<IQueryable<BackBoneStep>, IIncludableQueryable<BackBoneStep, object>> includes = x => x
                   .Include(y => y.HCMaterial);
                        row = await Repository.GetAsync(Criteria:Criteria,Includes:includes);
                        if (row == null) { return Result.Fail(Data.NotFound); }
                        await Repository.UpdateAsync(row);
                        cacheMaterial = [.. StaticClass.Materials.Cache.Key(row.Id, row.HCMaterial.FocusFactory)];
                    }


                    Data.Map(row);
                    List<string> cache = [.. StaticClass.BackBoneSteps.Cache.Key(row.Id, row.MaterialId)];
                    if (cacheMaterial.Count > 0) cache.AddRange(cacheMaterial);
                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);


                });


            }
            async Task<int> GetLastOrder(IRepository Repository, Guid MaterialId)
            {
                Expression<Func<BackBoneStep, bool>> Criteria = x => x.MaterialId == MaterialId;
                var rows = await Repository.GetAllAsync(Criteria: Criteria);

                var lastorder = rows.Count > 0 ? rows.Max(x => x.Order) : 0;
                return lastorder;
            }
        }


        public static BackBoneStep Map(this BackBoneStepDTO request, BackBoneStep row)
        {
            row.BackBoneStepType = request.BackBoneStepType;
            row.RawMaterialId = request.RawMaterialId;
            row.Percentage = request.Percentage;
            row.TimeValue = request.TimeValue;
            row.TimeUnitName = request.TimeUnitName;

            return row;
        }

    }
    public static class DeleteBackBoneStepEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.Delete, async (DeleteBackBoneStepRequest Data, IRepository Repository) =>
                {
                    var row = await Repository.GetByIdAsync<BackBoneStep>(Data.Id);
                    if (row == null) { return Result.Fail(Data.NotFound); }
                    await Repository.RemoveAsync(row);

                    List<string> cache = [.. StaticClass.BackBoneSteps.Cache.Key(row.Id, row.MaterialId)];

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class DeleteGroupBackBoneStepEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.DeleteGroup, async (DeleteGroupBackBoneStepRequest Data, IRepository Repository) =>
                {
                    foreach (var rowItem in Data.SelecteItems)
                    {
                        var row = await Repository.GetByIdAsync<BackBoneStep>(rowItem.Id);
                        if (row != null)
                        {
                            await Repository.RemoveAsync(row);
                        }
                    }


                    var cache = StaticClass.BackBoneSteps.Cache.GetAll(Data.MaterialId);

                    var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache);
                    return Result.EndPointResult(result,
                        Data.Succesfully,
                        Data.Fail);

                });
            }
        }




    }
    public static class GetAllBackBoneStepEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.GetAll, async (BackBoneStepGetAll request, IQueryRepository Repository) =>
                {

                    Func<IQueryable<BackBoneStep>, IIncludableQueryable<BackBoneStep, object>> includes = x => x
                   .Include(y => y.RawMaterial!);
                    string CacheKey = StaticClass.BackBoneSteps.Cache.GetAll(request.MaterialId);
                    Expression<Func<BackBoneStep, bool>> Criteria = x => x.MaterialId == request.MaterialId;
                    var rows = await Repository.GetAllAsync<BackBoneStep>(Cache: CacheKey, Includes: includes, Criteria: Criteria);

                    if (rows == null)
                    {
                        return Result<BackBoneStepResponseList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.BackBoneSteps.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();


                    BackBoneStepResponseList response = new BackBoneStepResponseList()
                    {
                        Items = maps
                    };
                    return Result<BackBoneStepResponseList>.Success(response);

                });
            }
        }
    }
    public static class GetBackBoneStepByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BackBoneSteps.EndPoint.GetById, async (GetBackBoneStepByIdRequest request, IQueryRepository Repository) =>
                {
                    Func<IQueryable<BackBoneStep>, IIncludableQueryable<BackBoneStep, object>> includes = x => x
                    .Include(y => y.RawMaterial!);
                    Expression<Func<BackBoneStep, bool>> Criteria = x => x.Id == request.Id;

                    string CacheKey = StaticClass.BackBoneSteps.Cache.GetById(request.Id);
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

        public static BackBoneStepDTO Map(this BackBoneStep row)
        {
            return new BackBoneStepDTO()
            {
                Id = row.Id,
                TimeValue = row.TimeValue,
                TimeUnitName = row.TimeUnitName,
                Percentage = row.Percentage,
                StepRawMaterial = row.RawMaterial == null ? null! : row.RawMaterial.MapMaterial(),
                MaterialId = row.MaterialId,
                BackBoneStepType = row.BackBoneStepType,
                Order = row.Order,
            };
        }

    }
    //public static class ValidateBackBoneStepsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.BackBoneSteps.EndPoint.Validate, async (ValidateBackBoneNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<HCBackBoneStep, bool>> CriteriaId = null!;
    //                Func<HCBackBoneStep, bool> CriteriaExist = x => Data.Id == null ?
    //                x.SAPName.Equals(Data.Name) : x.Id != Data.Id.Value && x.SAPName.Equals(Data.Name);
    //                string CacheKey = StaticClass.BackBoneSteps.Cache.GetAll;

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
