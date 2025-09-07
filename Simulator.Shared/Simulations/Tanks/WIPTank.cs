using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.Mixers;

namespace Simulator.Shared.Simulations.Tanks
{
    public class WIPTank : BaseTank
    {

        public List<NewBaseEquipment> ManufacturingEquipments =>
       ProcessInletEquipments.Select(x => x.ProcessInletEquipments.First()).ToList();

        protected PlannedSKUSimulation NextPlannedSKU { get; set; } = null!;
        protected PlannedSKUSimulation CurrentPlannedSKU { get; set; } = null!;

        public BaseLine Line { get; set; } = null!;

        public WIPTank(TankDTO tankDTO) : base(tankDTO)
        {
        }
        public virtual void InitFromLine()
        {

        }
        public void SetLine(BaseLine line)
        {
            Line = line;
        }
        public virtual bool ChechLevelForShiftChange()
        {

            return false;

        }
        public virtual void NewChangeSkuLine()
        {

        }

        public virtual void ChangeSkuLine()
        {

        }
        public void InitNextShift()
        {
            TotalMassOutletShift = new(MassUnits.KiloGram);
        }

        public virtual void ChangeOver()
        {

        }
        public virtual void SetInitFromMixer(BaseMixer mixer)
        {

        }
        public void SetToEmptyTank()
        {


            CalculateForInlet = () => { };
        }

        public override bool GetTankAvailableforTransferMixer(BaseMixer mixer)
        {
            return false;
        }
    }


}
