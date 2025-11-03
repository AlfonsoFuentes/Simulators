using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.Pumps
{
    public class PumpDTO: BaseEquipmentDTO
    {
        

        public bool IsForWashing {  get; set; }
        public override ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.Pump;
        double _FlowValue;
        string _FlowUnitName = MassFlowUnits.Kg_min.Name;
        public double FlowValue
        {
            get => _FlowValue;
            set
            {
                _FlowValue = value;
                if (Flow != null)
                    Flow=new Amount(_FlowValue, _FlowUnitName);
            }
        }
        public string FlowUnitName
        {
            get => _FlowUnitName;
            set
            {
                _FlowUnitName = value;
                if (Flow != null)
                    Flow=new Amount(_FlowValue, _FlowUnitName);
            }
        }
        public void ChangeFlow()
        {
            _FlowValue = Flow.GetValue(Flow.Unit);
            _FlowUnitName = Flow.UnitName;
        }
        [JsonIgnore]
        public Amount Flow { get; set; } = new(MassFlowUnits.Kg_min);

    }
    //public class DeletePumpRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Pumps.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.Pumps.EndPoint.Delete;
    //}
    //public class GetPumpByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Pumps.EndPoint.GetById;
    //    public override string ClassName => StaticClass.Pumps.ClassName;
    //}
    //public class PumpGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Pumps.EndPoint.GetAll;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class PumpResponseList : IResponseAll
    //{
    //    public List<PumpDTO> Items { get; set; } = new();
    //}
    //public class ValidatePumpNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Pumps.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Pumps.ClassName;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class DeleteGroupPumpRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of Pump";

    //    public override string ClassName => StaticClass.Pumps.ClassName;

    //    public HashSet<PumpDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.Pumps.EndPoint.DeleteGroup;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class ChangePumpOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.Pumps.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Pumps.ClassName;
    //}
    //public class ChangePumpOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.Pumps.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Pumps.ClassName;
    //}
    //public static class PumpMapper
    //{
    //    public static ChangePumpOrderDowmRequest ToDown(this PumpDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,
             
    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangePumpOrderUpRequest ToUp(this PumpDTO response)
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
