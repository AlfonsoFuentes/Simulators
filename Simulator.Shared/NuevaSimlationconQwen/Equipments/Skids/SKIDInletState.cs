using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids
{
    public abstract class SKIDInletState : InletState<ProcessContinuousSystem>
    {


        public SKIDInletState(ProcessContinuousSystem skid) : base(skid)
        {

        }
    }
    public class SKIDInletStateWaitingNewOrderState : SKIDInletState
    {

        public SKIDInletStateWaitingNewOrderState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Waiting for new Order";

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

            AddTransition<SKIDInletManufacturingStarvedByFeederState>(skid => skid.IsRawMaterialFeedersStarved());

        }

    }
    public class SKIDInletStopState : SKIDInletState
    {

        public SKIDInletStopState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Skid Waiting start command";



        }

    }
    public class SKIDInletManufacturingStarvedByFeederState : SKIDInletState, ISKIDStarvedInletState
    {

        public SKIDInletManufacturingStarvedByFeederState(ProcessContinuousSystem skid) : base(skid)
        {

            StateLabel = $"Starved by Feeder no available";
            AddTransition<SKIDInletReviewPumpsAvailableState>(skid =>
            {
                if (skid.Feeder != null && skid.Feeder.OcuppiedBy == skid)
                {
                    skid.EndCriticalReport();
                    skid.Feeder.OcuppiedBy = null!;
                    skid.Feeder = null!;
              
                    return true;
                }
                return false;
            });//Aqui pner condicion de que las bombas estan disponibles
        }

    }



}
