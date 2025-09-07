namespace Simulator.Shared.Simulations.Tanks.WIPLinetMixers
{
    public class ManageMassWIPTank
    {
        public int Batchs { get; set; } = 0;
        public Amount Producing { get; set; } = new(MassUnits.KiloGram);
        public Amount Needed { get; set; } = new(MassUnits.KiloGram);
        public Amount Delivered { get; set; } = new(MassUnits.KiloGram);
        public Amount NeedToDelivered { get; set; } = new(MassUnits.KiloGram);
        public Amount Received { get; set; } = new(MassUnits.KiloGram);

        public string NeedToDeliveredString=> NeedToDelivered.GetValue(MassUnits.KiloGram).ToString(); 

    }



}
