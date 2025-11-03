using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.SKUs;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.SKULines
{
    public class SKULineDTO : Dto
    {
        public const string ValidationSKUId = "validation_sku_id";
     
        public Guid SKUId => SKU == null ? Guid.Empty : SKU.Id;
        public SKUDTO? SKU { get; set; } = null!;
        public string SKUCode => SKU == null ? string.Empty : SKU.SkuCode;
        public string SKUName => SKU == null ? string.Empty : SKU.Name;
        public PackageType PackageType => SKU == null ? PackageType.None : SKU.PackageType;
        public ProductCategory ProductCategory => SKU == null ? ProductCategory.None : SKU.ProductCategory;
        public string BackBoneM_Number => SKU == null ? string.Empty : SKU.BackBoneM_Number;
        public string BackBoneCommonName => SKU == null ? string.Empty : SKU.BackBoneCommonName;
        public string Size => SKU == null ? string.Empty : SKU.Size.ToString();
        public string Weigth => SKU == null ? string.Empty : SKU.Weigth.ToString();
        public string EA_Case => SKU == null ? string.Empty : SKU.EA_Case.ToString();

        double EA_case => SKU == null ? 0 : SKU.EA_Case;
        public Guid LineId { get; set; }
        public double Case_Shift { get; set; }
        public double AUPercentage => LineSpeed.Value == 0 ? 0 : Math.Round(Case_Shift * EA_case / (LineSpeed.GetValue(LineVelocityUnits.EA_min) * 8 * 60) * 100, 2);
        double _LineSpeedValue;
        string _LineSpeedUnitName = LineVelocityUnits.EA_min.Name;
        public double LineSpeedValue
        {
            get => _LineSpeedValue;
            set
            {
                _LineSpeedValue = value;
                if (LineSpeed != null)
                    LineSpeed = new Amount(_LineSpeedValue, _LineSpeedUnitName);
            }
        }
        public string LineSpeedUnitName
        {
            get => _LineSpeedUnitName;
            set
            {
                _LineSpeedUnitName = value;
                if (LineSpeed != null)
                    LineSpeed = new Amount(_LineSpeedValue, _LineSpeedUnitName);
            }
        }
        public void ChangeLineSpeed()
        {
            _LineSpeedValue = LineSpeed.GetValue(LineSpeed.Unit);
            _LineSpeedUnitName = LineSpeed.UnitName;
        }
        [JsonIgnore]
        public Amount LineSpeed { get; set; } = new(LineVelocityUnits.EA_min);

    }
    //public class DeleteSKULineRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.SKULines.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.SKULines.EndPoint.Delete;
    //}
    //public class GetSKULineByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.SKULines.EndPoint.GetById;
    //    public override string ClassName => StaticClass.SKULines.ClassName;
    //}
    //public class SKULineGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.SKULines.EndPoint.GetAll;
    //    public Guid LineId { get; set; }
    //}
    //public class SKULineResponseList : IResponseAll
    //{
    //    public List<SKULineDTO> Items { get; set; } = new();
    //}
    //public class ValidateSKULineNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.SKULines.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.SKULines.ClassName;
    //    public Guid LineId { get; set; }
    //    public Guid SKUId { get; set; }
    //}
    //public class DeleteGroupSKULineRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of SKULine";

    //    public override string ClassName => StaticClass.SKULines.ClassName;

    //    public HashSet<SKULineDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.SKULines.EndPoint.DeleteGroup;
    //    public Guid LineId { get; set; }
    //}
    //public class ChangeSKULineOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.SKULines.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.SKULines.ClassName;
    //}
    //public class ChangeSKULineOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.SKULines.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.SKULines.ClassName;
    //}
    //public static class SKULineMapper
    //{
    //    public static ChangeSKULineOrderDowmRequest ToDown(this SKULineDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeSKULineOrderUpRequest ToUp(this SKULineDTO response)
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
