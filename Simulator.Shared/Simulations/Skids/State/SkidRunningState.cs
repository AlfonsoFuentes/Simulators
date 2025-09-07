namespace Simulator.Shared.Simulations.Skids.State
{
    public class SkidRunningState : SkidState
    {
        public SkidRunningState(BaseSKID sKID) : base(sKID, "Running")
        {
        }

        public override void CalculateSKID(DateTime currentDate)
        {
            CurrentFormula.CalculateNormalFlow(currentDate);
        }

       
    }
}
