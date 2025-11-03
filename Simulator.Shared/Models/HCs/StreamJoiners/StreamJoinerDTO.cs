using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.StreamJoiners
{
    public class StreamJoinerDTO: BaseEquipmentDTO
    {
       

        public override ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.StreamJoiner;
       

    }
    //public class DeleteStreamJoinerRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.StreamJoiners.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.Delete;
    //}
    //public class GetStreamJoinerByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.GetById;
    //    public override string ClassName => StaticClass.StreamJoiners.ClassName;
    //}
    //public class StreamJoinerGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.GetAll;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class StreamJoinerResponseList : IResponseAll
    //{
    //    public List<StreamJoinerDTO> Items { get; set; } = new();
    //}
    //public class ValidateStreamJoinerNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.StreamJoiners.ClassName;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class DeleteGroupStreamJoinerRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of StreamJoiner";

    //    public override string ClassName => StaticClass.StreamJoiners.ClassName;

    //    public HashSet<StreamJoinerDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.DeleteGroup;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class ChangeStreamJoinerOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.StreamJoiners.ClassName;
    //}
    //public class ChangeStreamJoinerOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.StreamJoiners.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.StreamJoiners.ClassName;
    //}
    //public static class StreamJoinerMapper
    //{
    //    public static ChangeStreamJoinerOrderDowmRequest ToDown(this StreamJoinerDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,
             
    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeStreamJoinerOrderUpRequest ToUp(this StreamJoinerDTO response)
    //    {
    //        return new()
    //        {
          
    //            Id = response.Id,
    //            Name = response.Name,
    //            Order = response.Order,
    //        };
    //    }

    //}
}
