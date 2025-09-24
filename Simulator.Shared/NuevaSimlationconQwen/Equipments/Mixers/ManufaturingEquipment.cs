using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public abstract class ManufaturingEquipment : Equipment
    {
        public List<IEquipmentMaterial> RecipedMaterials => EquipmentMaterials.ToList();
        public ManufacturingAnalysisResult AnalysisResult { get; set; } = new();
        public void ReceiveManufacturingOrderFromWIP(WIPManufacturingOrder order, ProcessWipTankForLine wip)
        {
            
            CurrentOrder = new FromWIPToMixerManufactureOrder(order.Material, wip);
        }
        protected FromWIPToMixerManufactureOrder CurrentOrder { get; set; } = null!;

        public IMaterial CurrentMaterial => CurrentOrder == null ? null! : CurrentOrder.Material;


    }
}
