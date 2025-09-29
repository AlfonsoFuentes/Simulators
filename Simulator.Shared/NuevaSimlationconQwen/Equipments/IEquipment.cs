﻿using Simulator.Shared.Enums.HCEnums.Enums;
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


        bool IsEquipmentInPlannedDownTimeState();
        void StartCriticalReport(IEquipment source, string reason, string description);
        void EndCriticalReport();
        void OnFeederMayBeAvailable(IManufactureFeeder feeder);

      
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
        //public WashoutPumpManager WashoutPumpManager { get; set; } = null!;
        //public MaterialFeederManager MaterialFeederManager { get; set; } = null!;
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

        public IManufactureFeeder? Feeder { get; set; } = null!;

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
        public bool IsFeederStartved { get; set; } = false;
        public void OnFeederMayBeAvailable(IManufactureFeeder feeder)
        {
            if (feeder.IsAvailableForAssignment())
            {
                EndCriticalReport();
                feeder.OcuppiedBy = Name;
                feeder.OutletState = new FeederIsInUseByAnotherEquipmentState(feeder);
                Feeder = feeder;
                IsFeederStartved = false;
            }
        }
        public bool IsFeederStarvedRealesed()
        {
            if (!IsFeederStartved)
            {

                return true;
            }
            return false;
        }


        /// <summary>
        /// 1. ¿Hay un feeder de materia prima DISPONIBLE para este material en mis entradas?
        /// </summary>
        public virtual bool IsMaterialFeederAvailable(Guid materialId)
        {
            return InletEquipments
                .OfType<ManufactureFeeder>()

                .Any(f => f.EquipmentMaterials.Any(m => m.Material.Id == materialId)
                       && f.IsAvailableForAssignment());
        }


        
        public virtual bool IsWashoutPumpAvailable()
        {

            var washoutpumps = InletEquipments.OfType<ManufactureFeeder>().Where(f => f.IsForWashout).ToList();

            if (washoutpumps.Any())
            {
                var firstpump= washoutpumps.FirstOrDefault(f=>f.IsAvailableForAssignment());
                if (firstpump != null)
                {
                    Feeder = AssignWashoutPump();
                    return true;
                }
                else
                {
                    EnqueueForWashoutPump();
                }

            }
         
            return false;
        }
        /// <summary>
        /// 2. Asigna un feeder de materia prima (debe llamarse SOLO si IsMaterialFeederAvailable es true).
        /// </summary>
        public virtual IManufactureFeeder? AssignMaterialFeeder(Guid materialId)
        {
            var feeder = InletEquipments
                .OfType<ManufactureFeeder>()
                .FirstOrDefault(f => f.EquipmentMaterials.Any(m => m.Material.Id == materialId)
                                  && f.IsAvailableForAssignment());

            if (feeder != null)
            {
                feeder.OcuppiedBy = Name;
                feeder.OutletState = new FeederIsInUseByAnotherEquipmentState(feeder);
                Feeder = feeder;
            }
            return feeder;
        }

        /// <summary>
        /// 2. Asigna una bomba de lavado (debe llamarse SOLO si IsWashoutPumpAvailable es true).
        /// </summary>
        public virtual IManufactureFeeder? AssignWashoutPump()
        {
            var feeder = InletEquipments
                .OfType<ManufactureFeeder>()
                .FirstOrDefault(f => f.IsForWashout && f.IsAvailableForAssignment());

            if (feeder != null)
            {
                feeder.OcuppiedBy = Name;
                feeder.ActualFlow = feeder.Flow;
                feeder.OutletState = new FeederIsInUseByAnotherEquipmentState(feeder);
                Feeder = feeder;
            }
            return feeder;
        }

        /// <summary>
        /// 3. Si NO hay feeder de materia prima disponible, encolarse en el de menor cola.
        /// </summary>
        public virtual void EnqueueForMaterialFeeder(Guid materialId)
        {
            var candidates = InletEquipments
                .OfType<ManufactureFeeder>()
                .Where(f => !f.IsForWashout)
                .Where(f => f.EquipmentMaterials.Any(m => m.Material.Id == materialId))
                .ToList();

            if (candidates.Any())
            {
                IsFeederStartved = true;
                var best = candidates.OrderBy(f => f.GetWaitingQueueLength()).First();
                best.EnqueueWaitingEquipment(this);
            }
        }

        /// <summary>
        /// 3. Si NO hay bomba de lavado disponible, encolarse en la de menor cola.
        /// </summary>
        public virtual void EnqueueForWashoutPump()
        {
            var candidates = InletEquipments
                .OfType<ManufactureFeeder>()
                .Where(f => f.IsForWashout)
                .ToList();

            if (candidates.Any())
            {
                IsFeederStartved = true;
                var best = candidates.OrderBy(f => f.GetWaitingQueueLength()).First();
                best.EnqueueWaitingEquipment(this);
            }
        }
        public virtual void ReleaseFeeder(IManufactureFeeder _feeder)
        {
            if (_feeder != null)
            {
                // 1. Liberar nombre
                _feeder.OcuppiedBy = string.Empty;

                // 2. Cambiar estado a disponible
                _feeder.OutletState = new FeederAvailableState(_feeder);
                _feeder.ActualFlow = ZeroFlow;
                // 3. Notificar al siguiente en la cola
                _feeder.NotifyNextWaitingEquipment();

                // Limpiar referencia local
                _feeder = null!;
            }
        }
        public virtual bool ReleaseWashoutPump()
        {
            if (Feeder != null)
            {
                ReleaseFeeder(Feeder);
                return true;
            }
            return false;
        }


    }


}
