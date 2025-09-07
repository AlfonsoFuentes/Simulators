namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateShiftNotPlanned : LineState
    {
        public LineStateShiftNotPlanned(BaseLine line) : base(line, "Shift Not Planned")
        {
        }

        public override void Run()
        {
           
        }

        protected override void CheckStatus()
        {
            if (!Line.IsPlannedShift  )
            {
                if( Line.IsNextShiftPlanned&&Line.IsWIPTankAbleToInitNextShifProduction)
                {
                    Line.SetLineStateState(new LineStateCheckLevelForChangeShift(Line));
                }
               

            }
            else
            {
                Line.SetLineStateState(new LineStateRun(Line));
            }
        }
    }
}
