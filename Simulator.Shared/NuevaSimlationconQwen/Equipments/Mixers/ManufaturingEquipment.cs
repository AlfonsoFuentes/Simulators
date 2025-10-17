using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public abstract class ManufaturingEquipment : Equipment
    {
        public List<IEquipmentMaterial> RecipedMaterials => EquipmentMaterials.ToList();
        public ManufacturingAnalysisResult AnalysisResult { get; set; } = new();

        public abstract Amount CurrentLevel { get; set; }

        public IManufactureOrder CurrentManufactureOrder { get; set; } = null!;
        public Queue<IManufactureOrder> ManufacturingOrders { get; set; } = new();
        public IMaterial? CurrentMaterial => CurrentManufactureOrder == null ? null! : CurrentManufactureOrder.Material;

        public IMaterial? LastMaterial { get; set; } = null!;

        //Lo que estamos produciendo ahora mismo


        public abstract void ReceiveManufactureOrderFromWIP(ITankManufactureOrder order);
    }
}
