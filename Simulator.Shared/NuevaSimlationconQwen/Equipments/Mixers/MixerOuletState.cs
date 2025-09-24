using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public abstract class MixerOuletState : OutletState<ProcessMixer>
    {
        protected MixerOuletState(ProcessMixer mixer) : base(mixer)
        {
        }


    }
    public class MixerOuletInitialState : MixerOuletState
    {
        public MixerOuletInitialState(ProcessMixer wip) : base(wip)
        {
            //Sirve para identificar en el constructor por cual via iniciara el calculo por tanques wips conectados a mixers o tanques wips conectados a SKID
            StateLabel = $"Initiating Calculation for Mixer outlet";
            AddTransition<MixerOuletWaitingState>();
        }


    }

    public class MixerOuletWaitingState : MixerOuletState
    {


        public MixerOuletWaitingState(ProcessMixer mixer) : base(mixer)
        {
            StateLabel = $"Waiting for Receive Transfer Request";
            AddTransition<MixerOuletTransferingToWIPState>(mixer => mixer.IsTransferRequestReceived());

            
        }


    }


    public class MixerOuletTransferingToWIPState : MixerOuletState
    {


        public MixerOuletTransferingToWIPState(ProcessMixer mixer) : base(mixer)
        {
            if(mixer.CurrentTransferRequest!=null)
            {
                StateLabel = $"Transfering to {mixer.CurrentTransferRequest.DestinationWip.Name}";
            }
            
            AddTransition<MixerOuletTransferingToWIPStarvedState>(mixer => mixer.IsTransferStarved());
            AddTransition<MixerOuletWaitingState>(mixer => mixer.IsOutletTransferFinished());

           
        }




    }
    public class MixerOuletTransferingToWIPStarvedState : MixerOuletState
    {


        public MixerOuletTransferingToWIPStarvedState(ProcessMixer mixer) : base(mixer)
        {
            StateLabel = $"Transfering to WIP Starved";
            AddTransition<MixerOuletTransferingToWIPState>(mixer => mixer.IsTranferStarvedReleased());
    
           
        }

    }
    
    
}