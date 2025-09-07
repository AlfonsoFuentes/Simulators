using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations.Tanks
{
    public abstract class BaseTank : NewBaseEquipment
    {
        public override Guid Id => TankDTO == null ? Guid.Empty : TankDTO.Id;
        public override string Name => TankDTO == null ? string.Empty : TankDTO.Name;
        protected TankDTO TankDTO { get; set; }
        public Amount Capacity => TankDTO.Capacity;


        public virtual bool IsTankMinLevel => CurrentLevel <= TankDTO.MinLevel;
        public virtual bool IsTankLoLevel => CurrentLevel <= TankDTO.LoLoLevel;
        public bool IsTankHiLevel => CurrentLevel >= TankDTO.MaxLevel;
        public BasePump OutletPump => ConnectedOutletEquipments.Count == 1 ? ConnectedOutletEquipments.Select(x => x as BasePump).First()! : null!;
        public FluidToStorage FluidToStorage => TankDTO.FluidStorage;
        public BaseTank(TankDTO tankDTO)
        {

            TankDTO = tankDTO;

            EquipmentType = Enums.HCEnums.Enums.ProccesEquipmentType.Tank;
            CurrentLevel = TankDTO.InitialLevel;

        }


        protected Amount Inlet { get; set; } = new(MassUnits.KiloGram);
        protected Amount Outlet { get; set; } = new(MassUnits.KiloGram);
        public Amount CurrentLevel { get; set; } = new(MassUnits.KiloGram);
        public Amount TotalMassOutletShift { get; set; } = new(MassUnits.KiloGram);
        protected Amount TotalMassOutlet { get; set; } = new(MassUnits.KiloGram);
        protected Amount CurrentTime { get; set; } = new(0, TimeUnits.Second);
        public Amount AverageOutlet { get; set; } = new(MassFlowUnits.Kg_hr);

        public string AverageOutletString => AverageOutlet.ToString("G6");

        protected Action CalculateForOutlet { get; set; } = null!;
        protected Action CalculateForInlet { get; set; } = null!;


        public NewBaseEquipment InletOcupiedBy { get; set; } = null!;

        protected virtual NewBaseEquipment SearchInlet()
        {
            return null!;
        }



        public void SetInletOcupiedBy(NewBaseEquipment equipment) => InletOcupiedBy = equipment;
        public void DeSetInletOcupiedBy(NewBaseEquipment equipment)
        {
            if (InletOcupiedBy == equipment)
                InletOcupiedBy = null!;
        }
        public void SetInletFlow(Amount flow)
        {
            Inlet = flow * OneSecond;

        }
        public void SetOutletFlow(Amount flow)
        {
            Outlet = flow * OneSecond;

        }

        protected DateTime CurrentDate { get; set; }

        public override void Calculate(DateTime currentdate)
        {
            CurrentDate = currentdate;
            CurrentTime+= OneSecond;
            CalculateForOutlet();

            CalculateForInlet();


        }


        public abstract bool GetTankAvailableforTransferMixer(BaseMixer mixer);

    }


}
