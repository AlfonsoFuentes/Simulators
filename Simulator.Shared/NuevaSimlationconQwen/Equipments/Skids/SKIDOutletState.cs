using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids
{
    public interface ISKIDStarvedInletState
    {

    }
    public abstract class SKIDOutletState : OutletState<ProcessContinuousSystem>
    {
        protected ProcessContinuousSystem _skid { get; set; }

        public SKIDOutletState(ProcessContinuousSystem skid) : base(skid)
        {
            _skid = skid;
        }
    }
    public class SKIDOutletWaitingNewOrderState : SKIDOutletState
    {

        public SKIDOutletWaitingNewOrderState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Waiting new SKID Order";
           
        }

    }
    public class SKIDOutletStopState : SKIDOutletState
    {

        public SKIDOutletStopState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Skid Waiting start command";



        }
       
    }


    public class SKIDOutletProducingState : SKIDOutletState
    {

        public SKIDOutletProducingState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Skid Delivering to WIP";
         
        }
        public override void Run(DateTime currentdate)
        {
            _skid.SendProductToWIPS();
        }
    }
    public class SKIDOutletStarvedbyInletState : SKIDOutletState
    {

        public SKIDOutletStarvedbyInletState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Skid starved by pumps";
     
        }
       
    }
    
}
