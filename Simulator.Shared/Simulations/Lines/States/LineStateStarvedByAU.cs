namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateStarvedByAU : LineState
    {
        public LineStateStarvedByAU(BaseLine line) : base(line, "Starved By %AU")
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
            if (Line.IsTimeStarvedAUAchieved)
            {
                
                Line.SetLineStateState(new LineStateRun(Line));

            }
            else if (Line.IsPlannedDowTime)
            {
                Line.SetLineStateState(new LineStatePlannedDowTime(Line));
            }

        }
    }
}
