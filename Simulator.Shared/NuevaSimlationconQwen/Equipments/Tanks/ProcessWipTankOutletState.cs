using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public interface IAvailableState
    {

    }
    public abstract class ProcessWipOutletState : OutletState<ProcessWipTankForLine>
    {
        

        public ProcessWipOutletState(ProcessWipTankForLine tank) : base(tank)
        {
    
        }
    }
    public class ProcessWipTankOutletInitializeTankState : ProcessWipOutletState, ITankOuletStarved
    {

        public ProcessWipTankOutletInitializeTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Review if order is received";
            AddTransition<ProcessWipTankOutletReviewInitInletStateTankState>(tank => tank.IsNewOrderReceived());

        }

    }

    public class ProcessWipTankOutletReviewInitInletStateTankState : ProcessWipOutletState, ITankOuletStarved
    {
        public ProcessWipTankOutletReviewInitInletStateTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Review if must wash tank";
            AddTransition<ProcessWipTankOutletReviewWashingTankState>(tank => tank.IsMustWashTank());
            AddTransition<ProcessWipTankOutletAvailableState>();

        }

    }

    public class ProcessWipTankOutletAvailableState : ProcessWipOutletState
    {
        public ProcessWipTankOutletAvailableState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Available to deliver mass to line";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => tank.IsNextMaterialNeeded());
            AddTransition<ProcessWipTankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());
            AddTransition<ProcessWipTankOutletAvailableToEmptyTankState>(tank => tank.IsMassDeliveredCompleted());

            AddTransition<ProcessWipTankOutletNotAvailableState>(tank => tank.IsTankInLoLevel());

        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateOutletLevel();



        }

    }
    public class ProcessWipTankOutletAvailableToEmptyTankState : ProcessWipOutletState
    {

        public ProcessWipTankOutletAvailableToEmptyTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Emptying vessel";
            AddTransition<ProcessWipTankOutletAvailableToEmptyTankState>(tank => tank.IsNextMaterialNeeded());
            AddTransition<ProcessWipTankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());

            AddTransition<ProcessWipReleaseCurrentOrderTankState>(tank => tank.IsTankInLoLevel());

        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateOutletLevel();


        }

    }
    public class ProcessWipReleaseCurrentOrderTankState : ProcessWipOutletState
    {

        public ProcessWipReleaseCurrentOrderTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Realising current order";

            AddTransition<ProcessWipTankOutletInitializeTankState>(tank => tank.IsCurrentOrderRealesed());

        }
      

    }
  
    public class ProcessWipTankOutletNotAvailableState : ProcessWipOutletState, ITankOuletStarved
    {


        public ProcessWipTankOutletNotAvailableState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Low Level Tank";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => !tank.IsTankInLoLevel());
        }


    }
    public class ProcessWipTankOutletPlannedDownTimeState : ProcessWipOutletState, ITankOuletStarved
    {


        public ProcessWipTankOutletPlannedDownTimeState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Planned down time";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => !tank.IsEquipmentInPlannedDownTimeState());
        }


    }
    public class ProcessWipTankOutletReviewWashingTankState : ProcessWipOutletState, ITankOuletStarved
    {



        public ProcessWipTankOutletReviewWashingTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Review any washing pump available";
            AddTransition<ProcessWipTankOutletWashingTankState>(tank => tank.IsWashoutPumpAvailable());
            AddTransition<ProcessWipTankOutletStarvedWashingTankState>(tank => !tank.IsWashoutPumpAvailable());
        }

    }

    public class ProcessWipTankOutletWashingTankState : ProcessWipOutletState, ITankOuletStarved
    {


        public ProcessWipTankOutletWashingTankState(ProcessWipTankForLine tank) : base(tank)
        {
            tank.WashingTime = tank.GetWashoutTime();
            tank.CurrentWashingTime = tank.ZeroTime;
            StateLabel = $"Washing Tank";
            AddTransition<ProcessWipTankOutletReleaseWashingPumpTankState>(tank => tank.IsWashingTimeCompleted());
        }
        public override void Run(DateTime currentdate)
        {
            StateLabel = $"Washing Tank {Math.Round(Context.PendingWashingTime.GetValue(TimeUnits.Minute), 1)}, min";
            Context.CurrentWashingTime += Context.OneSecond;
        }
        
    }
    public class ProcessWipTankOutletReleaseWashingPumpTankState : ProcessWipOutletState, ITankOuletStarved
    {



        public ProcessWipTankOutletReleaseWashingPumpTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Releasing Washout pump";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => tank.ReleaseWashoutPump());

        }

    }
    public class ProcessWipTankOutletStarvedWashingTankState : ProcessWipOutletState, ITankOuletStarved
    {



        public ProcessWipTankOutletStarvedWashingTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Washing Starved";
            AddTransition<ProcessWipTankOutletWashingTankState>(tank => tank.IsFeederStarvedRealesed());
        }

    }
}
