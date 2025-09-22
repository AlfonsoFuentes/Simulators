using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{
   
    public abstract class FeederOutletState : OutletState<IManufactureFeeder>
    {
        protected IManufactureFeeder _feeder { get; set; }

        public FeederOutletState(IManufactureFeeder feeder) : base(feeder)
        {
            _feeder = feeder;
        }
    }
    public class FeederAvailableState : FeederOutletState
    {

        public FeederAvailableState(IManufactureFeeder feeder) : base(feeder)
        {

            StateLabel = $"Available";
            AddTransition<FeederPlannedDownTimeState>(feeder => feeder.IsEquipmentInPlannedDownTimeState());
            AddTransition<IsFeederStarvedByInletState>(feeder => feeder.IsAnyTankInletStarved());
            AddTransition<FeederIsInUseByAnotherEquipmentState>(feeder => feeder.IsInUse());
        }

    }
    public class FeederPlannedDownTimeState : FeederOutletState
    {

        public FeederPlannedDownTimeState(IManufactureFeeder feeder) : base(feeder)
        {

            StateLabel = $"Is Starved by planned downtime";
            AddTransition<FeederAvailableState>(feeder => !feeder.IsEquipmentInPlannedDownTimeState());
        }

    }
    
    public class IsFeederStarvedByInletState : FeederOutletState
    {

        public IsFeederStarvedByInletState(ProcessPump feeder) : base(feeder)
        {

            StateLabel = $"is Starved by Tank Lo Level";
            AddTransition<FeederAvailableState>(feeder => !feeder.IsAnyTankInletStarved());
        }

    }
    public class FeederIsInUseByAnotherEquipmentState : FeederOutletState
    {

        public FeederIsInUseByAnotherEquipmentState(IManufactureFeeder feeder) : base(feeder)
        {

            StateLabel = $"In Use by {feeder.OcupiedByName}";
            AddTransition<FeederAvailableState>(feeder => !feeder.IsInUse());
        }

    }
}
