using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Skids;

namespace Simulator.Shared.Simulations.Materials
{
    public class BackBoneSimulation : MaterialSimulation
    {
        public BackBoneSimulation(NewSimulation simulation) : base(simulation)
        {
            Simulation = simulation;
        }
        public List<BackBoneStepSimulation> RawMaterialSteps => StepSimulations.Where(x => x.StepRawMaterial != null).ToList();

        public Queue<BackBoneStepSimulation> StepSimulations { get; private set; } = new();
        public void AddBackBoneStep(BackBoneStepSimulation step)
        {

            StepSimulations.Enqueue(step);

        }
        public Dictionary<BaseMixer, BackBoneStepSimulationCalculation> BatchDataMixer { get; private set; } = new();
        public Dictionary<BaseSKID, BackBoneForSKIDSimulationCalculation> FlowDataSKID { get; private set; } = new();

        public void Init(List<MaterialEquipmentRecord> processEquipmentMaterials)
        {
            try
            {
                foreach (var mixer in Mixers)
                {
                    Amount Capacity = new(0, MassUnits.KiloGram);
                    var materialequipment = processEquipmentMaterials.FirstOrDefault(x => x.MaterialId == Id && x.EquipmentId == mixer.Id);
                    if (materialequipment != null)
                    {
                        Capacity = materialequipment.Capacity;
                        BackBoneStepSimulationCalculation data = new(mixer, StepSimulations, Capacity);
                   
                        data.EnsureCalculated(); // <-- Asegurar que se calcule después de la inicialización
                        BatchDataMixer.Add(mixer, data);
                        var time = data.BatchCycleTime;
                    }
                    else
                        continue;
                    

                }
                foreach (var row in SKIDs)
                {
                    BackBoneForSKIDSimulationCalculation data = new(row, StepSimulations.ToList());
                    FlowDataSKID.Add(row, data);

                }

            }
            catch (Exception e)
            {
                var msg = e.Message;


            }
        }
    }

}
