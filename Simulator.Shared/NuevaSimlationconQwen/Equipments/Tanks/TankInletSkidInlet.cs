using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public interface IStarvedWIPTank
    {

    }
    public abstract class WipTankInlet : InletState<ProcessWipTankForLine>
    {
        public WipTankInlet(ProcessWipTankForLine tank) : base(tank)
        {

        }
    }
    public class WipTankInletInitiateState : WipTankInlet
    {

        public WipTankInletInitiateState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} waiting new State at inlet";
           

        }

    }
    public abstract class TankSKIDInlet : WipTankInlet
    {
        

        public TankSKIDInlet(ProcessWipTankForLine tank) : base(tank)
        {
            
        }
    }
    public class TankInletIniateSKIDState : TankSKIDInlet
    {

        public TankInletIniateSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Initiate Tank for SKID at Inlet";
            AddTransition<TankInletOutletStateAvalidableSKIDState>(tank => tank.IsNewOrderReceivedToStartOrder());

        }

    }
    public class TankInletOutletStateAvalidableSKIDState : TankSKIDInlet
    {

        public TankInletOutletStateAvalidableSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Manufacturing Order Recived";

            AddTransition<TankInletManufacturingOrderReceivedSKIDState>(tank => tank.IsOuletAvailable());


        }

    }
    public class TankInletManufacturingOrderReceivedSKIDState : TankSKIDInlet
    {

        public TankInletManufacturingOrderReceivedSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Manufacturing Order Recived";
         
            AddTransition<TankInletProducingBySKIDState>(tank => tank.IsTankIsLowerThanLowLevel());
           

        }

    }
    public class TankInletFinishinOrderReceivedSKIDState : TankSKIDInlet
    {

        public TankInletFinishinOrderReceivedSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Manufacturing Order Finalized";
            AddTransition<WipTankInletInitiateState>(tank => tank.IsInletSKIDFinalizedOrder());
           
        }

    }
    public class TankInletProducingBySKIDState : TankSKIDInlet
    {



        public TankInletProducingBySKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Producing By SKID";
            AddTransition<TankInletFinishinOrderReceivedSKIDState>(tank => tank.IsSKIDWIPProducedCompleted());
            AddTransition<TankInletHighLevelSKIDState>(tank => tank.IsTankHigherThenHiLevel());
           
        }
        

    }
   
    public class TankInletHighLevelSKIDState : TankSKIDInlet
    {



        public TankInletHighLevelSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Waiting for SKID";
          
            AddTransition<TankInletProducingBySKIDState>(tank => tank.IsTankIsLowerThanLowLevel());
        }

    }
    

}
