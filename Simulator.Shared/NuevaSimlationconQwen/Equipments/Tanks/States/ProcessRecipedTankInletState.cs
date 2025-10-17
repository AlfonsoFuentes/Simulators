using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States
{
    public interface  ITransferFromMixerStarved
    {
        
    }
    public interface ITransferFromMixerAvailable
    {

    }
    public abstract class ProcessRecipedTankInletState : InletState<ProcessRecipedTank>
    {
       

        public ProcessRecipedTankInletState(ProcessRecipedTank tank) : base(tank)
        {
      
        }
    }
    
    
    public class ProcessRecipedTankInletWaitingForInletMixerState : ProcessRecipedTankInletState
    {

        public ProcessRecipedTankInletWaitingForInletMixerState(ProcessRecipedTank tank) : base(tank)
        {

            StateLabel = $"Waiting for transfer from mixers";
          
            AddTransition<ProcessRecipedTankInletReceivingFromMixerState>(tank => tank.ReviewIfTransferCanInit());
            AddTransition<ProcessRecipedTankInletWaitingForInletMixerState>(tank => tank.IsMaterialNeeded());
        }

    }
    public class ProcessRecipedTankInletReceivingFromMixerState : ProcessRecipedTankInletState, ITransferFromMixerAvailable
    {

        public ProcessRecipedTankInletReceivingFromMixerState(ProcessRecipedTank tank) : base(tank)
        {

            StateLabel = $"Receiving product from mixer";
            
            AddTransition<ProcessRecipedTankInletWaitingForInletMixerState>(tank => tank.IsTransferFinalized());
            AddTransition<ProcessRecipedTankInletStarvedFromMixerState>(tank => tank.IsHighLevelDuringMixerTransfer());
            AddTransition<ProcessRecipedTankInletReceivingFromMixerState>(tank => tank.IsMaterialNeeded());
            
        }
        public override void Run(DateTime currentdate)
        {
            Context.SetCurrentMassTransfered();
        }

    }
   
    public class ProcessRecipedTankInletStarvedFromMixerState : ProcessRecipedTankInletState, ITransferFromMixerStarved
    {

        public ProcessRecipedTankInletStarvedFromMixerState(ProcessRecipedTank tank) : base(tank)
        {

            StateLabel = $"Starved current transfer";
          
            AddTransition<ProcessRecipedTankInletReceivingFromMixerState>(tank => tank.IfTransferStarvedByHighLevelCanResume());
            AddTransition<ProcessRecipedTankInletStarvedFromMixerState>(tank => tank.IsMaterialNeeded());

        }

    }

}


