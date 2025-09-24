using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public abstract class TankMixerInlet : InletState<ProcessWipTankForLine>
    {
        protected ProcessWipTankForLine _tank { get; set; }

        public TankMixerInlet(ProcessWipTankForLine tank) : base(tank)
        {
            _tank = tank;
        }
    }
    public class TankInletIniateMixerState : TankMixerInlet
    {

        public TankInletIniateMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Initiate Tank for Mixers Inlet";
            AddTransition<TankInletWaitingForInletMixerState>(tank => tank.IsNewOrderReceivedToStartOrder());
        }

    }
    
    public class TankInletWaitingForInletMixerState : TankMixerInlet
    {

        public TankInletWaitingForInletMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Waiting for transfer from mixers";
            AddTransition<TankInletIniateMixerState>(tank => tank.IsMassPendingToProduceCompleted());
            AddTransition<TankInletReceivingFromMixerState>(tank => tank.ReviewIfTransferCanInit());
            AddTransition<TankInletWaitingForInletMixerState>(tank => tank.IsMaterialNeeded());
        }

    }
    public class TankInletReceivingFromMixerState : TankMixerInlet
    {

        public TankInletReceivingFromMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Receiving product from mixer";
            
            AddTransition<TankInletFinishReceivingFromMixerState>(tank => tank.IsTransferFinalized());
            AddTransition<TankInletFinishStarvedFromMixerState>(tank => tank.IsTankHigherThenHiLevelForMixer());
            AddTransition<TankInletReceivingFromMixerState>(tank => tank.IsMaterialNeeded());
            
        }
        public override void Run(DateTime currentdate)
        {
            Context.SetCurrentMassTransfered();
        }

    }
    public class TankInletFinishReceivingFromMixerState : TankMixerInlet
    {

        public TankInletFinishReceivingFromMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Releasing current transfer";
            AddTransition<TankInletWaitingForInletMixerState>(tank => tank.IsReportFinishTransferToMixer());
            AddTransition<TankInletFinishReceivingFromMixerState>(tank => tank.IsMaterialNeeded());
           
        }

    }
    public class TankInletFinishStarvedFromMixerState : TankMixerInlet
    {

        public TankInletFinishStarvedFromMixerState(ProcessWipTankForLine tank) : base(tank)
        {

            StateLabel = $"Starved current transfer";
          
            AddTransition<TankInletReceivingFromMixerState>(tank => tank.ReviewIfTransferCanReinit());
            AddTransition<TankInletFinishStarvedFromMixerState>(tank => tank.IsMaterialNeeded());

        }

    }
    
}
