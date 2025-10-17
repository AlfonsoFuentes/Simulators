using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States
{
    public interface ITankOuletStarved
    {

    }
    public abstract class TankOutletState : OutletState<ProcessBaseTank>
    {
       

        public TankOutletState(ProcessBaseTank tank) : base(tank)
        {
    
        }
    }
    public class TankOutletInitializeTankState : TankOutletState, ITankOuletStarved
    {



        public TankOutletInitializeTankState(ProcessBaseTank tank) : base(tank)
        {
            if(tank.Name.Contains("T.Agua#2"))
            {
            }

            StateLabel = $"{tank.Name} Review if It is available";
            AddTransition<TankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());
            AddTransition<TankOutletNotAvailableState>();

        }

    }
   

    public class TankOutletAvailableState : TankOutletState
    {



        public TankOutletAvailableState(ProcessBaseTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Available";
            AddTransition<TankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());
            

            AddTransition<TankOutletNotAvailableState>(tank => tank.IsTankInLoLoLevel());

        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateOutletLevel();


        }

    }
    public class TankOutletNotAvailableState : TankOutletState, ITankOuletStarved
    {


        public TankOutletNotAvailableState(ProcessBaseTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Not Available";
            AddTransition<TankOutletAvailableState>(tank => tank.IsTankAvailable());
        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateRunTime();


        }

    }
    public class TankOutletPlannedDownTimeState : TankOutletState, ITankOuletStarved
    {


        public TankOutletPlannedDownTimeState(ProcessBaseTank tank) : base(tank)
        {
            if (tank.Name.Contains("T.Agua#2"))
            {
            }
            StateLabel = $"{tank.Name} Not Available";
            AddTransition<TankOutletAvailableState>(tank => tank.IsEquipmentInPlannedDownTimeStateRealesed());
        }
        public override void Run(DateTime currentdate)
        {
            Context.CalculateRunTime();


        }

    }
}
