using Simulator.Shared.Simulations.SimulationResults.Mixers;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates
{
    public abstract class MixerState
    {
        public BaseMixer Mixer { get; set; } = null!;
        public abstract void CalculateState();
        public abstract void CheckState();
        public void UpdateStepState(string label)
        {
            CurrentMixerResult.AddMixerStepResult(label);
        }
      
        public void Calculate()
        {
            CalculateState();
            CheckState();
        }
       
        protected Amount ZeroMass = new(MassUnits.KiloGram);
        protected Amount ZeroFlow = new(MassFlowUnits.Kg_min);

        public string LabelState { get; set; } = string.Empty;
        public MixerState(BaseMixer mixer, string labelState)
        {

            LabelState = labelState;
            Mixer = mixer;
            Mixer.AddMixerResult(labelState);


        }
        protected Amount ZeroLevel = new(MassUnits.KiloGram);
        public MixerResults CurrentMixerResult => Mixer.CurrentMixerResult;
    }

}
