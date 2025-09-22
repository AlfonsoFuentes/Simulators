using Simulator.Shared.Simulations.AStates.Lines;
using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations.States.Lines
{
    public class LineStatePlannedDowTime : LineState
    {
        public LineStatePlannedDowTime(BaseLine line) : base(line )
        {
            StateLabel = $"Planned Down time: {line.CurrentPlannedDowntimeName}";
            Line.CurrenTime = new(1, TimeUnits.Second);
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
            else if (Line.IsPlannedDowntimeAchieved)
            {

                Line.SetLineStateState(new LineStateRun(Line));

            }
        }
    }
}
