using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids
{
    public interface ISKIDProducerState
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

            StateLabel = $"Skid producing";
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
    public class SKIDInletStateWaitingStartCommandState : SKIDInletState
    {

        public SKIDInletStateWaitingStartCommandState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Waiting Start command";
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
    public class SKIDTotalInletCommandStopReceivedState : SKIDInletState
    {

        public SKIDTotalInletCommandStopReceivedState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"total command stop received";
            AddTransition<SKIDInletStateWaitingStartCommandState>(skid => skid.IsRawMaterialFeederReleased());
        }

    }
    public class SKIDInletManufacturingState : SKIDInletState, ISKIDProducerState
    {

        public SKIDInletManufacturingState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Manufacturing";
            AddTransition<SKIDTotalInletCommandStopReceivedState>(skid => skid.IsSkidTotalStopReceived());
            AddTransition<SKIDInletManufacturingStarvedByFeederState>(skid => skid.IsRawMaterialFeedersStarved());
            AddTransition<SKIDInletCommandStopReceivedState>(skid => skid.IsInletStopCommandReceived());
        }

    }
    public class SKIDInletManufacturingStarvedByFeederState : SKIDInletState
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

            StateLabel = $"Waiting Start command";
            AddTransition<SKIDTotalInletCommandStopReceivedState>(skid => skid.IsSkidTotalStopReceived());
            AddTransition<SKIDInletStateWaitingStartCommandState>(skid => skid.IsRawMaterialFeederReleased());
        }

    }
  
}
