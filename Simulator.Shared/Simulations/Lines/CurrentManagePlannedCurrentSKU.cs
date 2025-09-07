namespace Simulator.Shared.Simulations.Lines
{

    public class PlannedSKUSimulationCalculator
    {
        PlannedSKUSimulation PlannedSKUSimulation = null!;

        public PlannedSKUSimulationCalculator(PlannedSKUSimulation managePlannedCurrentSKU)
        {
            PlannedSKUSimulation = managePlannedCurrentSKU;
            PendingMass = PlannedSKUSimulation.PlannedMassSKU;
        }
        Amount OneSecond { get; set; } = new(1, TimeUnits.Second);
        public Amount Cases { get; set; } = new(CaseUnits.Case);
        public Amount Bottles { get; set; } = new(EAUnits.EA);
        public Amount PendingMass { get; set; } = new(MassUnits.KiloGram);
        public Amount ProducedMass { get; set; } = new(MassUnits.KiloGram);
        public void Calculate()
        {
            Bottles += PlannedSKUSimulation.LineSpeed * OneSecond;
            Cases = new(Bottles.GetValue(EAUnits.EA) / PlannedSKUSimulation.EA_Case.GetValue(EACaseUnits.EACase), CaseUnits.Case);

            var massproduced = PlannedSKUSimulation.MassFlow * OneSecond;
            PendingMass -= massproduced;
            ProducedMass += massproduced;
        }
    }

}
