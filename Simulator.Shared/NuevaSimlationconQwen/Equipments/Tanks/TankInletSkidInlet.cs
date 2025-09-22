using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public interface IStarvedWIPTank
    {

    }
    public abstract class TankSKIDInlet : InletState<ProcessWipTankForLine>
    {
        protected ProcessWipTankForLine _tank { get; set; }

        public TankSKIDInlet(ProcessWipTankForLine tank) : base(tank)
        {
            _tank = tank;
        }
    }
    public class TankInletIniateSKIDState : TankSKIDInlet
    {

        public TankInletIniateSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Initiate Tank for SKID at Inlet";
            AddTransition<TankInletManufacturingOrderReceivedSKIDState>(tank => tank.IsNewOrderReceivedToStartOrder());
            
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
            AddTransition<TankInletIniateSKIDState>(tank => tank.IsInletSKIDFinalizedOrder());
           
        }

    }
    public class TankInletProducingBySKIDState : TankSKIDInlet
    {



        public TankInletProducingBySKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Producing By SKID";
            AddTransition<TankInletFinishinOrderReceivedSKIDState>(tank => tank.IsMassDeliveredCompleted());
            AddTransition<TankInletHighLevelSKIDState>(tank => tank.IsTankHigherThenHiLevel());
           
        }
        

    }
   
    public class TankInletHighLevelSKIDState : TankSKIDInlet
    {



        public TankInletHighLevelSKIDState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Waiting for SKID";
            AddTransition<TankInletFinishinOrderReceivedSKIDState>(tank => tank.IsMassDeliveredCompleted());
            AddTransition<TankInletProducingBySKIDState>(tank => tank.IsTankIsLowerThanLowLevel());
        }

    }
    

}
