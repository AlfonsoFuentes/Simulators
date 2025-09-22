using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States;

namespace Simulator.Shared.Simulations.States.Lines
{

    public abstract class LineState : EquipmentState
    {
        protected Amount OneSecond = new(1, TimeUnits.Second);




        protected BaseLine Line;
        public LineState(BaseLine line) : base(line)
        {
            Line = (BaseLine)Equipment;

        }


    }

}
