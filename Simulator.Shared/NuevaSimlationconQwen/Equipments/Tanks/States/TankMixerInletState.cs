namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States
{
    public abstract class TankMixerInletState : WipTankInlet
    {
       

        public TankMixerInletState(ProcessWipTankForLine tank) : base(tank)
        {
      
        }
    }
    public class TankInletIniateMixerState : TankMixerInletState
    {

        public TankInletIniateMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Initiate Tank for Mixers Inlet";

        }

    }

    public class TankInletWaitingForInletMixerState : TankMixerInletState
    {

        public TankInletWaitingForInletMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Waiting for transfer from mixers";
            AddTransition<TankInletIniateMixerState>(tank => tank.IsCurrentOrderMassProducedCompleted());
            AddTransition<TankInletReceivingFromMixerState>(tank => tank.ReviewIfTransferCanInit());
            AddTransition<TankInletWaitingForInletMixerState>(tank => tank.IsMaterialNeeded());
        }

    }
    public class TankInletReceivingFromMixerState : TankMixerInletState
    {

        public TankInletReceivingFromMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Receiving product from mixer";
            
            AddTransition<TankInletWaitingForInletMixerState>(tank => tank.IsTransferFinalized());
            AddTransition<TankInletTransferStarvedFromMixerState>(tank =>tank.IsHighLevelDuringMixerTransfer());
            AddTransition<TankInletReceivingFromMixerState>(tank => tank.IsMaterialNeeded());
            
        }
        public override void Run(DateTime currentdate)
        {
            
            Context.SetCurrentMassTransfered();
        }

    }
    
    public class TankInletTransferStarvedFromMixerState : TankMixerInletState, ITransferFromMixerStarved
    {

        public TankInletTransferStarvedFromMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Starved current transfer";
          
            AddTransition<TankInletReceivingFromMixerState>(tank =>tank.IfTransferStarvedByHighLevelCanResume());
            AddTransition<TankInletTransferStarvedFromMixerState>(tank => tank.IsMaterialNeeded());

        }

    }

}


