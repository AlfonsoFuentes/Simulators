using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.PreferedMixers;

namespace Simulator.Server.EndPoints.HCs.PreferedMixers
{
    public class PreferedMixerEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<PreferedMixer>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(PreferedMixer)} table");
                }


                return Result.Success($"{nameof(PreferedMixer)} table Updated");

            });

            app.MapPost("api/getall/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<PreferedMixer>(dto, parentId: $"{dto.LinePlannedId}");

                var result = query.Select(x => x.MapToDto<PreferedMixerDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<PreferedMixer>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<PreferedMixerDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<PreferedMixer>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(PreferedMixer)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(PreferedMixer)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/PreferedMixerDTO", async (List<PreferedMixerDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(PreferedMixer)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<PreferedMixer>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(PreferedMixer)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<PreferedMixer>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("PreferedMixer moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<PreferedMixer>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("PreferedMixer moved down successfully");
            });
            // En PreferedMixerEndpointExtensions
            app.MapPost("api/validate/PreferedMixerDTO", async (PreferedMixerDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<PreferedMixer>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(PreferedMixer)} table, already exists");
            });
        }
    }
    //public static class CreatePreferedMixers
    //{
    //    public static async Task Create(this List<PreferedMixerDTO> SKUsPlanned, Guid Id, IRepository Repository, List<string> cache)
    //    {
    //        foreach (var item in SKUsPlanned)
    //        {
    //            var skuplanned = PreferedMixer.Create(Id);
    //            item.Map(skuplanned);
    //            await Repository.AddAsync(skuplanned);
    //            cache.AddRange(StaticClass.PreferedMixers.Cache.Key(skuplanned.Id, Id));
    //        }
    //    }
    //}
    //public static class CreateUpdatePreferedMixerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PreferedMixers.EndPoint.CreateUpdate, async (PreferedMixerDTO Data, IRepository Repository) =>
    //            {
    //                var lastorder = await GetLastOrder(Repository, Data.LinePlannedId);
    //                PreferedMixer? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = PreferedMixer.Create(Data.LinePlannedId);
    //                    row.Order = lastorder + 1;
    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<PreferedMixer>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                List<string> cache = [.. StaticClass.PreferedMixers.Cache.Key(row.Id, row.LinePlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }
    //        async Task<int> GetLastOrder(IRepository Repository, Guid LinePlannedId)
    //        {
    //            Expression<Func<PreferedMixer, bool>> Criteria = x => x.LinePlannedId == LinePlannedId;
    //            var rows = await Repository.GetAllAsync(Criteria: Criteria);

    //            var lastorder = rows.Count > 0 ? rows.Max(x => x.Order) : 0;
    //            return lastorder;
    //        }
    //    }


    //    public static PreferedMixer Map(this PreferedMixerDTO request, PreferedMixer row)
    //    {
    //        row.MixerId = request.MixerId;

    //        return row;
    //    }

    //}
    //public static class DeletePreferedMixerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PreferedMixers.EndPoint.Delete, async (DeletePreferedMixerRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<PreferedMixer>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.PreferedMixers.Cache.Key(row.Id, row.LinePlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupPreferedMixerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PreferedMixers.EndPoint.DeleteGroup, async (DeleteGroupPreferedMixerRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<PreferedMixer>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.PreferedMixers.Cache.GetAll(Data.LinePlannedId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllPreferedMixerEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PreferedMixers.EndPoint.GetAll, async (PreferedMixerGetAll request, IQueryRepository Repository) =>
    //            {

    //                Func<IQueryable<PreferedMixer>, IIncludableQueryable<PreferedMixer, object>> includes = x => x
    //                 .Include(x => x.Mixer)
    //               .Include(x => x.LinePlanned)
    //               ;
    //                Expression<Func<PreferedMixer, bool>> Criteria = x => x.LinePlannedId == request.LinePlannedId;
    //                string CacheKey = StaticClass.PreferedMixers.Cache.GetAll(request.LinePlannedId);
    //                var rows = await Repository.GetAllAsync<PreferedMixer>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (rows == null)
    //                {
    //                    return Result<PreferedMixerResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.PreferedMixers.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();


    //                PreferedMixerResponseList response = new PreferedMixerResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<PreferedMixerResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetPreferedMixerByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PreferedMixers.EndPoint.GetById, async (GetPreferedMixerByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                Func<IQueryable<PreferedMixer>, IIncludableQueryable<PreferedMixer, object>> includes = x => x
    //                .Include(x => x.LinePlanned)
    //                .Include(x => x.Mixer)

    //              ;
    //                Expression<Func<PreferedMixer, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.PreferedMixers.Cache.GetById(request.Id);
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

    //    public static PreferedMixerDTO Map(this PreferedMixer row)
    //    {


    //        PreferedMixerDTO result = new PreferedMixerDTO()
    //        {
    //            Id = row.Id,

    //            Mixer = row.Mixer == null ? null! : row.Mixer.Map(),
    //            LinePlannedId = row.LinePlannedId,

    //        };

    //        return result;
    //    }

    //}
    //public static class ValidatePreferedMixersNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.PreferedMixers.EndPoint.Validate, async (ValidatePreferedMixerNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<HCPreferedMixer, bool>> CriteriaId = null!;
    //                Func<HCPreferedMixer, bool> CriteriaExist = x => Data.Id == null ?
    //                x.Name.Equals(Data.Name) : x.Id != Data.Id.Value && x.Name.Equals(Data.Name);
    //                string CacheKey = StaticClass.PreferedMixers.Cache.GetAll(Data.MainProcessId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }



    //}
}
