using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.PreferedMixers;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.LinePlanneds
{
    public class LinePlannedDTO :Dto
    {
        public string Name => LineDTO == null ? string.Empty : LineDTO.Name;
       

        public Guid SimulationPlannedId { get; set; }
        public Guid MainProcesId { get; set; }
        public PackageType PackageType => LineDTO == null ? PackageType.None : LineDTO.PackageType;
        public LineDTO LineDTO { get; set; } = null!;
        public Guid LineId => LineDTO == null ? Guid.Empty : LineDTO.Id;
        public string LineName => LineDTO == null ? string.Empty : LineDTO.Name;
        public List<PlannedSKUDTO> PlannedSKUDTOs { get; set; } = new();
        public List<PreferedMixerDTO> PreferedMixerDTOs { get; set; } = new();
        public ShiftType ShiftType { get; set; }

        //public PlannedSKUDTO LastPlannedSKU(PlannedSKUDTO plannedSKUDTO)
        //{

        //    return PlannedSKUDTOs.Count == 0 ? null! :
        //        plannedSKUDTO.IsExisting == false ? PlannedSKUDTOs.MaxBy(x => x.Order)! :
        //        PlannedSKUDTOs.Any(x => x.Order == plannedSKUDTO.Order - 1) ?
        //         PlannedSKUDTOs.First(x => x.Order == plannedSKUDTO.Order - 1) :
        //         null!;

        //}
        double _WIPLevelValue;
        string _WIPLevelUnitName = MassUnits.KiloGram.Name;
        public double WIPLevelValue
        {
            get => _WIPLevelValue;
            set
            {
                _WIPLevelValue = value;
                if (WIPLevel != null)
                    WIPLevel=new Amount(_WIPLevelValue, _WIPLevelUnitName);
            }
        }
        public string WIPLevelUnitName
        {
            get => _WIPLevelUnitName;
            set
            {
                _WIPLevelUnitName = value;
                if (WIPLevel != null)
                    WIPLevel=new Amount(_WIPLevelValue, _WIPLevelUnitName);
            }
        }
        public void ChangeWIPLevel()
        {
            _WIPLevelValue = WIPLevel.GetValue(WIPLevel.Unit);
            _WIPLevelUnitName = WIPLevel.UnitName;
        }
        [JsonIgnore]
        public Amount WIPLevel { get; set; } = new Amount(MassUnits.KiloGram);

    }
    //public class DeleteLinePlannedRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.LinePlanneds.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.Delete;
    //}
    //public class GetLinePlannedByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.GetById;
    //    public override string ClassName => StaticClass.LinePlanneds.ClassName;
    //}
    //public class LinePlannedGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.GetAll;
    //    public Guid SimulationPlannedId { get; set; }
    //}
    //public class LinePlannedResponseList : IResponseAll
    //{
    //    public List<LinePlannedDTO> Items { get; set; } = new();
    //}
    //public class ValidateLinePlannedNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.LinePlanneds.ClassName;
    //}
    //public class DeleteGroupLinePlannedRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of LinePlanned";

    //    public override string ClassName => StaticClass.LinePlanneds.ClassName;

    //    public HashSet<LinePlannedDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.DeleteGroup;
    //    public Guid SimulationPlannedId { get; set; }
    //}
    //public class ChangeLinePlannedOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.LinePlanneds.ClassName;
    //}
    //public class ChangeLinePlannedOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.LinePlanneds.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.LinePlanneds.ClassName;
    //}
    //public static class LinePlannedMapper
    //{
    //    public static ChangeLinePlannedOrderDowmRequest ToDown(this LinePlannedDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeLinePlannedOrderUpRequest ToUp(this LinePlannedDTO response)
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
