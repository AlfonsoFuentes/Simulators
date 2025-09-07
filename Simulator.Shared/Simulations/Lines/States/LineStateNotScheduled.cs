namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateNotScheduled : LineState
    {
        public LineStateNotScheduled(BaseLine line) : base(line, "Line Not Planned")
        {
        }

        public override void Run()
        {
           
        }

        protected override void CheckStatus()
        {
           
        }
    }
}
