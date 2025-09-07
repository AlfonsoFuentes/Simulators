using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Skids.State;
using Simulator.Shared.Simulations.Tanks;

namespace Simulator.Shared.Simulations.SimulationResults.SKids
{
    public class SkidResult : EquipmentResult
    {
        public Amount Flow { get; set; } = null!;

        public SkidState SkidState { get; set; } = null!;

        public BackBoneSimulation BackBoneSimulation { get; set; } = null!;

        public WIPTank WIPTank { get; set; } = null!;
    }
}
