using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States
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
           

        }

    }
   
    public class TankInletManufacturingOrderReceivedSKIDState : TankSKIDInlet
    {

        public TankInletManufacturingOrderReceivedSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Manufacturing Order Recived";
         
            AddTransition<TankInletProducingBySKIDState>(tank => tank.IsSKIDCanStart());
           

        }

    }
    
    public class TankInletProducingBySKIDState : TankSKIDInlet
    {



        public TankInletProducingBySKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Producing By SKID";
            AddTransition<TankInletIniateSKIDState>(tank =>tank.IsSKIDWIPProducedCompleted());
            AddTransition<TankInletHighLevelSKIDState>(tank => tank.IsSKIDMustStop());
           
        }
        

    }
   
    public class TankInletHighLevelSKIDState : TankSKIDInlet
    {



        public TankInletHighLevelSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Waiting for SKID";
          
            AddTransition<TankInletProducingBySKIDState>(tank => tank.IsSKIDCanStart());
        }

    }
    

}
