using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStepStates;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates
{
    public class MixerStateBatching : MixerState
    {
        protected Queue<BackBoneStepSimulation> steps = new();
        MixerStepState MixerStepState = null!;
        public BackBoneStepSimulation BackBoneStep { get; set; } = null!;
        public MixerStateBatching(BaseMixer mixer) : base(mixer, "Batching")
        {
            foreach (var step in Mixer.CurrentBackBoneSimulation.StepSimulations)
            {
                steps.Enqueue(step);
            }

            BackBoneStep = ChangeStep();
            if (BackBoneStep != null)
            {
                MixerStepState = InitStepState(BackBoneStep);
            }
        }
        public MixerStateBatching(BaseMixer mixer, BackBoneStepSimulation currentStep) : base(mixer, "Batching")
        {
            foreach (var step in Mixer.CurrentBackBoneSimulation.StepSimulations)
            {
                steps.Enqueue(step);
            }

            do
            {
                BackBoneStep= ChangeStep();
            }
            while (BackBoneStep.Id != currentStep.Id);
            if(BackBoneStep!=null)
            {
                MixerStepState = InitStepState(BackBoneStep);
            }
        }

        public override void CalculateState()
        {

            if (MixerStepState.Calculate())
            {
                BackBoneStep = ChangeStep();
                if (BackBoneStep != null)
                {
                    MixerStepState = InitStepState(BackBoneStep);
                }
            }
        }


        BackBoneStepSimulation ChangeStep()
        {
            BackBoneStepSimulation retorno = null!;
            if (steps.Count == 0){

                return retorno;
            }
            retorno = steps.Peek();
            if (retorno != null)
            {
                steps.Dequeue();

            }
            return retorno!;
        }
        MixerStepState InitStepState(BackBoneStepSimulation step) => step.BackBoneStepType switch
        {
            BackBoneStepType.Add => new MixerStepStateByMass(this, step),
            BackBoneStepType.Washout=> new MixerStepStateByWashout(this, step),
            BackBoneStepType.Connect_Mixer_WIP=> new StepSimulationCalculationByConnectMixerToWIP(this, step),
            _ =>  new MixerStepStateByTime(this, step),

        };
        public override void CheckState()
        {
            if (BackBoneStep == null)
            {
                Mixer.EndBatchDate = Mixer.CurrentDate;

                Mixer.MixerState = new MixerStateTransfering(Mixer);

            }
        }


    }

}
