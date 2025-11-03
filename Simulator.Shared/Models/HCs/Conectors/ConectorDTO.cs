using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;

namespace Simulator.Shared.Models.HCs.Conectors
{
    public record ConnectorRecord(Guid FromId,Guid ToId);
    public class InletConnectorDTO : ConectorDTO
    {
        public const string ConnectorReview = "InletConnectorReview";

    }
    public class OutletConnectorDTO : ConectorDTO
    {
        public const string ConnectorReview = "OutletConnectorReview";
    }
    public class ConectorDTO : Dto, IValidationRequest
    {
        
        public Guid FromId { get; set; }
        BaseEquipmentDTO? _From = null!;
        public BaseEquipmentDTO? From
        {
            get { return _From; }
            set
            {
                _From = value;
                if (_From != null)
                    FromId = _From.Id;
            }
        }
        BaseEquipmentDTO? _To = null!;
        public Guid ToId { get; set; }
        public BaseEquipmentDTO? To
        {
            get { return _To; }
            set
            {
                _To = value;
                if (_To != null)
                    ToId = _To.Id;
            }
        }

        public string FromName => From == null! ? "" : From.Name;
        public string ToName => To == null! ? "" : To.Name;

        public Guid MainProcessId { get; set; }
        public List<BaseEquipmentDTO?> Froms { get; set; } = new();
        public List<BaseEquipmentDTO?> Tos { get; set; } = new();

    }
    //public class DeleteConectorRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Conectors.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.Conectors.EndPoint.Delete;

    //}
    //public class GetInletConectorByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Conectors.EndPoint.GetInletById;
    //    public override string ClassName => StaticClass.Conectors.ClassName;
    //}
    //public class GetOutletConectorByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Conectors.EndPoint.GetOutletById;
    //    public override string ClassName => StaticClass.Conectors.ClassName;
    //}
    //public class InletsConnectorGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Conectors.EndPoint.GetAllInlets;
    //    public Guid ToId { set; get; }
    //}
    //public class OutletsConnectorGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Conectors.EndPoint.GetAllOutlets;
    //    public Guid FromId { set; get; }
    //}
    //public class InletConnectorResponseList : IResponseAll
    //{
    //    public List<InletConnectorDTO> Items { get; set; } = new();
    //}
    //public class OutletConnectorResponseList : IResponseAll
    //{
    //    public List<OutletConnectorDTO> Items { get; set; } = new();
    //}
    //public class ConectorResponseList : IResponseAll
    //{
    //    public List<ConectorDTO> Items { get; set; } = new();
    //}
    //public class ValidateInletConectorNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Conectors.EndPoint.ValidateInlet;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Conectors.ClassName;
    //    public Guid FromId { get; set; }
    //    public Guid ToId { get; set; }

    //}
    //public class ValidateOutletConectorNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Conectors.EndPoint.ValidateOutlet;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Conectors.ClassName;
    //    public Guid FromId { get; set; }
    //    public Guid ToId { get; set; }

    //}
    //public class DeleteGroupConectorRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of Conector";

    //    public override string ClassName => StaticClass.Conectors.ClassName;

    //    public HashSet<ConectorDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.Conectors.EndPoint.DeleteGroup;
    //    public Guid MainProcessId {  get; set; }

    //}
    //public class ChangeConectorOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.Conectors.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Conectors.ClassName;
    //}
    //public class ChangeConectorOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.Conectors.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Conectors.ClassName;
    //}
    //public static class ConectorMapper
    //{
    //    public static ChangeConectorOrderDowmRequest ToDown(this ConectorDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeConectorOrderUpRequest ToUp(this ConectorDTO response)
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
