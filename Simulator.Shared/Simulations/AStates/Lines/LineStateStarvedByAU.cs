using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States.Lines;

namespace Simulator.Shared.Simulations.AStates.Lines
{
    public class LineStateStarvedByAU : LineState
    {
        public LineStateStarvedByAU(BaseLine line) : base(line )
        {
            StateLabel = "Starved By %AU";
        }

        public override void Run()
        {
           
            Line.NotCalculateOneSecond();
      
        }

        public override void CheckStatus()
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
