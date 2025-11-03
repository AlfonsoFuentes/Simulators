using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Tanks;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.MixerPlanneds
{
    public class MixerPlannedDTO : Dto
    {
      
        public Guid SimulationPlannedId { get; set; }
        public Guid MainProcesId { get; set; }
        double _CapacityValue;
        string _CapacityUnitName = MassUnits.KiloGram.Name;
        public double CapacityValue
        {
            get => _CapacityValue;
            set
            {
                _CapacityValue = value;
                if (Capacity != null)
                    Capacity=new Amount(_CapacityValue, _CapacityUnitName);
            }
        }
        public string CapacityUnitName
        {
            get => _CapacityUnitName;
            set
            {
                _CapacityUnitName = value;
                if (Capacity != null)
                    Capacity=new Amount(_CapacityValue, _CapacityUnitName);
            }
        }
        public void ChangeCapacity()
        {
            _CapacityValue = Capacity.GetValue(Capacity.Unit);
            _CapacityUnitName = Capacity.UnitName;
        }
        [JsonIgnore]
        public Amount Capacity { get; set; } = new(MassUnits.KiloGram);

        public MixerDTO? MixerDTO { get; set; } = null!;




        public Guid MixerId => MixerDTO == null ? Guid.Empty : MixerDTO.Id;
        public string ProducingToName => ProducingTo == null ? string.Empty : ProducingTo.Name;
        public string MixerName => MixerDTO == null ? string.Empty : MixerDTO.Name;
        public string BackBoneName => BackBone == null ? string.Empty : BackBone.CommonName;
        public MaterialDTO BackBone { get; set; } = null!;

        BackBoneStepDTO _BackBoneStep = null!;
        public BackBoneStepDTO BackBoneStep
        {
            get
            {
                return _BackBoneStep;
            }
            set
            {
                _BackBoneStep = value;
                

            }
        }

        public CurrentMixerState CurrentMixerState { get; set; } = CurrentMixerState.None;
        double _MixerLevelValue;
        string _MixerLevelUnitName = MassUnits.KiloGram.Name;
        public double MixerLevelValue
        {
            get => _MixerLevelValue;
            set
            {
                _MixerLevelValue = value;
                if (MixerLevel != null)
                    MixerLevel=new Amount(_MixerLevelValue, _MixerLevelUnitName);
            }
        }
        public string MixerLevelUnitName
        {
            get => _MixerLevelUnitName;
            set
            {
                _MixerLevelUnitName = value;
                if (MixerLevel != null)
                    MixerLevel=new Amount(_MixerLevelValue, _MixerLevelUnitName);
            }
        }
        public void ChangeMixerLevel()
        {
            _MixerLevelValue = MixerLevel.GetValue(MixerLevel.Unit);
            _MixerLevelUnitName = MixerLevel.UnitName;
        }
        [JsonIgnore]
        public Amount MixerLevel { get; set; } = new(MassUnits.KiloGram);



        public List<BackBoneStepDTO> BackBoneSteps { get; set; } = new();

        public BaseEquipmentDTO? ProducingTo { get; set; } = null!;

        public Amount CalculateMixerLevel()
        {
            Amount mixerlevel = new Amount(MassUnits.KiloGram);
            if (BackBoneStep == null) return mixerlevel;
            if (BackBoneSteps.Count == 0) return mixerlevel;
            if (BackBoneStep.Order > BackBoneSteps.Count) return mixerlevel;

            var backbonestepes = BackBoneSteps.Where(x => x.Order <= BackBoneStep.Order && x.BackBoneStepType == BackBoneStepType.Add).ToList();
        
            backbonestepes.ForEach(x => { mixerlevel += x.Percentage / 100 * Capacity; });

            return mixerlevel;
        }
    }
    //public class DeleteMixerPlannedRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MixerPlanneds.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.Delete;
    //    public Guid SimulationPlannedId { get; set; }
    //}
    //public class GetMixerPlannedByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.GetById;
    //    public override string ClassName => StaticClass.MixerPlanneds.ClassName;
    //}
    //public class MixerPlannedGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.GetAll;
    //    public Guid SimulationPlannedId { get; set; }
    //}
    //public class MixerPlannedResponseList : IResponseAll
    //{
    //    public List<MixerPlannedDTO> Items { get; set; } = new();
    //}
    //public class ValidateMixerPlannedNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MixerPlanneds.ClassName;
    //}
    //public class DeleteGroupMixerPlannedRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of MixerPlanned";

    //    public override string ClassName => StaticClass.MixerPlanneds.ClassName;

    //    public HashSet<MixerPlannedDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.DeleteGroup;
    //    public Guid SimulationPlannedId { get; set; }
    //}
    //public class ChangeMixerPlannedOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MixerPlanneds.ClassName;
    //}
    //public class ChangeMixerPlannedOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.MixerPlanneds.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.MixerPlanneds.ClassName;
    //}
    //public static class MixerPlannedMapper
    //{
    //    public static ChangeMixerPlannedOrderDowmRequest ToDown(this MixerPlannedDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeMixerPlannedOrderUpRequest ToUp(this MixerPlannedDTO response)
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
