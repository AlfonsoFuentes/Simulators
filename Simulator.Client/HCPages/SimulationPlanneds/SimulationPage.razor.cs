using Simulator.Shared.Simulations;

namespace Simulator.Client.HCPages.SimulationPlanneds
{
    public partial class SimulationPage
    {
        [Parameter]
        public NewSimulation Simulation { get; set; } = null!;
    }
}
