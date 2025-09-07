using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;
using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStepStates
{
    public class MixerStepStateByWashout : MixerStepState
    {
        Amount MaxTime { get; set; } = new(TimeUnits.Minute);
        Amount CurrenTime { get; set; } = new(TimeUnits.Minute);
        BasePump Pump { get; set; } = null!;
        public MixerStepStateByWashout(MixerState mixer, BackBoneStepSimulation stepSimulation) : base(mixer, stepSimulation)
        {
        }

        public override void CalculateStep()
        {
            if (Pump != null)
            {
                CurrenTime+= OneSecond;
                LabelStepStateInferior = $"Time: {CurrenTime}";

            }
            else
            {
                Init();
                LabelStepStateInferior = "Starved by Washout Pump";
                MixerState.UpdateStepState(LabelStepStateInferior);
            }

        }

        public override bool CheckState()
        {
            return CurrenTime > MaxTime;
        }

        public override void Init()
        {
            MaxTime = Mixer.GetNextWashoutTime(stepSimulation.Material);
            Pump = Mixer.SearchInletWashingEquipment();
            LabelStepStateSuperior = Pump != null ? $"Washing Mixer: {Mixer.Name}" : "Starved by Washout Pump";
            MixerState.UpdateStepState(LabelStepStateSuperior);
        }

        public override void Reset()
        {
            Mixer.RemoveProcessInletEquipment(Pump);
            LabelStepStateSuperior = "Finishing Washout";
            MixerState.UpdateStepState(LabelStepStateSuperior);
        }


    }


}
