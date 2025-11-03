using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.ContinuousSystems
{
    public class ContinuousSystemDTO: BaseEquipmentDTO
    {
        
        public override ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.ContinuousSystem;
        double _FlowValue;
        string _FlowUnitName = MassFlowUnits.Kg_hr.Name;
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
        public Amount Flow { get; set; } = new(MassFlowUnits.Kg_hr);
      
       

    }
    //public class DeleteContinuousSystemRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.ContinuousSystems.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.Delete;
    //}
    //public class GetContinuousSystemByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.GetById;
    //    public override string ClassName => StaticClass.ContinuousSystems.ClassName;
    //}
    //public class ContinuousSystemGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.GetAll;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class ContinuousSystemResponseList : IResponseAll
    //{
    //    public List<ContinuousSystemDTO> Items { get; set; } = new();
    //}
    //public class ValidateContinuousSystemNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.ContinuousSystems.ClassName;
    //    public Guid MainProcessId {  set; get; }
    //}
    //public class DeleteGroupContinuousSystemRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of ContinuousSystem";

    //    public override string ClassName => StaticClass.ContinuousSystems.ClassName;

    //    public HashSet<ContinuousSystemDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.DeleteGroup;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class ChangeContinuousSystemOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.ContinuousSystems.ClassName;
    //}
    //public class ChangeContinuousSystemOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.ContinuousSystems.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.ContinuousSystems.ClassName;
    //}
    //public static class ContinuousSystemMapper
    //{
    //    public static ChangeContinuousSystemOrderDowmRequest ToDown(this ContinuousSystemDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,
             
    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeContinuousSystemOrderUpRequest ToUp(this ContinuousSystemDTO response)
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
