using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public interface ITankOuletStarved
    {

    }
    public abstract class TankOutletState : OutletState<ProcessBaseTank>
    {
        protected ProcessBaseTank _tank { get; set; }

        public TankOutletState(ProcessBaseTank tank) : base(tank)
        {
            _tank = tank;
        }
    }
    public class TankOutletInitializeTankState : TankOutletState, ITankOuletStarved
    {



        public TankOutletInitializeTankState(ProcessBaseTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Review if must wash tank";
            AddTransition<TankOutletNotAvailableState>();



        }

    }
   

    public class TankOutletAvailableState : TankOutletState
    {



        public TankOutletAvailableState(ProcessBaseTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Available";
            AddTransition<TankOutletPlannedDownTimeState>(tank => tank.IsEquipmentInPlannedDownTimeState());
            

            AddTransition<TankOutletNotAvailableState>(tank => tank.IsTankInLoLevel());

        }
        public override void Run(DateTime currentdate)
        {
            _tank.CalculateOutletLevel();


        }

    }
    public class TankOutletNotAvailableState : TankOutletState, ITankOuletStarved
    {


        public TankOutletNotAvailableState(ProcessBaseTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Not Available";
            AddTransition<TankOutletAvailableState>(tank => !tank.IsTankInLoLevel());
        }


    }
    public class TankOutletPlannedDownTimeState : TankOutletState, ITankOuletStarved
    {


        public TankOutletPlannedDownTimeState(ProcessBaseTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Not Available";
            AddTransition<TankOutletAvailableState>(tank => !tank.IsEquipmentInPlannedDownTimeState());
        }


    }
}
