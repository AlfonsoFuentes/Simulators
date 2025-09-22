using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{

    public class ProcessPump : Equipment, IManufactureFeeder
    {
        public Amount Flow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public Amount ActualFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public List<ProcessBaseTankForRawMaterial> RawMaterialInletTanks => InletEquipments.OfType<ProcessBaseTankForRawMaterial>().ToList();
        public ProcessBaseTankForRawMaterial? InletTank => RawMaterialInletTanks.FirstOrDefault();

        public List<ManufaturingEquipment> InletMixers => InletEquipments.OfType<ManufaturingEquipment>().ToList();
        public ManufaturingEquipment? Mixer => InletMixers.FirstOrDefault();
        public List<ProcessWipTankForLine> InletWipTanks => InletEquipments.OfType<ProcessWipTankForLine>().ToList();
        public ProcessWipTankForLine? WipTank => InletWipTanks.FirstOrDefault();
        public string OcuppiedBy { get; set; } = string.Empty;


        public List<ProcessRecipedRawMaterialTank> RecipedRawMaterialTank => OutletEquipments.OfType<ProcessRecipedRawMaterialTank>().ToList();
        public List<ProcessWipTankForLine> WIPTanksForLines => OutletEquipments.OfType<ProcessWipTankForLine>().ToList();

        public List<ProcessLine> Lines => OutletEquipments.OfType<ProcessLine>().ToList();
        public ProcessLine? Line => Lines.FirstOrDefault();

        public List<ProcessContinuousSystem> Skids => OutletEquipments.OfType<ProcessContinuousSystem>().ToList();
        public ProcessContinuousSystem? Skid => Skids.FirstOrDefault();

        public List<ProcessBaseTank> InletTanks => InletEquipments.OfType<ProcessBaseTank>().ToList();
        public bool IsForWashout { get; set; } = false;

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            OutletState = new FeederAvailableState(this);
        }

        public bool IsAnyTankInletStarved()
        {
            if (InletTanks.Any(x => x.OutletState is ITankOuletStarved))
            {
                return true;
            }
            return false;
        }

        public bool IsInUse()
        {
            if (ProcessFeederManager.IsFeederInUse(this))
            {
                return true;
            }
            return false;
        }
    }

}
