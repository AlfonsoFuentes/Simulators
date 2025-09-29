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
            AddTransition<SKIDOutletStartInletStateSKIDState>(skid => skid.IsOutletNewOrderReceived());
        }

    }
    public class SKIDOutletStartInletStateSKIDState : SKIDOutletState
    {

        public SKIDOutletStartInletStateSKIDState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Start inlet state SKID";
            AddTransition<SKIDOutletWaitingStartCommandState>(mixer => mixer.IsInitInletStateInit());

        }

    }
   
    public class SKIDOutletWaitingStartCommandState : SKIDOutletState
    {

        public SKIDOutletWaitingStartCommandState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Waiting Start command";
            AddTransition<SKIDOutletProducingState>(skid => skid.IsOutletStartCommandReceived());
        }

    }
    public class SKIDOutletProducingState : SKIDOutletState
    {

        public SKIDOutletProducingState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Skid Delivering to WIP";
            AddTransition<SKIDOutletWaitingNewOrderState>(skid => skid.IsOutletSkidTotalStopReceived());
            AddTransition<SKIDOutletWaitingStartCommandState>(skid => skid.IsOutletStopCommandReceived());
            AddTransition<SKIDOutletStarvedbyInletState>(skid => skid.IsSkidStarved());
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
            AddTransition<SKIDOutletProducingState>(skid => skid.IsSkidStarvedReleased());
        }
       
    }

    public abstract class SKIDInletState : InletState<ProcessContinuousSystem>
    {
        protected ProcessContinuousSystem _skid { get; set; }

        public SKIDInletState(ProcessContinuousSystem skid) : base(skid)
        {
            _skid = skid;
        }
    }
    public class SKIDInletStateWaitingNewOrderState : SKIDInletState
    {

        public SKIDInletStateWaitingNewOrderState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Waiting for new Order";
            AddTransition<SKIDInletStateWaitingStartCommandState>(skid => skid.IsInletNewOrderReceived());
        }

    }
    public class SKIDInletStateWaitingStartCommandState : SKIDInletState
    {

        public SKIDInletStateWaitingStartCommandState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Waiting for Start command";
            AddTransition<SKIDInletReviewPumpsAvailableState>(skid => skid.IsInletStartCommandReceived());
        }

    }
    
    public class SKIDInletReviewPumpsAvailableState : SKIDInletState
    {

        public SKIDInletReviewPumpsAvailableState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Review Inlets Pumps";
            AddTransition<SKIDInletManufacturingState>(skid => skid.IsFeederCatched());
            AddTransition<SKIDInletManufacturingStarvedByFeederState>();
      
            
        }

    }
    
    public class SKIDInletManufacturingState : SKIDInletState
    {

        public SKIDInletManufacturingState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Skid Inlet producing";
            AddTransition<SKIDInletTotalCommandStopReceivedState>(skid => skid.IsInletSkidTotalStopReceived());
            AddTransition<SKIDInletManufacturingStarvedByFeederState>(skid => skid.IsRawMaterialFeedersStarved());
            AddTransition<SKIDInletCommandStopReceivedState>(skid => skid.IsInletStopCommandReceived());
        }

    }
    public class SKIDInletManufacturingStarvedByFeederState : SKIDInletState, ISKIDStarvedInletState
    {

        public SKIDInletManufacturingStarvedByFeederState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Starved by Feeder no available";
            AddTransition<SKIDInletManufacturingState>(skid => skid.IsRawMaterialFeedersReleaseStarved());//Aqui pner condicion de que las bombas estan disponibles
        }

    }
    
    public class SKIDInletCommandStopReceivedState : SKIDInletState
    {

        public SKIDInletCommandStopReceivedState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Stop command received";
          
            AddTransition<SKIDInletStateWaitingStartCommandState>(skid => skid.IsRawMaterialFeederReleased());
        }

    }
    public class SKIDInletTotalCommandStopReceivedState : SKIDInletState
    {

        public SKIDInletTotalCommandStopReceivedState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Stop total command received";

            AddTransition<SKIDInletStateWaitingNewOrderState>(skid => skid.IsRawMaterialFeederCurrentOrderReleased());
        }

    }
}
