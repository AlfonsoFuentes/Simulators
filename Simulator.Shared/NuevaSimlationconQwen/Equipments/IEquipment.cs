using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;
using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;
using Simulator.Shared.NuevaSimlationconQwen.States.PlannedDownTimes;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments
{

    public interface ISetMaterialsAtOutlet
    {
        void SetMaterialsAtOutlet(IMaterial material);
    }

    public interface IEquipment
    {
        List<WashoutTime> WashoutTimes { get; set; }
        List<PlannedDownTime> PlannedDownTimes { get; set; }
        int TopologicalLevel { get; set; }
        Guid Id { get; set; }
        string Name { get; set; }
        ProccesEquipmentType EquipmentType { get; set; }
        IEquipmentState? InletState { get; set; }
        IEquipmentState? OutletState { get; set; }
        List<IEquipment> InletEquipments { get; set; }
        List<IEquipment> OutletEquipments { get; set; }
        void Init(DateTime currentdate);
        void CheckPlannedDownTimeStatus(DateTime currentdate);
        void Calculate(DateTime currentdate);
        void Report(DateTime currentdate);
        List<IEquipmentMaterial> EquipmentMaterials { get; set; }
        IMaterial? Material { get; }
        string OcupiedByName { get; set; }
        IPlannedDownTimeState? PlannedDownTimeState { get; set; }

        void AddInletEquipment(IEquipment equipment);
        void AddOutletEquipment(IEquipment equipment);
        void AddMaterial(IMaterial material);
        void GetReleaseFromManager(IManufactureFeeder feeder);

        bool IsEquipmentInPlannedDownTimeState();
        void StartCriticalReport(IEquipment source, string reason, string description);
        void EndCriticalReport();

        ProcessFeederManager ProcessFeederManager { get; set; }
        CriticalDowntimeReportManager ReportManager { get; set; }
        Guid? ActiveDowntimeReportId { get; set; }

    }
    public abstract class Equipment : IEquipment
    {
        public Amount ZeroFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public Amount OneSecond { get; set; } = new Amount(1, TimeUnits.Second);
        public Amount ZeroTime { get; set; } = new Amount(0, TimeUnits.Second);
        public Amount ZeroMass { get; set; } = new Amount(0, MassUnits.KiloGram);
        public IEquipmentState? InletState { get; set; } = null!;
        public IEquipmentState? OutletState { get; set; } = null!;
        public Guid? ActiveDowntimeReportId { get; set; }
        public CriticalDowntimeReportManager ReportManager { get; set; } = null!;
        public ProcessFeederManager ProcessFeederManager { get; set; } = null!;
        public List<WashoutTime> WashoutTimes { get; set; } = new();
        public List<PlannedDownTime> PlannedDownTimes { get; set; } = new();
        public override string ToString() => $"{Name}";
        public int TopologicalLevel { get; set; } = int.MaxValue;
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProccesEquipmentType EquipmentType { get; set; }

        public List<IEquipment> InletEquipments { get; set; } = new();
        public List<IEquipment> OutletEquipments { get; set; } = new();
        public List<IEquipmentMaterial> EquipmentMaterials { get; set; } = new();
        IMaterial? _Material = null;
        public IMaterial? Material
        {
            get => EquipmentMaterials.Count == 1 ? EquipmentMaterials.First().Material : _Material;
            set
            {
                if (EquipmentMaterials.Count == 0 || EquipmentMaterials.Count > 1)
                {
                    _Material = value;
                }
            }
        }
        public string OcupiedByName { get; set; } = string.Empty;
        public IPlannedDownTimeState? PlannedDownTimeState { get; set; } = null!;


        DateTime CurrentDate { get; set; } = DateTime.Now;
        public void Calculate(DateTime currentdate)
        {
            CurrentDate = currentdate;
            try
            {

                BeforeRun(currentdate);
                OutletState?.Calculate(currentdate);
                InletState?.Calculate(currentdate);
                AfterRun(currentdate);
                Report(currentdate);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }


        }
        public virtual void BeforeRun(DateTime currentdate) { }
        public virtual void AfterRun(DateTime currentdate) { }
        public void Init(DateTime currentdate)
        {
            PlannedDownTimeState = new CheckScheduledPlannedDownTimeState(this);
            OnInit(currentdate);
            ValidateOutletInitialState(currentdate);
            ValidateInletInitialState(currentdate);
        }
        public virtual void OnInit(DateTime currentdate) { }
        public virtual void ValidateInletInitialState(DateTime currentdate) { }
        public virtual void ValidateOutletInitialState(DateTime currentdate) { }
        public void AddInletEquipment(IEquipment equipment)
        {
            if (!InletEquipments.Any(x => x.Id == equipment.Id))
            {
                InletEquipments.Add(equipment);
                equipment.AddOutletEquipment(this); // ← Delega al otro equipo
            }
        }

        public void AddOutletEquipment(IEquipment equipment)
        {
            if (!OutletEquipments.Any(x => x.Id == equipment.Id))
            {
                OutletEquipments.Add(equipment);
                equipment.AddInletEquipment(this); // ← Delega al otro equipo
            }
        }
        public void AddMaterial(IMaterial material)
        {
            if (material == null) return; // o lanzar ArgumentNullException
            if (!EquipmentMaterials.Any(m => m.Material.Id == material.Id))
            {
                EquipmentMaterial equiMaterial = new EquipmentMaterial()
                {
                    Material = material,
                };
                EquipmentMaterials.Add(equiMaterial);
                material.AddEquipment(this);
            }
        }

        public void CheckPlannedDownTimeStatus(DateTime currentdate)
        {
            PlannedDownTimeState?.CheckStatus(currentdate);

        }
        public virtual void Report(DateTime currentdate) { }
        public virtual void TransitionToOutletState(OutletState newState)
        {

            OutletState = newState;


        }
        public virtual void TransitionToInletState(InletState newState)
        {

            InletState = newState;

        }
        public IManufactureFeeder Feeder { get; set; } = null!;
        public virtual void GetReleaseFromManager(IManufactureFeeder feeder)
        {
            Feeder = feeder;

        }
        public void StartCriticalReport(IEquipment source, string reason, string description)
        {
            ReportManager.StartReport(
                      generator: this,
                      source: source,
                      reason: reason,
                      description: description
                  );

        }
        public void EndCriticalReport()
        {
            if (ActiveDowntimeReportId.HasValue)
                ReportManager.EndReport(ActiveDowntimeReportId!.Value);
        }
        public bool CheckStatusForPlannedDowntime()
        {
            return PlannedDownTimeState?.CheckStatus(CurrentDate) ?? false;
        }
        public bool IsEquipmentInPlannedDownTimeState()
        {
            if (PlannedDownTimeState is ScheduledPlannedDownTimeState)
            {
                return true;
            }
            return false;
        }

    }


}
