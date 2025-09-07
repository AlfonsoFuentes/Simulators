namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateStarvedDuringWIPEmptyByAU : LineState
    {
        public LineStateStarvedDuringWIPEmptyByAU(BaseLine line) : base(line, "Starved By %AU")
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
                return;

            }
            else if (Line.IsTimeStarvedAUAchieved)
            {

                Line.SetLineStateState(new LineStateEmptyMassFromWIP(Line));

            }
        }
    }
}
