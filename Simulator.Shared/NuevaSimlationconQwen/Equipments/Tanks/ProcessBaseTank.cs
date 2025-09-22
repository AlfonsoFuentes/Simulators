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
       
       
        public bool IsMustWashTank()
        {
            if (LastMaterial == null)
            {

                LastMaterial = Material;
                return false;
            }
            if (Material == null) return false;
            if (Material.Id == LastMaterial.Id) return false;

            var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);

            LastMaterial = Material;
            if (washDef != null)
            {

                return true;
            }

            return false;
        }
        public bool IsWashoutPumpFree()
        {
            if (Feeder != null)
            {
                return true;
            }
            return false;
        }
        public bool IsWashoutPumpAvailable()
        {
            if (!ProcessFeederManager.AnyWashoutPumpAvailable())
            {
                // ❌ No hay bombas → encolar y retornar false
                ProcessFeederManager.EnqueueWashoutRequest(this);
                return false;
            }

            Feeder = ProcessFeederManager.AssignWashingPump(this);

            if (Feeder != null)
            {
                // ✅ Asignación exitosa → retornar true
                return true;
            }
            // ❌ Asignación falló → encolar y retornar false
            ProcessFeederManager.EnqueueWashoutRequest(this);
            return false;
        }
        public bool IsTankInLoLevel()
        {
            if (CurrentLevel < LoLolevel)
            {
                return true;
            }

            return false;
        }
        public Amount GetWashoutTime()
        {
            if (LastMaterial != null)
            {
                var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);
                if (washDef != null)
                {
                    return washDef.LineWashoutTime;
                }
            }

            return new Amount(0, TimeUnits.Second);
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
