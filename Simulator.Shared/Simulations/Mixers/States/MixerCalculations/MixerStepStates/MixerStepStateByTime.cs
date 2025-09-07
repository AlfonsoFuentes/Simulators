using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStepStates
{
    public class MixerStepStateByTime : MixerStepState
    {
        Amount MaxTime { get; set; } = new(TimeUnits.Minute);
        Amount CurrenTime { get; set; } = new(TimeUnits.Minute);

        public MixerStepStateByTime(MixerState mixer, BackBoneStepSimulation stepSimulation) : base(mixer, stepSimulation)
        {
        }

        public override void CalculateStep()
        {
            CurrenTime+= OneSecond;

            LabelStepStateInferior = $"Time: {CurrenTime}";
        }

        public override bool CheckState()
        {
            return CurrenTime >= MaxTime;
        }

        public override void Init()
        {
            MaxTime = stepSimulation.Time;
            LabelStepStateSuperior = $"Step: {stepSimulation.BackBoneStepType}";
            MixerState.UpdateStepState(LabelStepStateSuperior);
        }

        public override void Reset()
        {

        }


    }

}
