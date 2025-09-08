namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates
{
    public class MixerStateStarvedByTankNoAvailable : MixerState
    {


        public MixerStateStarvedByTankNoAvailable(BaseMixer mixer) : base(mixer, "WIP Not Available")
        {
           
        }

        public override void CalculateState()
        {


        }

        public override void CheckState()
        {
            if (Mixer.ProducingTo.GetTankAvailableforTransferMixer(Mixer))
            {
             
                Mixer.MixerState = new MixerStateTransfering(Mixer);
                Mixer.CloseCurrentEvent();
            }
        }

        
    }

}
