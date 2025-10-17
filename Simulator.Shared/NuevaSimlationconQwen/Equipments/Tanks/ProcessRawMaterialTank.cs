using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States;

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
            if (Name.Contains("Agua"))
            {

            }
            CurrentLevel += (Capacity - CurrentLevel);
            return true;
        }
    }


}
