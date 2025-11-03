using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;

namespace Simulator.Shared.Models.HCs.Operators
{
    public class OperatorDTO : BaseEquipmentDTO
    {
        
        public override ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.Operator;



    }
    //public class DeleteOperatorRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Operators.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.Operators.EndPoint.Delete;
    //}
    //public class GetOperatorByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Operators.EndPoint.GetById;
    //    public override string ClassName => StaticClass.Operators.ClassName;
    //}
    //public class OperatorGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Operators.EndPoint.GetAll;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class OperatorResponseList : IResponseAll
    //{
    //    public List<OperatorDTO> Items { get; set; } = new();
    //}
    //public class ValidateOperatorNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Operators.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Operators.ClassName;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class DeleteGroupOperatorRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of Operator";

    //    public override string ClassName => StaticClass.Operators.ClassName;

    //    public HashSet<OperatorDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.Operators.EndPoint.DeleteGroup;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class ChangeOperatorOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.Operators.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Operators.ClassName;
    //}
    //public class ChangeOperatorOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.Operators.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Operators.ClassName;
    //}
    //public static class OperatorMapper
    //{
    //    public static ChangeOperatorOrderDowmRequest ToDown(this OperatorDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeOperatorOrderUpRequest ToUp(this OperatorDTO response)
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
