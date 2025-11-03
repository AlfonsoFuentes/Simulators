using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.BaseEquipments;

namespace Simulator.Server.EndPoints.HCs.ProccesEquipments
{
    public class BaseEquipmentEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            

            app.MapPost("api/getall/BaseEquipmentDTO", async (BaseEquipmentDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<BaseEquipment>(dto, parentId: $"{dto.MainProcessId}");

                var result = query.Select(x => new BaseEquipmentDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    EquipmentType = x.ProccesEquipmentType,
                    MainProcessId = x.MainProcessId,
                    FocusFactory = x.FocusFactory,
                }  ).ToList();

                return Result.Success(result);
            });


            // GetById
           
        }
    }
    //public static class GetAllProccesEquipmentEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.BaseEquipments.EndPoint.GetAll, async (BaseEquipmentGetAll request, IQueryRepository Repository) =>
    //            {

                    
    //                Expression<Func<BaseEquipment, bool>> Criteria = x => x.MainProcessId == request.MainProcessId;
    //                string CacheKey = StaticClass.BaseEquipments.Cache.GetAll(request.MainProcessId);
    //                var rows = await Repository.GetAllAsync<BaseEquipment>(Cache: CacheKey, Criteria: Criteria);
    //                if (request.ProccesEquipmentType != Shared.Enums.HCEnums.Enums.ProccesEquipmentType.None)
    //                {
    //                    rows = rows.Where(x => x.ProccesEquipmentType == request.ProccesEquipmentType).ToList();
    //                }
    //                if (rows == null)
    //                {
    //                    return Result<BaseEquipmentList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.BaseEquipments.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();


    //                BaseEquipmentList response = new BaseEquipmentList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<BaseEquipmentList>.Success(response);

    //            });
    //        }
    //    }
    //    public static BaseEquipmentDTO? Map(this BaseEquipment row)
    //    {
    //        return new BaseEquipmentDTO()
    //        {
    //            Id = row.Id,
    //            Name = row.Name,
    //            EquipmentType = row.ProccesEquipmentType,
    //        };
    //    }
    //}
   
   
}
