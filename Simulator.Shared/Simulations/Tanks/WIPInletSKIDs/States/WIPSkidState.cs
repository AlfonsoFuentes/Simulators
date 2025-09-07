using Simulator.Shared.Simulations.Skids;

namespace Simulator.Shared.Simulations.Tanks.WIPInletSKIDs.States
{
    public abstract class WIPSkidState
    {
        protected Amount ZeroFlow = new(MassFlowUnits.Kg_hr);
        public string LabelState { get; private set; }
        public abstract void CalculateWIP(DateTime currentDate);
        public abstract void CheckState();

        protected WIPInletSKID WIP { get; private set; } = null!;

        protected BaseSKID SKID { get; private set; } = null!;

        public WIPSkidState(WIPInletSKID Wip, BaseSKID sKID, string labelState)
        {


            LabelState = labelState;
            WIP = Wip;
            SKID = sKID;
        }
        public void Calculate(DateTime dateTime)
        {
            CalculateWIP(dateTime);
            CheckState();
        }
    }
}
