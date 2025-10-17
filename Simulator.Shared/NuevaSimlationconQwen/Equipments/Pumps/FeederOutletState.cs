using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{
    
    
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
            AddTransition<FeederStarvedByInletState>(feeder => feeder.IsAnyTankInletStarved());
            AddTransition<FeederIsInUseByAnotherEquipmentState>(feeder =>
            {
                if (feeder.OcuppiedBy != null)
                {
                    return true;
                }
                return false;
            });
        }
    }

    public class FeederPlannedDownTimeState : FeederOutletState
    {
        public FeederPlannedDownTimeState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = "Is Starved by planned downtime";
            AddTransition<FeederRealesStarvedState>(feeder => feeder.IsEquipmentInPlannedDownTimeStateRealesed());
        }
    }

    public class FeederStarvedByInletState : FeederOutletState
    {
        public FeederStarvedByInletState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = "Is Starved by Tank No Available";
            AddTransition<FeederRealesStarvedState>(feeder => feeder.IsAnyTankInletStarvedRealesed());
        }
    }

    public class FeederIsInUseByAnotherEquipmentState : FeederOutletState
    {
        public FeederIsInUseByAnotherEquipmentState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel= "In Use by Another Equipment";
            if (Context.OcuppiedBy != null)
            {
                StateLabel = $"In Use by {Context.OcuppiedBy.Name}";
            }
           
           
            AddTransition<FeederRealesStarvedState>(feeder =>
            {
                if (feeder.OcuppiedBy == null)
                {
                    return true;
                }
                return false;
            });
        }

       
    }
    
    public class FeederRealesStarvedState : FeederOutletState
    {
        public FeederRealesStarvedState(IManufactureFeeder feeder) : base(feeder)
        {
            StateLabel = $"Realesing Starved state";
            AddTransition<FeederAvailableState>(feeder =>
            {
              
                if (feeder.GetWaitingQueueLength() > 0)
                {
                    feeder.NotifyNextWaitingEquipment();

                }
                return true;
            });
        }
    }
}
