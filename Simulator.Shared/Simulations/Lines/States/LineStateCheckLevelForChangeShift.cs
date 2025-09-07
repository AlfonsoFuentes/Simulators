namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateCheckLevelForChangeShift : LineState
    {
        public LineStateCheckLevelForChangeShift(BaseLine line) : base(line, "Review/Start next Shift")
        {
        }

        public override void Run()
        {

            Line.NotCalculateOneSecond();

        }

        protected override void CheckStatus()
        {
            if (Line.IsPlannedShift)
            {
                Line.SetLineStateState(new LineStateWipLoLevel(Line));


            }
        }
    }
}
