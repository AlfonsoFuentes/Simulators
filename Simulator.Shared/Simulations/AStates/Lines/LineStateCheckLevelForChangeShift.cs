using Simulator.Shared.Simulations.AStates.Lines;
using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations.States.Lines
{
    public class LineStateCheckLevelForChangeShift : LineState
    {
        public LineStateCheckLevelForChangeShift(BaseLine line) : base(line )
        {
            StateLabel = "Review/Start next Shift";
        }

        public override void Run()
        {

            Line.NotCalculateOneSecond();

        }

        public override void CheckStatus()
        {
            if (Line.IsPlannedShift)
            {
                Line.SetLineStateState(new LineStateWipLoLevel(Line));


            }
        }
    }
}
