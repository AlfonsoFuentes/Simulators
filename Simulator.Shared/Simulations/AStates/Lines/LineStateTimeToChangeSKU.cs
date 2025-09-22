using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States.Lines;

namespace Simulator.Shared.Simulations.AStates.Lines
{
    public class LineStateTimeToChangeSKU : LineState
    {
        public LineStateTimeToChangeSKU(BaseLine line) : base(line)
        {
            StateLabel = "Change Over Washout";
            Line.CurrenTime = new(TimeUnits.Minute);
            Line.CurrenChangeOverTime = Line.TimeToChangeSKU;

        }

        public override void Run()
        {
            Line.NotCalculateOneSecond();
            Line.CurrenChangeOverTime -= OneSecond;
        }

        public override void CheckStatus()
        {
            if (!Line.IsPlannedShift)
            {
                Line.SetLineStateState(new LineStateShiftNotPlanned(Line));
                Line.CurrenChangeOverTime = new(TimeUnits.Minute);

            }
            else if (Line.IsTimeChangingSKUAchieved)
            {
                Line.CurrenChangeOverTime = new(TimeUnits.Minute);
                Line.ChangeSku();
                ReleaseWashoutPump();
                Line.WIPChangeOver();
                Line.SetLineStateState(new LineStateWipLoLevel(Line));

            }
        }

        void ReleaseWashoutPump()
        {

            Line.RemoveProcessInletEquipment(Line.Washoutpump);
            Line.Washoutpump = null!;
        }
        public void WipChangeOver()
        {
            
        }
    }
}
