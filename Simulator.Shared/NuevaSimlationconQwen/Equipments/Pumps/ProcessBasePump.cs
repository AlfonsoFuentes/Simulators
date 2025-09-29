using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{

    public class ProcessPump : ManufactureFeeder
    {
        // Propiedades específicas de ProcessPump
        public List<ProcessBaseTankForRawMaterial> RawMaterialInletTanks =>
            InletEquipments.OfType<ProcessBaseTankForRawMaterial>().ToList();

        public ProcessBaseTankForRawMaterial? InletTank => RawMaterialInletTanks.FirstOrDefault();
        public List<ManufaturingEquipment> InletManufacturingEquipments => InletEquipments.OfType<ManufaturingEquipment>().ToList();
        public List<ProcessWipTankForLine> InletWipTanks => InletEquipments.OfType<ProcessWipTankForLine>().ToList();
        public ProcessWipTankForLine? WipTank => InletWipTanks.FirstOrDefault();
        public List<ProcessRecipedRawMaterialTank> RecipedRawMaterialTank => OutletEquipments.OfType<ProcessRecipedRawMaterialTank>().ToList();
        public List<ProcessWipTankForLine> WIPTanksForLines => OutletEquipments.OfType<ProcessWipTankForLine>().ToList();
        public List<ProcessLine> Lines => OutletEquipments.OfType<ProcessLine>().ToList();
        public ProcessLine? Line => Lines.FirstOrDefault();
        public List<ProcessContinuousSystem> Skids => OutletEquipments.OfType<ProcessContinuousSystem>().ToList();
        public ProcessContinuousSystem? Skid => Skids.FirstOrDefault();
        public List<ProcessBaseTank> InletTanks => InletEquipments.OfType<ProcessBaseTank>().ToList();

        // 👇 Define si es para lavado o no (ajusta según tu lógica)
        public override bool IsForWashout { get; set; } = false;

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            OutletState = new FeederAvailableState(this);
        }

        public override bool IsAnyTankInletStarved()
        {
            return InletTanks.Any(x => x.OutletState is ITankOuletStarved);
        }
    }
}
