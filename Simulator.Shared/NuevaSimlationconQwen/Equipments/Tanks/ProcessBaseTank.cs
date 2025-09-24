using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public abstract class ProcessBaseTank : Equipment
    {
        public Amount Capacity { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount HiLevel { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount LoLevel { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount LoLolevel { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount InitialLevel { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount CurrentLevel { get; set; } = new Amount(0, MassUnits.KiloGram);
    
        public List<ProcessPump> OutletPumps => OutletEquipments.OfType<ProcessPump>().ToList();

        public Amount OutletFlows => new Amount(OutletPumps.Sum(x => x.ActualFlow.GetValue(MassFlowUnits.Kg_sg)), MassFlowUnits.Kg_sg);
        public string MaterialName => Material?.CommonName ?? "No Material";

        public IMaterial? LastMaterial { get; set; } = null!;
       
       
        
        public bool IsTankInLoLevel()
        {
            if (CurrentLevel < LoLolevel)
            {
                return true;
            }

            return false;
        }
        
        public virtual void CalculateOutletLevel()
        {
            var mass = OutletFlows * OneSecond;
        
            CurrentLevel -= mass;
            
        }

        public bool ReleaseWashingPump()
        {
            if (ProcessFeederManager.ReleaseEquipment(this))
            {
              
                Feeder = null!;
                return true;
            }
            return false;
        }
        
    }


}
