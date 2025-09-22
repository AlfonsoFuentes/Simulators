
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.ProcessWipTankForLines.States.InletStates.StatesForTankSKIDs;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public class ProcessRawMaterialTank : ProcessBaseTankForRawMaterial
    {

        public override void ValidateInletInitialState(DateTime currentdate)
        {
            InletState = new RawMaterialTankInletInitialState(this);
        }

        public bool FillingRawMaterialTank()
        {
            CurrentLevel += (Capacity - CurrentLevel);
            return true;
        }
    }


}
