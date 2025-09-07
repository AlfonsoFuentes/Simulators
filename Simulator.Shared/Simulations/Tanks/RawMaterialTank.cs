using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Mixers;

namespace Simulator.Shared.Simulations.Tanks
{
    public class RawMaterialTank : BaseTank
    {
        public RawMaterialTank(TankDTO tankDTO) : base(tankDTO)
        {

            TankDTO = tankDTO;
           
            CalculateForInlet = () => { };
            
            CalculateForOutlet = CalculateFromOutlet;
           
        }

        public override void Calculate(DateTime currentdate)
        {
            base.Calculate(currentdate);
            if (IsTankLoLevel)
                CurrentLevel = TankDTO.MaxLevel;
            CheckAutomaticoOnOFF();
        }


        protected void CalculateFromOutlet()
        {
            CurrentTime+= OneSecond;
            CurrentLevel -= Outlet;
            TotalMassOutlet += Outlet;
            if (CurrentTime.GetValue(TimeUnits.Minute) > 10)
                AverageOutlet = TotalMassOutlet / CurrentTime;
            Outlet.SetValue(0, MassUnits.KiloGram);

        }


        public override void Init()
        {

            CurrentLevel = TankDTO.InitialLevel;

           
         



        }
        
      

        void CheckAutomaticoOnOFF()
        {
            if (CurrentLevel < TankDTO.MinLevel)
            {
                CurrentLevel = TankDTO.MaxLevel;
            }
        }


        
      
        
        public override bool GetTankAvailableforTransferMixer(BaseMixer mixer)
        {
            return false;
        }
    }


}
