using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations.SimulationResults.Lines
{
    public class LineResult : EquipmentResult
    {

        public PlannedSKUSimulation SKU { get; set; } = null!;
        public Amount Cases { get; set; } = null!;
        public string State { get; set; } = null!;

        public Amount ProducedMass { get; set; } = null!;
        public Amount PendingMass { get; set; } = null!;
        public Amount Flow { get; set; } = null!;

        public Amount WIPLevel { get; set; } = null!;
        public Amount TimeInitNextBatch { get; set; } = null!;

    }
}
