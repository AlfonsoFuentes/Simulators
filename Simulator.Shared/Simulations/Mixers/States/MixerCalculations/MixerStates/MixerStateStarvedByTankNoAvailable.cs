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
                if (!string.IsNullOrEmpty(Mixer.CurrentEventId))
                {
                    Mixer.EndEquipmentEvent(
                        Mixer.CurrentEventId,
                        "WIP Available",
                        $"Mixer {Mixer.Name} resumed operation - WIP tank level restored at {Mixer.Simulation?.CurrentDate:HH:mm:ss}"
                    );

                    Mixer.CurrentEventId = null!;
                }
            }
        }

        
    }

}
