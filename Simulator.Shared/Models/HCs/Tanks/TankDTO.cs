using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.Tanks
{

    public class TankDTO : BaseEquipmentDTO
    {
       
        public override ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.Tank;
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
                if (Capacity != null)
                    Capacity = new Amount(_CapacityValue, _CapacityUnitName);
            }
        }
        public void ChangeCapacity()
        {
            _CapacityValue = Capacity.GetValue(Capacity.Unit);
            _CapacityUnitName = Capacity.UnitName;
        }
        [JsonIgnore]
        public Amount Capacity { get; set; } = new(MassUnits.KiloGram);
        double _MaxLevelValue;
        string _MaxLevelUnitName = MassUnits.KiloGram.Name;
        public double MaxLevelValue
        {
            get => _MaxLevelValue;
            set
            {
                _MaxLevelValue = value;
                if (MaxLevel != null)
                    MaxLevel = new Amount(_MaxLevelValue, _MaxLevelUnitName);
            }
        }
        public string MaxLevelUnitName
        {
            get => _MaxLevelUnitName;
            set
            {
                _MaxLevelUnitName = value;
                if (MaxLevel != null)
                    MaxLevel = new Amount(_MaxLevelValue, _MaxLevelUnitName);
            }
        }
        public void ChangeMaxLevel()
        {
            _MaxLevelValue = MaxLevel.GetValue(MaxLevel.Unit);
            _MaxLevelUnitName = MaxLevel.UnitName;
        }
        [JsonIgnore]
        public Amount MaxLevel { get; set; } = new(MassUnits.KiloGram);
        double _LoLoLevelValue;
        string _LoLoLevelUnitName = MassUnits.KiloGram.Name;
        public double LoLoLevelValue
        {
            get => _LoLoLevelValue;
            set
            {
                _LoLoLevelValue = value;
                if (LoLoLevel != null)
                    LoLoLevel = new Amount(_LoLoLevelValue, _LoLoLevelUnitName);
            }
        }
        public string LoLoLevelUnitName
        {
            get => _LoLoLevelUnitName;
            set
            {
                _LoLoLevelUnitName = value;
                if (LoLoLevel != null)
                    LoLoLevel = new Amount(_LoLoLevelValue, _LoLoLevelUnitName);
            }
        }
        public void ChangeLoLoLevel()
        {
            _LoLoLevelValue = LoLoLevel.GetValue(LoLoLevel.Unit);
            _LoLoLevelUnitName = LoLoLevel.UnitName;
        }
        [JsonIgnore]

        public Amount LoLoLevel { get; set; } = new(MassUnits.KiloGram);
        double _MinLevelValue;
        string _MinLevelUnitName = MassUnits.KiloGram.Name;
        public double MinLevelValue
        {
            get => _MinLevelValue;
            set
            {
                _MinLevelValue = value;
                if (MinLevel != null)
                    MinLevel = new Amount(_MinLevelValue, _MinLevelUnitName);
            }
        }
        public string MinLevelUnitName
        {
            get => _MinLevelUnitName;
            set
            {
                _MinLevelUnitName = value;
                if (MinLevel != null)
                    MinLevel = new Amount(_MinLevelValue, _MinLevelUnitName);
            }
        }
        public void ChangeMinLevel()
        {
            _MinLevelValue = MinLevel.GetValue(MinLevel.Unit);
            _MinLevelUnitName = MinLevel.UnitName;
        }
        [JsonIgnore]
        public Amount MinLevel { get; set; } = new(MassUnits.KiloGram);
        double _InitialLevelValue;
        string _InitialLevelUnitName = MassUnits.KiloGram.Name;
        public double InitialLevelValue
        {
            get => _InitialLevelValue;
            set
            {
                _InitialLevelValue = value;
                if (InitialLevel != null)
                    InitialLevel = new Amount(_InitialLevelValue, _InitialLevelUnitName);
            }
        }
        public string InitialLevelUnitName
        {
            get => _InitialLevelUnitName;
            set
            {
                _InitialLevelUnitName = value;
                if (InitialLevel != null)
                    InitialLevel = new Amount(_InitialLevelValue, _InitialLevelUnitName);
            }
        }
        public void ChangeInitialLevel()
        {
            _InitialLevelValue = InitialLevel.GetValue(InitialLevel.Unit);
            _InitialLevelUnitName = InitialLevel.UnitName;
        }
        [JsonIgnore]
        public Amount InitialLevel { get; set; } = new(MassUnits.KiloGram);
        public virtual FluidToStorage FluidStorage { get; set; } = FluidToStorage.None;
        public MaterialType MaterialType { get; set; } = MaterialType.None;
        public virtual TankCalculationType TankCalculationType { get; set; } = TankCalculationType.None;
        public void ChangeFluidToStorage()
        {
            switch (FluidStorage)
            {
                case FluidToStorage.RawMaterial:
                    MaterialType = MaterialType.RawMaterial;
                    break;
                case FluidToStorage.RawMaterialBackBone:
                    MaterialType = MaterialType.RawMaterialBackBone;
                    break;
                case FluidToStorage.ProductBackBone:
               
                    MaterialType = MaterialType.ProductBackBone;
                    break;
            }
        }

        public virtual bool IsStorageForOneFluid { get; set; } = false;



    }
    //public class DeleteTankRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Tanks.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.Tanks.EndPoint.Delete;
    //}
    //public class GetTankByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Tanks.EndPoint.GetById;
    //    public override string ClassName => StaticClass.Tanks.ClassName;
    //}
    //public class TankGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Tanks.EndPoint.GetAll;
    //    public Guid MainProcessId { get; set; }

    //}
    //public class TankResponseList : IResponseAll
    //{
    //    public List<TankDTO> Items { get; set; } = new();
    //}
    //public class ValidateTankNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Tanks.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Tanks.ClassName;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class DeleteGroupTankRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of Tank";

    //    public override string ClassName => StaticClass.Tanks.ClassName;

    //    public HashSet<TankDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.Tanks.EndPoint.DeleteGroup;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class ChangeTankOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.Tanks.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Tanks.ClassName;
    //}
    //public class ChangeTankOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.Tanks.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Tanks.ClassName;
    //}
    //public static class TankMapper
    //{
    //    public static ChangeTankOrderDowmRequest ToDown(this TankDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeTankOrderUpRequest ToUp(this TankDTO response)
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
