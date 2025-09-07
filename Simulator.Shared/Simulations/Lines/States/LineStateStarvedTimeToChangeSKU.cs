namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateStarvedTimeToChangeSKU : LineState
    {
        public LineStateStarvedTimeToChangeSKU(BaseLine line) : base(line, "Change Over Washout Starved by Washout pump")
        {
        }

        public override void Run()
        {
            Line.NotCalculateOneSecond();
        }

        protected override void CheckStatus()
        {
            if (!Line.IsPlannedShift)
            {
                Line.SetLineStateState(new LineStateShiftNotPlanned(Line));

            }
            else if (Line.GetWashoutPump() is not null)
            {
                Line.SetLineStateState(new LineStateTimeToChangeSKU(Line));

            }
        }
    }
}
