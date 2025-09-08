namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates
{
    public class MixerStateStarvedByTankHiLevel : MixerState
    {


        public MixerStateStarvedByTankHiLevel(BaseMixer mixer) : base(mixer, "WIP High Level")
        {
        

        }

        public override void CalculateState()
        {

        }

        public override void CheckState()
        {
            if (!Mixer.ProducingTo.IsTankHiLevel)
            {
               
                Mixer.MixerState = new MixerStateTransfering(Mixer);
                Mixer.CloseCurrentEvent();
            }
        }

       
    }

}
