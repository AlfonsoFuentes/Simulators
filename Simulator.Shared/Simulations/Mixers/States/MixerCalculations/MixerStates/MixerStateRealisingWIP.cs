namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates
{
    public class MixerStateRealisingWIP : MixerState
    {


        public MixerStateRealisingWIP(BaseMixer mixer) : base(mixer, "Realising WIP")
        {

        }



        public override void CheckState()
        {

            Mixer.MixerState = new MixerStateAvailable(Mixer);
            Mixer.OutletPump.SetOutletFlow(new(MassFlowUnits.Kg_hr));
            Mixer.OutletPump.RemoveProcessOutletEquipment(Mixer.ProducingTo);
            Mixer.ProducingTo.SetInletOcupiedBy(null!);
            Mixer.ProducingTo = null!;

        }







        public override void CalculateState()
        {




        }

    }

}
