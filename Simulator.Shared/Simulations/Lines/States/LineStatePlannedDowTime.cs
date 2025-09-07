namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStatePlannedDowTime : LineState
    {
        public LineStatePlannedDowTime(BaseLine line) : base(line, $"Planned Down time: {line.CurrentPlannedDowntimeName}")
        {
            Line.CurrenTime = new(1, TimeUnits.Second);
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
            else if (Line.IsPlannedDowntimeAchieved)
            {

                Line.SetLineStateState(new LineStateRun(Line));

            }
        }
    }
}
