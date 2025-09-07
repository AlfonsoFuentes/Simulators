namespace Simulator.Shared.Simulations.Skids.State
{
    public class SkidHiLevelState : SkidState
    {
        public SkidHiLevelState(BaseSKID sKID) : base(sKID, "Starved")
        {
        }

        public override void CalculateSKID(DateTime currentDate)
        {
            CurrentFormula.CalculateZeroFlow(currentDate);
        }


    }
}
