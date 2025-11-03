using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.ProccesEquipments;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;

namespace Simulator.Server.EndPoints.HCs.Conectors
{
    //public static class CreateInletConnectors
    //{
    //    public static async Task Create(this List<InletConnectorDTO> InletConnectors, Guid ToId, IRepository Repository, List<string> cache)
    //    {
    //        foreach (var item in InletConnectors)
    //        {
    //            var rowinlet = Conector.CreateInlet(ToId, item.MainProcessId);

    //            item.MapInlet(rowinlet);
    //            await Repository.AddAsync(rowinlet);
    //            cache.AddRange(StaticClass.Conectors.Cache.KeyInlets(rowinlet.Id, rowinlet.ToId, item.MainProcessId));
    //        }
    //    }
    //}
    //public static class CreateOutletConnectors
    //{
    //    public static async Task Create(this List<OutletConnectorDTO> OutletConnectors, Guid FromId, IRepository Repository, List<string> cache)
    //    {
    //        foreach (var item in OutletConnectors)
    //        {
    //            var rowoutlet = Conector.CreateOutlet(FromId, item.MainProcessId);

    //            item.MapOutlet(rowoutlet);
    //            await Repository.AddAsync(rowoutlet);
    //            cache.AddRange(StaticClass.Conectors.Cache.KeyOutlets(rowoutlet.Id, rowoutlet.FromId, item.MainProcessId));
    //        }
    //    }
    //}
    //public static class CreateUpdateInletConectorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.CreateUpdateInletConnectors, async (InletConnectorDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                Conector? row = null;
    //                Expression<Func<Conector, bool>> Criteria = x => x.ToId == Data.ToId;
    //                var existingrows = await Repository.GetAllAsync<Conector>(Criteria: Criteria);
    //                if (Data.Id == Guid.Empty)
    //                {
                        
    //                    foreach (var item in Data.Froms)
    //                    {
    //                        if (existingrows.Any(x => x.FromId == item!.Id))
    //                        {
    //                            continue;
    //                        }
    //                        row = Conector.CreateInlet(Data.ToId, Data.MainProcessId);
    //                        row.FromId = item!.Id;
    //                        await Repository.AddAsync(row);
    //                        cache.AddRange(StaticClass.Conectors.Cache.KeyInlets(row.Id, row.ToId, Data.MainProcessId));
    //                        await Repository.AddAsync(row);
    //                    }




    //                }
    //                else
    //                {
    //                    foreach (var rowconnector in existingrows)
    //                    {
    //                        if (!Data.Froms.Any(x => x!.Id == rowconnector.FromId))
    //                        {
    //                            await Repository.RemoveAsync(rowconnector);
    //                            cache.AddRange(StaticClass.Conectors.Cache.KeyInlets(rowconnector.Id, rowconnector.ToId, Data.MainProcessId));
    //                        }

    //                    }
    //                    foreach (var item in Data.Froms)
    //                    {
    //                        if (!existingrows.Any(x => x.FromId == item!.Id))
    //                        {
    //                            row = Conector.CreateInlet(Data.ToId, Data.MainProcessId);
    //                            row.FromId = item!.Id;
    //                            await Repository.AddAsync(row);
    //                            cache.AddRange(StaticClass.Conectors.Cache.KeyInlets(row.Id, row.ToId, Data.MainProcessId));
    //                        }

    //                    }
                     
    //                }




    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static Conector MapInlet(this ConectorDTO request, Conector row)
    //    {
    //        row.FromId = request.FromId;



    //        return row;
    //    }

    //}
    //public static class CreateUpdateOutletConectorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.CreateUpdateOuletConnectors, async (OutletConnectorDTO Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                Expression<Func<Conector, bool>> Criteria = x => x.FromId == Data.FromId;
    //                var existingrows = await Repository.GetAllAsync<Conector>(Criteria: Criteria);
    //                Conector? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
                       
    //                    foreach (var item in Data.Tos)
    //                    {
    //                        if (existingrows.Any(x => x.ToId == item!.Id))
    //                        {
    //                            continue;
    //                        }
    //                        row = Conector.CreateOutlet(Data.FromId, Data.MainProcessId);
    //                        row.ToId = item!.Id;
    //                        await Repository.AddAsync(row);
    //                        cache.AddRange(StaticClass.Conectors.Cache.KeyOutlets(row.Id, row.FromId, Data.MainProcessId));
    //                    }




    //                }
    //                else
    //                {
    //                    foreach(var rowconnector in existingrows)
    //                    {
    //                        if(!Data.Tos.Any(x => x!.Id == rowconnector.ToId))
    //                        {
    //                            await Repository.RemoveAsync(rowconnector);
    //                            cache.AddRange(StaticClass.Conectors.Cache.KeyOutlets(rowconnector.Id, rowconnector.FromId, Data.MainProcessId));
    //                        }
                            
    //                    }
    //                    foreach (var item in Data.Tos)
    //                    {
    //                        if (!existingrows.Any(x => x.ToId == item!.Id))
    //                        {
    //                            row = Conector.CreateOutlet(Data.FromId, Data.MainProcessId);
    //                            row.ToId = item!.Id;
    //                            await Repository.AddAsync(row);
    //                            cache.AddRange(StaticClass.Conectors.Cache.KeyOutlets(row.Id, row.FromId, Data.MainProcessId));
    //                        }
                                                        
    //                    }
                        
    //                }





    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static Conector MapOutlet(this ConectorDTO request, Conector row)
    //    {
    //        row.ToId = request.ToId;



    //        return row;
    //    }

    //}

    //public static class DeleteConectorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.Delete, async (DeleteConectorRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<Conector>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);
    //                List<string> cache = new();
    //                cache.AddRange(StaticClass.Conectors.Cache.KeyInlets(row.Id, row.ToId, row.MainProcessId));
    //                cache.AddRange(StaticClass.Conectors.Cache.KeyOutlets(row.Id, row.FromId, row.MainProcessId));

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupConectorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.DeleteGroup, async (DeleteGroupConectorRequest Data, IRepository Repository) =>
    //            {
    //                List<string> cache = new();
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<Conector>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                        cache.AddRange(StaticClass.Conectors.Cache.KeyInlets(row.Id, row.ToId, Data.MainProcessId));
    //                        cache.AddRange(StaticClass.Conectors.Cache.KeyOutlets(row.Id, row.FromId, Data.MainProcessId));
    //                    }
    //                }




    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllOutletsConectorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.GetAllOutlets, async (OutletsConnectorGetAll request, IQueryRepository Repository) =>
    //            {

    //                Func<IQueryable<Conector>, IIncludableQueryable<Conector, object>> includes = x => x
    //                .Include(x => x.To);
    //                Expression<Func<Conector, bool>> Criteria = x => x.FromId == request.FromId;
    //                string CacheKey = StaticClass.Conectors.Cache.GetAllOutlets(request.FromId);
    //                var rows = await Repository.GetAllAsync<Conector>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (rows == null)
    //                {
    //                    return Result<OutletConnectorResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Conectors.ClassLegend));
    //                }

    //                var maps = rows.Select(x => x.MapOutlets()).ToList();


    //                OutletConnectorResponseList response = new OutletConnectorResponseList()
    //                {
    //                    Items = maps.OrderBy(x => x.ToName).ToList(),
    //                };
    //                return Result<OutletConnectorResponseList>.Success(response);

    //            });
    //        }
    //    }
    //    public static OutletConnectorDTO MapOutlets(this Conector row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new OutletConnectorDTO()
    //        {
    //            Id = row.Id,
    //            FromId = row.FromId,

    //            To = row.To == null ? null! : row.To.Map(),
    //            Order = row.Order,
    //            MainProcessId = row.MainProcessId,
    //        };
    //    }


    //}
    //public static class GetAllInletsConectorEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.GetAllInlets, async (InletsConnectorGetAll request, IQueryRepository Repository) =>
    //            {

    //                Func<IQueryable<Conector>, IIncludableQueryable<Conector, object>> includes = x => x
    //                .Include(x => x.From);
    //                Expression<Func<Conector, bool>> Criteria = x => x.ToId == request.ToId;
    //                string CacheKey = StaticClass.Conectors.Cache.GetAllInlets(request.ToId);
    //                var rows = await Repository.GetAllAsync<Conector>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (rows == null)
    //                {
    //                    return Result<InletConnectorResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.Conectors.ClassLegend));
    //                }

    //                var maps = rows.Select(x => x.MapInlets()).ToList();


    //                InletConnectorResponseList response = new InletConnectorResponseList()
    //                {
    //                    Items = maps.OrderBy(x => x.FromName).ToList(),
    //                };
    //                return Result<InletConnectorResponseList>.Success(response);

    //            });
    //        }
    //    }
    //    public static InletConnectorDTO MapInlets(this Conector row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        return new InletConnectorDTO()
    //        {
    //            Id = row.Id,
    //            ToId = row.ToId,

    //            From = row.From == null ? null! : row.From.Map(),
    //            Order = row.Order,
    //            MainProcessId = row.MainProcessId,
    //        };
    //    }


    //}
    //public static class GetInletConectorByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.GetInletById, async (GetInletConectorByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                Func<IQueryable<Conector>, IIncludableQueryable<Conector, object>> includes = x => x
    //                 .Include(y => y.From)
    //                 .Include(x => x.To);
    //                Expression<Func<Conector, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.Conectors.Cache.GetById(request.Id);
    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (row == null)
    //                {
    //                    return Result.Fail(request.NotFound);
    //                }

    //                var response = row.MapInlet();
    //                return Result.Success(response);

    //            });
    //        }
    //    }
    //    public static ConectorDTO MapInlet(this Conector row)
    //    {
    //        //Se debe crear relacion to base equipment para mapear estos equipos
    //        ConectorDTO result = new()
    //        {
    //            Id = row.Id,
    //            From = row.From == null ? null! : row.From.Map(),
    //            To = row.To == null ? null! : row.To.Map(),
    //            Froms = row.From == null ? new() : new List<BaseEquipmentDTO?>() { row.From.Map() },
    //            MainProcessId = row.MainProcessId,
    //        };

    //        return result;
    //    }



    //}
    //public static class GetOutletConectorByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.GetOutletById, async (GetOutletConectorByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                Func<IQueryable<Conector>, IIncludableQueryable<Conector, object>> includes = x => x
    //                 .Include(y => y.From)
    //                 .Include(x => x.To);
    //                Expression<Func<Conector, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.Conectors.Cache.GetById(request.Id);
    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (row == null)
    //                {
    //                    return Result.Fail(request.NotFound);
    //                }

    //                var response = row.MapOutlet();
    //                return Result.Success(response);

    //            });
    //        }
    //    }
    //    public static ConectorDTO MapOutlet(this Conector row)
    //    {
    //        ConectorDTO result = new()
    //        {
    //            Id = row.Id,
    //            From = row.From == null ? null! : row.From.Map(),
    //            To = row.To == null ? null! : row.To.Map(),
    //            Tos = row.To == null ? new() : new List<BaseEquipmentDTO?>() { row.To.Map() },
    //            MainProcessId = row.MainProcessId,

    //        };

    //        return result;
    //    }



    //}
    //public static class ValidateInletConectorsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.ValidateInlet, async (ValidateInletConectorNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<Conector, bool>> CriteriaId = null!;
    //                Func<Conector, bool> CriteriaExist = x => Data.Id == null ?
    //                x.FromId == Data.FromId && x.ToId == Data.ToId : x.Id != Data.Id.Value && x.FromId == Data.FromId && x.ToId == Data.ToId;
    //                string CacheKey = StaticClass.Conectors.Cache.GetAllInlets(Data.ToId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }
    //}
    //public static class ValidateOutletConectorsNameEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.Conectors.EndPoint.ValidateOutlet, async (ValidateOutletConectorNameRequest Data, IQueryRepository Repository) =>
    //            {
    //                Expression<Func<Conector, bool>> CriteriaId = null!;
    //                Func<Conector, bool> CriteriaExist = x => Data.Id == null ?
    //                x.FromId == Data.FromId && x.ToId == Data.ToId : x.Id != Data.Id.Value && x.FromId == Data.FromId && x.ToId == Data.ToId;
    //                string CacheKey = StaticClass.Conectors.Cache.GetAllOutlets(Data.FromId);

    //                return await Repository.AnyAsync(Cache: CacheKey, CriteriaExist: CriteriaExist, CriteriaId: CriteriaId);
    //            });


    //        }
    //    }
    //}
}
