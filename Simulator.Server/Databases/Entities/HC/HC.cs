using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Material : AuditableEntity<Guid>, ITenantCommon
    {

        public MaterialType MaterialType { get; set; }

        public string M_Number { get; set; } = string.Empty;
        public string SAPName { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public bool IsForWashing { get; set; } = false;
        public MaterialPhysicState PhysicalState { get; set; } = MaterialPhysicState.None;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public static Material Create() =>
            new()
            {

                Id = Guid.NewGuid(),


            };

        public List<BackBoneStep> BackBoneSteps { get; private set; } = new();
        [ForeignKey("RawMaterialId")]
        public List<BackBoneStep> RawMaterials { get; private set; } = new();
        [ForeignKey("MaterialId")]
        public List<SKU> SKUs { get; private set; } = new();
        public BackBoneStep AddBakBoneStep()
        {
            var lastorder = BackBoneSteps.Count == 0 ? 1 : BackBoneSteps.OrderBy(x => x.Order).Last().Order + 1;
            BackBoneStep backBoneStep = new BackBoneStep()
            {
                Id = Guid.NewGuid(),
                MaterialId = Id,
                Order = lastorder,
            };
            BackBoneSteps.Add(backBoneStep);
            return backBoneStep;
        }
        public List<MaterialEquipment> ProcessEquipments { get; set; } = new();
        [ForeignKey("BackBoneId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new List<MixerPlanned>();
    }
    public class BackBoneStep : AuditableEntity<Guid>, ITenantCommon
    {

        public Material HCMaterial { get; set; } = null!;
        public Guid MaterialId { get; set; }


        public Material? RawMaterial { get; set; } = null!;
        public Guid? RawMaterialId { get; set; }

        public BackBoneStepType BackBoneStepType { get; set; } = BackBoneStepType.None;
        public double Percentage { get; set; }
        public double TimeValue { get; set; }
        public string TimeUnitName { get; set; } = string.Empty;

        public static BackBoneStep Create(Guid MaterialId)
        {
            return new BackBoneStep()
            {
                Id = Guid.NewGuid(),
                MaterialId = MaterialId,
            };
        }
        [ForeignKey("BackBoneStepId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new List<MixerPlanned>();

    }
    public class SKU : AuditableEntity<Guid>, ITenantCommon
    {
        public static SKU Create() => new()
        {
            Id = Guid.NewGuid(),


        };

        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public List<SKULine> SKULines { get; set; } = new();

        public string SkuCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public Material Material { get; set; } = null!;
        public Guid MaterialId { get; set; }
        public PackageType PackageType { get; set; }
        public int EA_Case { get; set; }
        public double SizeValue { get; set; }
        public string SizeUnit { get; set; } = string.Empty;
        public double WeigthValue { get; set; }
        public string WeigthUnit { get; set; } = string.Empty;

        [ForeignKey("SKUId")]
        public List<PlannedSKU> PlannedSKUs { get; set; } = new();

    }
    public class SKULine : AuditableEntity<Guid>, ITenantCommon
    {
        public Guid SKUId { get; set; }
        public SKU SKU { get; set; } = null!;

        public Guid LineId { get; set; }
        public Line Line { get; set; } = null!;
        public double LineSpeedValue { get; set; }
        public string LineSpeedUnit { get; set; } = string.Empty;
        public double Case_Shift { get; set; }
        public static SKULine Create(Guid LineId)
        {
            return new SKULine()
            {
                Id = Guid.NewGuid(),
                LineId = LineId,
            };
        }

    }
    public class Line : BaseEquipment
    {
        public List<SKULine> SKULines { get; set; } = new();

        public PackageType PackageType { get; set; } = PackageType.None;

        public static Line Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,


        };
        public double TimeToReviewAUValue { get; set; }
        public string TimeToReviewAUUnit { get; set; } = string.Empty;


        [ForeignKey("LineId")]
        public List<LinePlanned> LinePlanneds { get; set; } = new();
    }
    public class Washout : AuditableEntity<Guid>, ITenantCommon
    {
        public static Washout Create() => new()
        {
            Id = Guid.NewGuid(),


        };

        public ProductCategory ProductCategoryCurrent { get; set; } = ProductCategory.None;
        public ProductCategory ProductCategoryNext { get; set; } = ProductCategory.None;
        public double MixerWashoutTimeValue { get; set; }
        public string MixerWashoutTimeUnit { get; set; } = string.Empty;

        public double LineWashoutTimeValue { get; set; }
        public string LineWashoutTimeUnit { get; set; } = string.Empty;

    }
    public class Conector : AuditableEntity<Guid>, ITenantCommon
    {

        public Guid MainProcessId { get; set; } = Guid.Empty;

        public Guid FromId { get; set; }
        public BaseEquipment From { get; set; } = null!;
        public Guid ToId { get; set; }
        public BaseEquipment To { get; set; } = null!;


        public static Conector CreateInlet(Guid ToId, Guid MainProcessId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ToId = ToId,
                MainProcessId = MainProcessId
            };

        }
        public static Conector CreateOutlet(Guid FromId, Guid MainProcessId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                FromId = FromId,
                MainProcessId = MainProcessId
            };

        }

    }
    public abstract class BaseEquipment : AuditableEntity<Guid>, ITenantCommon
    {
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public ProccesEquipmentType ProccesEquipmentType { get; set; } = ProccesEquipmentType.None;
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; } = string.Empty;

        public ProcessFlowDiagram MainProcess { get; set; } = null!;
        public Guid MainProcessId { get; set; } = Guid.Empty;
        public List<EquipmentPlannedDownTime> PlannedDownTimes { get; private set; } = new();
        public List<MaterialEquipment> Materials { get; set; } = new();

        [ForeignKey("FromId")]
        public List<Conector> Froms { get; set; } = new();
        [ForeignKey("ToId")]
        public List<Conector> Tos { get; set; } = new();
    }
    public class Mixer : BaseEquipment
    {
        public static Mixer Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };


        [ForeignKey("MixerId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new();

        public LinePlanned? LinePlanned { get; set; } = null!;
        public Guid? LinePlannedId { get; set; } = null!;

        [ForeignKey("MixerId")]
        public List<PreferedMixer> PreferedMixers { get; set; } = new();
    }
    public class Operator : BaseEquipment
    {
        public static Operator Create(Guid mainId) => new() { Id = Guid.NewGuid(), MainProcessId = mainId };

    }
    public class Pump : BaseEquipment
    {
        public static Pump Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };
        public bool IsForWashing { get; set; }
        public double FlowValue { get; set; }
        public string FlowUnit { get; set; } = string.Empty;

    }
    public class StreamJoiner : BaseEquipment
    {
        public static StreamJoiner Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };


    }
    public class Tank : BaseEquipment
    {
        public static Tank Create(Guid mainId) => new() { Id = Guid.NewGuid(), MainProcessId = mainId };


        public double CapacityValue { get; set; }
        public string CapacityUnit { get; set; } = string.Empty;
        public double MaxLevelValue { get; set; }
        public string MaxLevelUnit { get; set; } = string.Empty;
        public double MinLevelValue { get; set; }
        public string MinLevelUnit { get; set; } = string.Empty;
        public double LoLoLevelValue { get; set; }
        public string LoLoLevelUnit { get; set; } = string.Empty;
        public double InitialLevelValue { get; set; }
        public string InitialLevelUnit { get; set; } = string.Empty;

        public FluidToStorage FluidStorage { get; set; } = FluidToStorage.None;

        public bool IsStorageForOneFluid { get; set; } = false;

        public TankCalculationType TankCalculationType { get; set; } = TankCalculationType.None;

        [ForeignKey("ProducingToId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new List<MixerPlanned>();

    }
    public class ContinuousSystem : BaseEquipment
    {
        public static ContinuousSystem Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };
        public double FlowValue { get; set; }
        public string FlowUnit { get; set; } = string.Empty;


    }
    public class MaterialEquipment : AuditableEntity<Guid>, ITenantCommon
    {

        public Guid MainProcessId { get; set; } = Guid.Empty;

        public Guid MaterialId { get; set; }
        public Material Material { get; private set; } = null!;

        public BaseEquipment ProccesEquipment { get; private set; } = null!;
        public Guid ProccesEquipmentId { get; set; }
        public double CapacityValue { get; set; }
        public string CapacityUnit { get; set; } = string.Empty;
        public static MaterialEquipment Create(Guid EquipmentId, Guid MainProcessId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ProccesEquipmentId = EquipmentId,
                MainProcessId = MainProcessId,
            };
        }
        public bool IsMixer { get; set; } = false;
    }
    public class EquipmentPlannedDownTime : AuditableEntity<Guid>, ITenantCommon
    {


        public string Name { get; set; } = string.Empty;
        public static EquipmentPlannedDownTime Create(Guid _BaseEquipmentId) =>
            new()
            {
                Id = Guid.NewGuid(),
                BaseEquipmentId = _BaseEquipmentId
            };

        public Guid BaseEquipmentId { get; set; }
        public BaseEquipment BaseEquipment { get; set; } = null!;

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

    }
    public class ProcessFlowDiagram : AuditableEntity<Guid>, ITenantCommon
    {
        public static ProcessFlowDiagram Create() => new() { Id = Guid.NewGuid() };

        public string Name { get; set; } = string.Empty;
        public List<BaseEquipment> ProccesEquipments { get; set; } = new List<BaseEquipment>();
        public List<SimulationPlanned> SimulationPlanneds { get; set; } = new();
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;

    }


    public class MixerPlanned : AuditableEntity<Guid>, ITenantCommon
    {


        public Guid MixerId { get; set; }
        public Mixer Mixer { get; private set; } = null!;

        public SimulationPlanned SimulationPlanned { get; set; } = null!;
        public Guid SimulationPlannedId { get; set; }
        public CurrentMixerState CurrentMixerState { get; set; } = CurrentMixerState.None;
        public static MixerPlanned Create(Guid SimulationPlannedId)
        {
            var retorno = new MixerPlanned
            {
                Id = Guid.NewGuid(),
                SimulationPlannedId = SimulationPlannedId
            };

            return retorno;
        }
        public double MixerLevelValue { get; set; }
        public string MixerLevelUnit { get; set; } = string.Empty;

        public double MixerCapacityValue { get; set; }
        public string MixerCapacityUnit { get; set; } = string.Empty;

        public Guid? BackBoneStepId { get; set; }
        public BackBoneStep? BackBoneStep { get; set; } = null!;


        public Guid? BackBoneId { get; set; }
        public Material? BackBone { get; set; } = null!;

        public Guid? ProducingToId { get; set; }
        public Tank? ProducingTo { get; set; } = null!;



    }
    public class LinePlanned : AuditableEntity<Guid>, ITenantCommon
    {

        public Guid LineId { get; set; }
        public Line Line { get; set; } = null!;
        public ShiftType ShiftType { get; set; }

        public SimulationPlanned HCSimulationPlanned { get; set; } = null!;
        public Guid SimulationPlannedId { get; set; }

        public static LinePlanned Create(Guid SimulationId)
        {
            LinePlanned retorno = new() { Id = Guid.NewGuid() };
            retorno.SimulationPlannedId = SimulationId;

            return retorno;
        }
        [ForeignKey("LinePlannedId")]
        public List<PlannedSKU> SKUPlanneds { get; private set; } = new List<PlannedSKU>();


        public double WIPLevelValue { get; set; }
        public string WIPLevelUnit { get; set; } = string.Empty;

        [ForeignKey("LinePlannedId")]
        public List<PreferedMixer> PreferedMixers { get; private set; } = new List<PreferedMixer>();

    }
    public class SimulationPlanned : AuditableEntity<Guid>, ITenantCommon
    {

        public string Name { get; set; } = string.Empty;
        public DateTime? InitDate { get; set; }
        public TimeSpan? InitSpam { get; set; }
        public DateTime? EndDate { get; set; }
        public double PlannedHours { get; set; }
        public ProcessFlowDiagram MainProcess { get; set; } = null!;
        public Guid MainProcessId { get; set; }
        public List<LinePlanned> LinePlanneds { get; private set; } = new();

        public List<MixerPlanned> MixerPlanneds { get; private set; } = new();

        public static SimulationPlanned Create(Guid MainProcessId) => new SimulationPlanned()
        {
            Id = Guid.NewGuid(),
            MainProcessId = MainProcessId,
        };
        public bool OperatorHasNotRestrictionToInitBatch { get; set; }
        public double MaxRestrictionTimeValue { get; set; }
        public string MaxRestrictionTimeUnit { get; set; } = string.Empty;
    }
    public class PlannedSKU : AuditableEntity<Guid>, ITenantCommon
    {


        public int PlannedCases { get; set; }

        public Guid SKUId { get; set; }
        public SKU SKU { get; set; } = null!;

        public double TimeToChangeSKUValue { get; set; }
        public string TimeToChangeSKUUnit { get; set; } = string.Empty;

        public double LineSpeedValue { get; set; }
        public string LineSpeedUnit { get; set; } = string.Empty;


        public static PlannedSKU Create(Guid LinePlannedId) =>
            new() { Id = Guid.NewGuid(), LinePlannedId = LinePlannedId };

        public LinePlanned LinePlanned { get; set; } = null!;
        public Guid LinePlannedId { get; set; }
    }
    public class PreferedMixer : AuditableEntity<Guid>, ITenantCommon
    {

        public Guid MixerId { get; set; }
        public Mixer Mixer { get; set; } = null!;

        public static PreferedMixer Create(Guid LinePlannedId) =>
            new() { Id = Guid.NewGuid(), LinePlannedId = LinePlannedId };

        public LinePlanned LinePlanned { get; set; } = null!;
        public Guid LinePlannedId { get; set; }
    }

}

