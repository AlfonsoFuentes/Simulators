using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;

namespace Simulator.Server.EndPoints.HCs.ProccesEquipments
{
    //public static class CreateUpdateProccesEquipmentEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ProccesEquipments.EndPoint.CreateUpdate, async (ProccesEquipmentDTO Data, IRepository Repository) =>
    //            {

    //                HCProccesEquipment? row = null;
    //                if (Data.Id == Guid.Empty)
    //                {
    //                    row = HCProccesEquipment.Create(Data.MainProcessId);


    //                    await Repository.AddAsync(row);
    //                }
    //                else
    //                {
    //                    row = await Repository.GetByIdAsync<HCProccesEquipment>(Data.Id);
    //                    if (row == null) { return Result.Fail(Data.NotFound); }
    //                    await Repository.UpdateAsync(row);
    //                }


    //                Data.Map(row);
    //                List<string> cache = [.. StaticClass.ProccesEquipments.Cache.Key(row.Id, row.MainProcessId), .. StaticClass.MainProcesss.Cache.Key(row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());

    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);


    //            });


    //        }

    //    }


    //    public static HCProccesEquipment Map(this ProccesEquipmentDTO request, HCProccesEquipment row)
    //    {
    //        row.FlowValue = request.FlowValue;
    //        row.FlowUnit = request.FlowUnitName;
    //        row.IsForWashing = request.IsForWashing;
    //        row.Name = request.Name;



    //        return row;
    //    }

    //}
    //public static class DeleteProccesEquipmentEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ProccesEquipments.EndPoint.Delete, async (DeleteProccesEquipmentRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<HCProccesEquipment>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.ProccesEquipments.Cache.Key(row.Id, row.MainProcessId), .. StaticClass.MainProcesss.Cache.Key(row.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupProccesEquipmentEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.ProccesEquipments.EndPoint.DeleteGroup, async (DeleteGroupProccesEquipmentRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<HCProccesEquipment>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                List<string> cache = [StaticClass.ProccesEquipments.Cache.GetAll(Data.MainProcessId), .. StaticClass.MainProcesss.Cache.Key(Data.MainProcessId)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    public static class GetAllProccesEquipmentEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.BaseEquipments.EndPoint.GetAll, async (BaseEquipmentGetAll request, IQueryRepository Repository) =>
                {

                    
                    Expression<Func<BaseEquipment, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
                    string CacheKey = StaticClass.BaseEquipments.Cache.GetAll(request.MainProcessId);
                    var rows = await Repository.GetAllAsync<BaseEquipment>(Cache: CacheKey, Criteria: Criteria);
                    if (request.ProccesEquipmentType != Shared.Enums.HCEnums.Enums.ProccesEquipmentType.None)
                    {
                        rows = rows.Where(x => x.ProccesEquipmentType == request.ProccesEquipmentType).ToList();
                    }
                    if (rows == null)
                    {
                        return Result<BaseEquipmentList>.Fail(
                        StaticClass.ResponseMessages.ReponseNotFound(StaticClass.BaseEquipments.ClassLegend));
                    }

                    var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


                    BaseEquipmentList response = new BaseEquipmentList()
                    {
                        Items = maps
                    };
                    return Result<BaseEquipmentList>.Success(response);

                });
            }
        }
        public static BaseEquipmentDTO? Map(this BaseEquipment row)
        {
            return new BaseEquipmentDTO()
            {
                Id = row.Id,
                Name = row.Name,
                EquipmentType = row.ProccesEquipmentType,
            };
        }
    }
   
   
}
