using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public abstract class ProcessBaseTankForRawMaterial : ProcessBaseTank, ISetMaterialsAtOutlet,ILiveReportable
    {
      
       
        public virtual void SetMaterialsAtOutlet(IMaterial material)
        {
            foreach (var outlet in OutletPumps)
            {
                outlet.AddMaterial(material);

            }
        }
        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            CurrentLevel = InitialLevel;
            OutletState = new TankOutletInitializeTankState(this);



        }
        

    }


}
