namespace Simulator.Shared.Simulations.Tanks.WIPInletSKIDs.States
{
    public class WIPSkidHiLevelState : WIPSkidState
    {
        public WIPSkidHiLevelState(WIPInletSKID wip) : base(wip, wip.SKID, "Hi Level WIP Tank")
        {
            SKID.SetZeroFlowState();
        }

        public override void CalculateWIP(DateTime currentDate)
        {
            SKID.Calculate(currentDate);

        }

        public override void CheckState()
        {
            if(WIP.IsTankAchievedMassProduced)
            {
                WIP.WIPState = new WIPSkidRunningStateToEmptyTank(WIP);
              
            }
            else if (WIP.IsTankMinLevel)
            {
                WIP.WIPState = new WIPSkidRunningState(WIP);

            }
           
        }
    }
}
