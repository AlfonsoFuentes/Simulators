using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{
    public interface IFeederStarved
    {
    }
    public interface IFeederInUse
    {
    }
    public abstract class FeederOutletState : OutletState<IManufactureFeeder>
    {
       

        public FeederOutletState(IManufactureFeeder feeder) : base(feeder)
        {
        
        }
    }

    public class FeederAvailableState : FeederOutletState
    {
        public FeederAvailableState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = "Available";
            AddTransition<FeederPlannedDownTimeState>(feeder => feeder.IsEquipmentInPlannedDownTimeState());
            AddTransition<IsFeederStarvedByInletState>(feeder => feeder.IsAnyTankInletStarved());
            AddTransition<FeederIsInUseByAnotherEquipmentState>(feeder => !string.IsNullOrEmpty(feeder.OcupiedByName));
        }
    }

    public class FeederPlannedDownTimeState : FeederOutletState, IFeederStarved
    {
        public FeederPlannedDownTimeState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = "Is Starved by planned downtime";
            AddTransition<FeederAvailableState>(feeder => !feeder.IsEquipmentInPlannedDownTimeState());
        }
    }

    public class IsFeederStarvedByInletState : FeederOutletState, IFeederStarved
    {
        public IsFeederStarvedByInletState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = "Is Starved by Tank Low Level";
            AddTransition<FeederAvailableState>(feeder => !feeder.IsAnyTankInletStarved());
        }
    }

    public class FeederIsInUseByAnotherEquipmentState : FeederOutletState, IFeederInUse
    {
        public FeederIsInUseByAnotherEquipmentState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = $"In Use by {Context.OcupiedByName}";
            AddTransition<FeederAvailableState>(feeder => string.IsNullOrEmpty(feeder.OcupiedByName));
        }

       
    }
}
