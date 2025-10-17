using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States
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
        }

    }

    public class ProcessWipTankOutletReviewInitInletStateTankState : ProcessWipOutletState, ITankOuletStarved
    {
        public ProcessWipTankOutletReviewInitInletStateTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Review if must wash tank";
            AddTransition<ProcessWipTankOutletReviewWashingTankState>(tank => tank.IsMustWashTank());
            AddTransition<ProcessWipTankOutletAvailableState>(tank =>
            {
                // ✅ Aquí, configura el estado de ENTRADA
                tank.SelectInletStateBasedOnManufacturingEquipment();
                return true;
            });

        }

    }

    public class ProcessWipTankOutletAvailableState : ProcessWipOutletState
    {
        public ProcessWipTankOutletAvailableState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Available to deliver mass to line";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => tank.IsNextOrderMaterialNeeded());
            AddTransition<ProcessWipTankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());
            AddTransition<ProcessWipTankOutletAvailableToEmptyTankState>(tank => tank.IsCurrentOrderMassDeliveredCompleted());

            AddTransition<ProcessWipTankOutletNotAvailableState>(tank => tank.IsTankInLoLoLevel());

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
            AddTransition<ProcessWipTankOutletAvailableToEmptyTankState>(tank => tank.IsNextOrderMaterialNeeded());
            AddTransition<ProcessWipTankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());

            AddTransition<ProcessWipTankOutletReviewInitInletStateTankState>(tank =>
            {
                if(tank.IsTankInLoLoLevel())
                {
                    tank.IsCurrentOrderRealesed();
                    return true;
                }
                return false;
            });

        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateOutletLevel();
       

        }

    }
    

    public class ProcessWipTankOutletNotAvailableState : ProcessWipOutletState, ITankOuletStarved
    {


        public ProcessWipTankOutletNotAvailableState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Low Level Tank";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => !tank.IsTankInLoLoLevel());
        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateRunTime();


        }

    }
    public class ProcessWipTankOutletPlannedDownTimeState : ProcessWipOutletState, ITankOuletStarved
    {


        public ProcessWipTankOutletPlannedDownTimeState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Planned down time";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => !tank.IsEquipmentInPlannedDownTimeState());
        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateRunTime();


        }

    }
    public class ProcessWipTankOutletReviewWashingTankState : ProcessWipOutletState, ITankOuletStarved
    {



        public ProcessWipTankOutletReviewWashingTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Review any washing pump available";
            AddTransition<ProcessWipTankOutletWashingTankState>(tank => tank.IsWashoutPumpAvailable());
            AddTransition<ProcessWipTankOutletStarvedWashingTankState>();
        }

    }

    public class ProcessWipTankOutletWashingTankState : ProcessWipOutletState, ITankOuletStarved
    {

        Amount CurrentWashingTime = new Amount(0, TimeUnits.Second);
        Amount WashingTime = new Amount(0, TimeUnits.Second);
        Amount PendingWashingTime => WashingTime - CurrentWashingTime;
        public ProcessWipTankOutletWashingTankState(ProcessWipTankForLine tank) : base(tank)
        {
            WashingTime = tank.GetWashoutTime();

            StateLabel = $"Washing Tank";
            AddTransition<ProcessWipTankOutletAvailableState>(tank => IsWashingTimeCompleted());
        }
        public override void Run(DateTime currentdate)
        {
            StateLabel = $"Washing Tank {Math.Round(PendingWashingTime.GetValue(TimeUnits.Minute), 1)}, min";
            CurrentWashingTime += Context.OneSecond;
            Context.CalculateRunTime();
        }
        bool IsWashingTimeCompleted()
        {
            if (PendingWashingTime <= Context.ZeroTime)
            {
                Context.ReleaseWashoutPump();
                return true;
            }
            return false;
        }
    }

    public class ProcessWipTankOutletStarvedWashingTankState : ProcessWipOutletState, ITankOuletStarved
    {



        public ProcessWipTankOutletStarvedWashingTankState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Washing Starved";
            AddTransition<ProcessWipTankOutletWashingTankState>(tank =>
            {
                if (tank.Feeder != null && tank.Feeder.OcuppiedBy == tank)
                {
                    return true;
                }
                return false;
            });
        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateRunTime();


        }
    }
}
