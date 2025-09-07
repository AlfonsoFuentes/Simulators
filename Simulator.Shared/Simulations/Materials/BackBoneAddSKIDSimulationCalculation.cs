using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations.Materials
{
    public class BackBoneAddSKIDSimulationCalculation
    {
        public Amount Flow { get; set; }


        public BasePump Pump { get; set; }

        public BackBoneAddSKIDSimulationCalculation(BasePump pump, Amount flow)
        {
            Pump = pump;
            Flow = flow;
        }
    }

}
