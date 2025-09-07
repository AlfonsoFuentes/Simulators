using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Tanks;

namespace Simulator.Shared.Simulations.SimulationResults.Mixers
{
    public class MixerResults : EquipmentResult
    {
        public BaseMixer Mixer { get; private set; } = null!;
        public MixerResults(BaseMixer Mixer, string mixerSimulationState)
        {
            MixerState = mixerSimulationState;
            WIPLevel = Mixer.ProducingTo == null ? new(MassUnits.KiloGram) : Mixer.ProducingTo.CurrentLevel;
            backBoneSimulation = Mixer.CurrentBackBoneSimulation == null ? null! : Mixer.CurrentBackBoneSimulation;
            this.Mixer = Mixer;
            CurrentDate = Mixer.CurrentDate;
            MixerLevel = Mixer.CurrentLevel;
            ProducingTo = Mixer.ProducingTo == null ?  null! : Mixer.ProducingTo;
        }
        public BaseTank ProducingTo { get; private set; } = null!;
        public string MixerState { get; private set; } = null!;
        public Amount WIPLevel { get; private set; } = null!;
        public Amount MixerLevel { get; private set; } = null!;
        public BackBoneSimulation backBoneSimulation { get; private set; } = null!;
        public List<MixerStepResult> MixerStepResults { get; private set; } = new();

        public void AddMixerStepResult(string MixerStepState)
        {
            MixerStepResults.Add(new()
            {
                MixerState = Mixer.LabelMixerState,
                MixerStepState = MixerStepState,
                CurrentDate = Mixer.CurrentDate,
                WIPLevel = Mixer.ProducingTo == null ? new(MassUnits.KiloGram) : Mixer.ProducingTo.CurrentLevel,
                MixerLevel = Mixer.CurrentLevel,

            });
        }
    }
    public class MixerStepResult : EquipmentResult
    {
        public string MixerState { get; set; } = string.Empty;
        public string MixerStepState { get; set; } = string.Empty;
        public Amount MixerLevel { get; set; } = null!;

        public Amount WIPLevel { get; set; } = null!;
    }
}
