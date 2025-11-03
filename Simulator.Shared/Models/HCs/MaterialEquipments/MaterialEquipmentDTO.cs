using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.MaterialEquipments
{
    public class MaterialEquipmentRecord
    {
        public Guid MaterialId { get; set; }
        public Guid EquipmentId { get; set; }
        public double CapacityValue { get; set; }
        public string CapacityUnitName { get; set; } = string.Empty;
        [JsonIgnore]
        public Amount Capacity => new Amount(CapacityValue, CapacityUnitName);

        public string EquipmentName { get; set; } = string.Empty;
        public string MaterialName { get; set; }= string.Empty;
    }
    public class MaterialEquipmentDTO : Dto   
    {
        public const string MaterialEquipmentCombination = "MaterialEquipmentCombination";
        public override string ToString()
        {
            return $"{MaterialId}-{ProccesEquipmentId}";
        }
        
        public Guid MainProcessId { get; set; } = Guid.Empty;
        public Guid MaterialId { get; set; } = Guid.Empty;
        MaterialDTO _Material = null!;

        public MaterialDTO Material
        {
            get => _Material;
            set
            {
                _Material = value;
                if (_Material != null)
                {
                    MaterialId = _Material.Id;
                }
            }
        }
        public bool IsMixer { get; set; } = false;
        public string MaterialM_Number => _Material == null ? string.Empty : _Material!.M_Number;
        public string MaterialSAPName => _Material == null ? string.Empty : _Material!.SAPName;
        public string MaterialCommonName => _Material == null ? string.Empty : _Material!.CommonName;

        public bool IsSkid { get; set; } = false;
        double _CapacityValue;
        string _CapacityUnitName = MassUnits.KiloGram.Name;
        public double CapacityValue
        {
            get => _CapacityValue;
            set
            {
                _CapacityValue = value;
                if (Capacity != null)
                    Capacity = new Amount(_CapacityValue, _CapacityUnitName);
            }
        }
        public string CapacityUnitName
        {
            get => _CapacityUnitName;
            set
            {
                _CapacityUnitName = value;
                if (string.IsNullOrEmpty(_CapacityUnitName))
                {

                }
                else
                {
                    if (Capacity != null)
                        Capacity = new Amount(_CapacityValue, _CapacityUnitName);
                }

            }
        }
        public void ChangeCapacity()
        {
            _CapacityValue = Capacity.GetValue(Capacity.Unit);
            _CapacityUnitName = Capacity.UnitName;
        }
        [JsonIgnore]
        public Amount Capacity { get; set; } = new(MassUnits.KiloGram);
        public Guid ProccesEquipmentId { get; set; }
        public BaseEquipmentDTO? Equipment { get; set; } = null!;

    }
    //public class DeleteMaterialEquipmentRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MaterialEquipments.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.Delete;
    //}
    //public class GetMaterialEquipmentByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.GetById;
    //    public override string ClassName => StaticClass.MaterialEquipments.ClassName;
    //}
    //public class MaterialEquipmentGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.GetAll;
    //    public Guid EquipmentId { get; set; }
    //}
    //public class MaterialEquipmentResponseList : IResponseAll
    //{
    //    public List<MaterialEquipmentDTO> Items { get; set; } = new();
    //}
    //public class ValidateMaterialEquipmentNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MaterialEquipments.ClassName;
    //    public Guid MaterialId { get; set; }
    //    public Guid EquipmentId { get; set; }
    //}
    //public class DeleteGroupMaterialEquipmentRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of MaterialEquipment";

    //    public override string ClassName => StaticClass.MaterialEquipments.ClassName;

    //    public HashSet<MaterialEquipmentDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.DeleteGroup;
    //    public Guid EquipmentId { get; set; }
    //    public Guid MainProcessId { get; set; }
    //}
    //public class ChangeMaterialEquipmentOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MaterialEquipments.ClassName;
    //}
    //public class ChangeMaterialEquipmentOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.MaterialEquipments.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MaterialEquipments.ClassName;
    //}
    //public static class MaterialEquipmentMapper
    //{
    //    public static ChangeMaterialEquipmentOrderDowmRequest ToDown(this MaterialEquipmentDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeMaterialEquipmentOrderUpRequest ToUp(this MaterialEquipmentDTO response)
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
