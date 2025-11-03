using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.SimulationPlanneds;

namespace Simulator.Shared.Models.HCs.MainProcesss
{
    //public class CopyAndPasteMainProcessDTO : BaseResponse, IMessageResponse, IRequest
    //{
    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.CopyAndPaste;

    //    public string Legend => Name;
    //    public string NewName { get; set; } = string.Empty;
    //    public string ActionType => $"Copy and Paste {Name}";
    //    public string ClassName => StaticClass.MainProcesss.ClassName;
    //    public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
    //    public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
    //    public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);
    //}
    public class ProcessFlowDiagramDTO : Dto
    {

        public string Name { get; set; } = string.Empty;
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
       
        public List<SimulationPlannedDTO> PlannedSimulations { get; set; } = new();


        public List<BaseEquipmentDTO> EquipmentDTOs { get; set; } = new();

    }
    //public class DeleteMainProcessRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MainProcesss.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.Delete;
    //}
    //public class GetMainProcessByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.GetById;
    //    public override string ClassName => StaticClass.MainProcesss.ClassName;
    //}
    //public class MainProcessGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.GetAll;
    //}
    //public class MainProcessResponseList : IResponseAll
    //{
    //    public List<MainProcessDTO> Items { get; set; } = new();
    //}
    //public class ValidateMainProcessNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MainProcesss.ClassName;
    //}
    //public class DeleteGroupMainProcessRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of MainProcess";

    //    public override string ClassName => StaticClass.MainProcesss.ClassName;

    //    public HashSet<MainProcessDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.DeleteGroup;
    //}
    //public class ChangeMainProcessOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MainProcesss.ClassName;
    //}
    //public class ChangeMainProcessOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.MainProcesss.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MainProcesss.ClassName;
    //}
    //public static class MainProcessMapper
    //{
    //    public static ChangeMainProcessOrderDowmRequest ToDown(this MainProcessDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeMainProcessOrderUpRequest ToUp(this MainProcessDTO response)
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
