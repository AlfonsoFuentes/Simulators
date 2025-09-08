using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;
using Simulator.Shared.Simulations.Operators;
using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStepStates
{
    public class MixerStepStateByMass : MixerStepState
    {
        Amount MaxMass { get; set; } = new(MassUnits.KiloGram);
        Amount CurrentMass { get; set; } = new(MassUnits.KiloGram);
        BasePump Pump { get; set; } = null!;

        BaseOperator Operator { get; set; } = null!;
        public MixerStepStateByMass(MixerState mixerstate, BackBoneStepSimulation stepSimulation) : base(mixerstate, stepSimulation)
        {
        }

        public override void CalculateStep()
        {
            if (CalculateMass != null) CalculateMass();
            else Init();
        }

        public override bool CheckState()
        {
            return CurrentMass >= MaxMass;
        }

        public override void Init()
        {
            MaxMass = Mixer.CurrentMixerBackBoneCapacity * stepSimulation.Percentage / 100;

            var equipment = Mixer.AddProcessEquipmentInletOrPutQueue(stepSimulation.StepRawMaterial);
            LabelStepStateSuperior = $"Add: {stepSimulation.StepRawMaterial.SAPName}";
            MixerState.UpdateStepState(LabelStepStateSuperior);
            if (equipment != null)
            {
                if(Mixer.CurrentEventId!=Guid.Empty)
                {
                    Mixer.CloseCurrentEvent();
                }

                if (equipment is BasePump pump)
                {
                    Pump = pump;
                    CalculateMass = CalculateByPump;

                }
                else if (equipment is BaseOperator _operator)
                {
                    Operator = _operator;
                    CalculateMass = CalculateByOperator;

                }
            }
            else
            {
                LabelStepStateInferior = $"Starved: {stepSimulation.StepRawMaterial.SAPName}";
                MixerState.UpdateStepState(LabelStepStateInferior);
                if(Mixer.CurrentEventId==Guid.Empty)
                {
                    Mixer.StartEquipmentEvent($"Pump of {stepSimulation.StepRawMaterial.CommonName} not available");
                }
                
               

            }

        }
        Action CalculateMass = null!;
        public override void Reset()
        {
            if (Pump != null) Mixer.RemoveProcessInletEquipment(Pump);
            if (Operator != null) Mixer.RemoveProcessInletEquipment(Operator);
            LabelStepStateInferior = $"Finished: {stepSimulation.StepRawMaterial.SAPName}";
            MixerState.UpdateStepState(LabelStepStateInferior);
            CalculateMass = null!;
        }
        public void CalculateByPump()
        {
            if (Pump != null)
            {
                var mass = CurrentMass + Pump.Flow * OneSecond >= MaxMass ? MaxMass - CurrentMass : Pump.Flow * OneSecond;
                CurrentMass += mass;

                Pump.SetInletFlow(mass / OneSecond);
                Pump.Calculate(Mixer.CurrentDate);
                Mixer.AddMassToMixer(mass);
                LabelStepStateInferior = $"Mass: {CurrentMass}";

            }
            else
            {

                Init();
            }
        }
        public void CalculateByOperator()
        {
            if (Operator != null)
            {
                CurrentMass = MaxMass;

                Mixer.AddMassToMixer(MaxMass);
                LabelStepStateInferior = $"Mass: {CurrentMass}";

            }
            else
            {


                Init();
            }
        }


    }

}
