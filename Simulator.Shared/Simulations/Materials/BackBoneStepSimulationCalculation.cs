using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Simulations.Mixers;

namespace Simulator.Shared.Simulations.Materials
{
    public class BackBoneStepSimulationCalculation
    {
        Queue<BackBoneStepSimulation> steps = null!;
        BaseMixer Mixer = null!;

        public Amount Capacity { get; set; } = new Amount(0, MassUnits.KiloGram);

        // Propiedades pre-calculadas para evitar cálculos en UI
        public List<StepTimeDetail> StepTimes { get; private set; } = new();
        public Amount TotalTime { get; private set; } = new Amount(0, TimeUnits.Minute);
        public Amount TransferTime { get; private set; } = new Amount(0, TimeUnits.Minute);
        public Amount BatchCycleTime { get; set; } = new Amount(0, TimeUnits.Minute);

       
        private bool _isCalculated = false;

       

        public BackBoneStepSimulationCalculation(BaseMixer mixer, Queue<BackBoneStepSimulation> step, Amount capacity)
        {
            steps = step ?? new Queue<BackBoneStepSimulation>();
            Mixer = mixer ?? throw new ArgumentNullException(nameof(mixer));
            Capacity = capacity;
            // NO calcular aquí, solo inicializar
        }

        // Método público para calcular cuando todo esté listo
        public void EnsureCalculated()
        {
            if (!_isCalculated)
            {
                CalculateAllTimes();
                _isCalculated = true;
            }
        }

        private void CalculateAllTimes()
        {
            // Verificar que todo esté inicializado
            if (Mixer == null || steps == null)
                return;

            if (Mixer.OutletPump == null) // Protección adicional
            {
                TransferTime = new Amount(0, TimeUnits.Minute);

            }
            else
            {
                TransferTime = Capacity / Mixer.OutletPump.Flow;
            }

            var stepscalc = steps.ToList();
            Amount totalTime = new(TimeUnits.Minute);
            StepTimes.Clear();

            foreach (var step in stepscalc)
            {
                if (step != null) // Protección contra null
                {
                    var stepTime = GetTime(step);
                    totalTime += stepTime;

                    StepTimes.Add(new StepTimeDetail
                    {
                        Step = step,
                        Time = stepTime,
                        StepDescription = GetStepDescription(step),
                        StepType = step.BackBoneStepType.ToString()
                    });
                }
            }

            BatchCycleTime = totalTime;

            TotalTime = totalTime + TransferTime;
        }

        private string GetStepDescription(BackBoneStepSimulation step)
        {
            if (step.StepRawMaterial != null)
            {
                return $"{step.BackBoneStepType} {step.StepRawMaterial.M_NumberCommonName ?? step.StepRawMaterial.SAPName ?? "Unknown Material"}";
            }
            return $"{step.BackBoneStepType} Step";
        }

        public Amount GetTime(BackBoneStepSimulation step) => step.BackBoneStepType switch
        {
            BackBoneStepType.Add => CalculateTimeMass(step),
            _ => step.Time,
        };

        Amount CalculateTimeMass(BackBoneStepSimulation step)
        {
            if (step.StepRawMaterial == null) return new Amount(0, TimeUnits.Minute);

            var pump = step.StepRawMaterial.GetRawMaterialPumpByDestinationId(Mixer.Id);
            if (pump != null)
            {
                var pumpflow = pump.Flow;
                var time = Capacity * step.Percentage / 100 / pumpflow;
                return time;
            }

            var operador = step.StepRawMaterial.GetRawMaterialOperatorByDestinationId(Mixer.Id);
            if (operador != null)
            {
                return new Amount(30, TimeUnits.Second);
            }

            return new Amount(0, TimeUnits.Minute);
        }
    }
    public class StepTimeDetail
    {
        public BackBoneStepSimulation Step { get; set; } = null!;
        public Amount Time { get; set; } = new Amount(0, TimeUnits.Minute);
        public string StepDescription { get; set; } = string.Empty;
        public string StepType { get; set; } = string.Empty;
    }
}
