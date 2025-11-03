using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.Lines
{
    public class LineDTO : BaseEquipmentDTO
    {
       

        public List<SKULineDTO> LineSKUs { get; set; } = new();
        public List<SKUDTO> SKUs => LineSKUs == null || LineSKUs.Count == 0 ? new() : LineSKUs.Select(x => x.SKU!).ToList();
        double _TimeToReviewAUValue;
        string _TimeToReviewAUUnitName = TimeUnits.Minute.Name;
        public double TimeToReviewAUValue
        {
            get => _TimeToReviewAUValue;
            set
            {
                _TimeToReviewAUValue = value;
                if (TimeToReviewAU != null)
                    TimeToReviewAU=new Amount(_TimeToReviewAUValue, _TimeToReviewAUUnitName);
            }
        }
        public string TimeToReviewAUUnitName
        {
            get => _TimeToReviewAUUnitName;
            set
            {
                _TimeToReviewAUUnitName = value;
                if (TimeToReviewAU != null)
                    TimeToReviewAU=new Amount(_TimeToReviewAUValue, _TimeToReviewAUUnitName);
            }
        }
        public void ChangeTimeToReviewAU()
        {
            _TimeToReviewAUValue = TimeToReviewAU.GetValue(TimeToReviewAU.Unit);
            _TimeToReviewAUUnitName = TimeToReviewAU.UnitName;
        }
        [JsonIgnore]
        public Amount TimeToReviewAU { get; set; } = new(TimeUnits.Minute);

        public PackageType PackageType { get; set; } = PackageType.None;

    }
    //public class DeleteLineRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Lines.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.Lines.EndPoint.Delete;
    //}
    //public class GetLineByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Lines.EndPoint.GetById;
    //    public override string ClassName => StaticClass.Lines.ClassName;
    //}
    //public class LineGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Lines.EndPoint.GetAll;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class LineResponseList : IResponseAll
    //{
    //    public List<LineDTO> Items { get; set; } = new();
    //}
    //public class ValidateLineNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Lines.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Lines.ClassName;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class DeleteGroupLineRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of Line";

    //    public override string ClassName => StaticClass.Lines.ClassName;

    //    public HashSet<LineDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.Lines.EndPoint.DeleteGroup;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class ChangeLineOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.Lines.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Lines.ClassName;
    //}
    //public class ChangeLineOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.Lines.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Lines.ClassName;
    //}
    //public static class LineMapper
    //{
    //    public static ChangeLineOrderDowmRequest ToDown(this LineDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeLineOrderUpRequest ToUp(this LineDTO response)
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
