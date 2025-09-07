using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Skids;
using Simulator.Shared.Simulations.Tanks.WIPInletSKIDs.States;
using Simulator.Shared.Simulations.Tanks.WIPLinetMixers;

namespace Simulator.Shared.Simulations.Tanks.WIPInletSKIDs
{
    public class WIPInletSKID : WIPTank
    {
        public BaseSKID SKID { get; set; } = null!;
        public WIPInletSKID(TankDTO tankDTO) : base(tankDTO)
        {

            TankDTO = tankDTO;

            CalculateForInlet = CalculateInletFromSKID;
            CalculateForOutlet = CalculateFromOutlet;


        }
        public ManageMassWIPTank MassCurrentSKU { get; set; } = new();
        public bool IsTankAchievedMassProduced => Line.IsNextBackBoneSameAsCurrent ? false : MassCurrentSKU.Needed.Value <= 0;

        public WIPSkidState WIPState { get; set; } = null!;

        protected void CalculateFromOutlet()
        {
            CurrentTime+= OneSecond;
            CurrentLevel -= Outlet;
            TotalMassOutlet += Outlet;
            TotalMassOutletShift += Outlet;

            if (CurrentTime.GetValue(TimeUnits.Minute) > 10)
                AverageOutlet = TotalMassOutlet / CurrentTime;
            Outlet.SetValue(0, MassUnits.KiloGram);

        }


        public override void Init()
        {

            CurrentLevel = TankDTO.InitialLevel;
           


        }
        BackBoneSimulation CurrentBackBone = null!;
        public override void InitFromLine()
        {
            if(Line.IsProductionPlanAchieved)
            {
                MassCurrentSKU = new ManageMassWIPTank();
                return;
            }

            SetCurrentMaterialSimulation(Line.CurrentBackBone);
            CurrentBackBone = Line.CurrentBackBone;
            MassCurrentSKU.Needed = Line.PlannedMass - CurrentLevel + TankDTO.LoLoLevel;
            AverageOutlet = Line.AverageMassFlow;
            SKID = (BaseSKID)GetInletAttachedEquipment();
            SKID.SetCurrentMaterialSimulation(CurrentBackBone);

            WIPState = new WIPSkidRunningState(this);
     
        }

        protected void CalculateInletFromSKID()
        {
            WIPState.Calculate(CurrentDate);
            CurrentLevel += Inlet;

            MassCurrentSKU.Needed -= Inlet;

            Inlet.SetValue(0, MassUnits.KiloGram);

        }

        public override void Calculate(DateTime currentdate)
        {
            base.Calculate(currentdate);


        }



        public override void ChangeOver()
        {
            CurrentLevel = new(MassUnits.KiloGram);
            TotalMassOutlet = new(MassUnits.KiloGram);
            CurrentTime = new(TimeUnits.Minute);

            InitFromLine();

        }


    }


}

