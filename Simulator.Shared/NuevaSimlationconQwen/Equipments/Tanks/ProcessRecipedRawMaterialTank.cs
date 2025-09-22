using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public class ProcessRecipedRawMaterialTank : ProcessBaseTankForRawMaterial
    {
       

        public List<ProcessMixer> InletMixers => InletEquipments.OfType<ProcessMixer>().ToList();
        public List<ProcessContinuousSystem> SKIDs => InletEquipments.OfType<ProcessContinuousSystem>().ToList();
    }


}
