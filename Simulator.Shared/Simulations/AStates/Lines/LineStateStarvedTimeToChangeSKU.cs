using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States.Lines;

namespace Simulator.Shared.Simulations.AStates.Lines
{
    public class LineStateStarvedTimeToChangeSKU : LineState
    {
        public LineStateStarvedTimeToChangeSKU(BaseLine line) : base(line)
        {
            StateLabel = "Change Over Washout Starved by Washout pump";
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

            }
            else if (Line.GetWashoutPump() is not null)
            {
                Line.SetLineStateState(new LineStateTimeToChangeSKU(Line));

            }
        }
    }
}
