namespace Simulator.Shared.Simulations.Tanks.WIPInletSKIDs.States
{
    public class WIPSkidRunningState : WIPSkidState
    {
        public WIPSkidRunningState(WIPInletSKID wip) : base(wip, wip.SKID, "Receive from SKID")
        {
            SKID.SetNormalFlowState();
        }

        public override void CalculateWIP(DateTime currentDate)
        {
            SKID.Calculate(currentDate);
           
        }

        public override void CheckState()
        {
            if (WIP.IsTankAchievedMassProduced)
            {
                WIP.WIPState = new WIPSkidRunningStateToEmptyTank(WIP);

            }
            else if(WIP.IsTankHiLevel)
            {
                WIP.WIPState = new WIPSkidHiLevelState(WIP);
                
            }
        }
    }
}
