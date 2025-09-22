using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States.Lines;

namespace Simulator.Shared.Simulations.AStates.Lines
{
    public class LineStateShiftNotPlanned : LineState
    {
        public LineStateShiftNotPlanned(BaseLine line) : base(line)
        {
            StateLabel = "Shift Not Planned";
        }

        public override void Run()
        {
           
        }

        public override void CheckStatus()
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
