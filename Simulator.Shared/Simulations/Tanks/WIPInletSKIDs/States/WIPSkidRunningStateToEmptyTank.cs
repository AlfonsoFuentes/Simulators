namespace Simulator.Shared.Simulations.Tanks.WIPInletSKIDs.States
{
    public class WIPSkidRunningStateToEmptyTank : WIPSkidState
    {
        public WIPSkidRunningStateToEmptyTank(WIPInletSKID wip) : base(wip, wip.SKID, "Emptying WIP Tank")
        {
            SKID.SetZeroFlowState();
        }

        public override void CalculateWIP(DateTime currentDate)
        {
            SKID.Calculate(currentDate);

        }

        public override void CheckState()
        {
            if (WIP.IsTankLoLevel)
            {
               

            }
        }
    }
}
