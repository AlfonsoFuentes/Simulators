using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States.Lines;

namespace Simulator.Shared.Simulations.AStates.Lines
{
    public class LineStateStarvedDuringWIPEmptyByAU : LineState
    {
        public LineStateStarvedDuringWIPEmptyByAU(BaseLine line) : base(line )
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
            else if (Line.IsTimeStarvedAUAchieved)
            {

                Line.SetLineStateState(new LineStateEmptyMassFromWIP(Line));

            }
        }
    }
}
