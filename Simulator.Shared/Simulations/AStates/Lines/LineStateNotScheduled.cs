using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations.States.Lines
{
    public class LineStateNotScheduled : LineState
    {
        public LineStateNotScheduled(BaseLine line) : base(line )
        {
            StateLabel = "Line Not Planned";
        }

        public override void Run()
        {
           
        }

        public override void CheckStatus()
        {
           
        }
    }
}
