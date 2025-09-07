using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStepStates
{
    public abstract class MixerStepState
    {

        protected Amount OneSecond = new(1, TimeUnits.Second);

        public abstract void CalculateStep();
        public abstract bool CheckState();
        public abstract void Init();
        public abstract void Reset();

        public string LabelStepStateInferior { get; set; } = string.Empty;
        public string LabelStepStateSuperior { get; set; } = string.Empty;
        protected MixerState MixerState { get; set; }
        protected BackBoneStepSimulation stepSimulation { get; set; }

        protected BaseMixer Mixer { get; set; } = null!;

        protected MixerStepState(MixerState MixerState, BackBoneStepSimulation stepSimulation)
        {
            this.MixerState = MixerState;
            this.stepSimulation = stepSimulation;
            Mixer = this.MixerState.Mixer;
            Mixer.MixerStepState = this;
            Init();
        }
        public bool Calculate()
        {
            CalculateStep();
            if (CheckState())
            {
                Reset();
          
                return true;
            }
            return false;
        }

    }

}
