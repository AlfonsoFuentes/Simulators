using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.SKUs;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.PlannedSKUs
{
    public class PlannedSKUDTO: Dto
    {
        public string Name { get; set; } = string.Empty;
       
        public int PlannedCases { get; set; }
        public Guid LineId { get; set; }
        public Guid LinePlannedId {  get; set; }
      
        public SKUDTO? SKU { get; set; } = null!;
        public Guid SKUId => SKU == null ? Guid.Empty : SKU.Id;
        public string SkuName => SKU == null ? "" : SKU.Name;
        public string SkuCode => SKU == null ? "" : SKU.SkuCode;
        public int EA_Case => SKU == null ? 0 : SKU.EA_Case;
        public double PlannedBottles => SKU == null ? 0 : PlannedCases * SKU.EA_Case;
        public double PlannedShifts => Case_Shift == 0 ? 0 : Math.Round(PlannedCases / Case_Shift, 2);
        public Amount PlannedProductMass => SKU == null ? new(MassUnits.KiloGram) : new(PlannedBottles * Weigth_EA.GetValue(MassUnits.KiloGram), MassUnits.KiloGram);
        public double Case_Shift { get; set; }
        public Amount Weigth_EA => SKU == null ? new(MassUnits.KiloGram) : SKU.Weigth;
        public Amount MassFlowSKU => LineSpeed.Value == 0 ? new(MassFlowUnits.Kg_hr) : new(
                    LineSpeed.GetValue(LineVelocityUnits.EA_hr) * Weigth_EA.GetValue(MassUnits.KiloGram),
                    MassFlowUnits.Kg_hr);
        public double PlannedAU => LineSpeed.Value == 0 ? 0 : Math.Round(Case_Shift * EA_Case / (LineSpeed.GetValue(LineVelocityUnits.EA_min) * 8 * 60) * 100, 2);
        public Amount MassFlowSKUAverage => MassFlowSKU * PlannedAU / 100;
        double _LineSpeedValue;
        string _LineSpeedUnitName = LineVelocityUnits.EA_min.Name;
        public double LineSpeedValue
        {
            get => _LineSpeedValue;
            set
            {
                _LineSpeedValue = value;
                if (LineSpeed != null)
                    LineSpeed=new Amount(_LineSpeedValue, _LineSpeedUnitName);
            }
        }
        public string LineSpeedUnitName
        {
            get => _LineSpeedUnitName;
            set
            {
                _LineSpeedUnitName = value;
                if (LineSpeed != null)
                    LineSpeed=new Amount(_LineSpeedValue, _LineSpeedUnitName);
            }
        }
        public void ChangeLineSpeed()
        {
            _LineSpeedValue = LineSpeed.GetValue(LineSpeed.Unit);
            _LineSpeedUnitName = LineSpeed.UnitName;
        }
        [JsonIgnore]
        public Amount LineSpeed { get; set; } = new(LineVelocityUnits.EA_min);

        double _TimeToChangeSKUValue;
        string _TimeToChangeSKUUnitName = TimeUnits.Minute.Name;
        public double TimeToChangeSKUValue
        {
            get => _TimeToChangeSKUValue;
            set
            {
                _TimeToChangeSKUValue = value;
                if (TimeToChangeSKU != null)
                    TimeToChangeSKU=new Amount(_TimeToChangeSKUValue, _TimeToChangeSKUUnitName);
            }
        }
        public string TimeToChangeSKUUnitName
        {
            get => _TimeToChangeSKUUnitName;
            set
            {
                _TimeToChangeSKUUnitName = value;
                if (TimeToChangeSKU != null)
                    TimeToChangeSKU=new Amount(_TimeToChangeSKUValue, _TimeToChangeSKUUnitName);
            }
        }
        public void ChangeTimeToChangeSKU()
        {
            _TimeToChangeSKUValue = TimeToChangeSKU.GetValue(TimeToChangeSKU.Unit);
            _TimeToChangeSKUUnitName = TimeToChangeSKU.UnitName;
        }
        [JsonIgnore]
        public Amount TimeToChangeSKU { get; set; } = new Amount(TimeUnits.Minute);

    }
    //public class DeletePlannedSKURequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PlannedSKUs.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.Delete;
    //}
    //public class GetPlannedSKUByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.GetById;
    //    public override string ClassName => StaticClass.PlannedSKUs.ClassName;
    //}
    //public class PlannedSKUGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.GetAll;
    //    public Guid LinePlannedId {  get; set; }
    //}
    //public class PlannedSKUResponseList : IResponseAll
    //{
    //    public List<PlannedSKUDTO> Items { get; set; } = new();
    //}
    //public class ValidatePlannedSKUNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PlannedSKUs.ClassName;
    //}
    //public class DeleteGroupPlannedSKURequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of PlannedSKU";

    //    public override string ClassName => StaticClass.PlannedSKUs.ClassName;

    //    public HashSet<PlannedSKUDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.DeleteGroup;
    //    public Guid LinePlannedId { get; set; }
    //}
    //public class ChangePlannedSKUOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid LinePlannedId { get; set; }
    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PlannedSKUs.ClassName;
    //}
    //public class ChangePlannedSKUOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid LinePlannedId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.PlannedSKUs.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PlannedSKUs.ClassName;
    //}
    //public static class PlannedSKUMapper
    //{
    //    public static ChangePlannedSKUOrderDowmRequest ToDown(this PlannedSKUDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,
             
    //            Order = response.Order,
    //            LinePlannedId = response.LinePlannedId, 


    //        };
    //    }
    //    public static ChangePlannedSKUOrderUpRequest ToUp(this PlannedSKUDTO response)
    //    {
    //        return new()
    //        {
          
    //            Id = response.Id,
    //            Name = response.Name,
    //            Order = response.Order,
    //            LinePlannedId = response.LinePlannedId,
    //        };
    //    }

    //}
}
