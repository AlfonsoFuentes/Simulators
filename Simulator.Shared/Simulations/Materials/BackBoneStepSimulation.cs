using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Simulations.Pumps;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Simulations.Materials
{
    public class BackBoneStepSimulation
    {

        public Guid Id { get; set; }
        public MaterialSimulation StepRawMaterial { get; set; } = null!;
        public MaterialSimulation Material { get; set; } = null!;
        public int Order { get; set; }
        public BackBoneStepType BackBoneStepType { get; set; }
        public double Percentage { get; set; }

       
        public Amount Time { get; set; } = new Amount(TimeUnits.Minute);

        public BasePump Pump { get; set; } = null!;
   

    }
}
