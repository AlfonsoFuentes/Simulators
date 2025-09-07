namespace Simulator.Shared.Simulations.Tanks.WIPInletSKIDs.States
{
    public class WIPSkidStarvedRawMaterialState : WIPSkidState
    {
        public WIPSkidStarvedRawMaterialState(WIPInletSKID wip) : base(wip, wip.SKID, "Starved")
        {
        }

        public override void CalculateWIP(DateTime currentDate)
        {
           
        }

        public override void CheckState()
        {
          
        }
    }
}
