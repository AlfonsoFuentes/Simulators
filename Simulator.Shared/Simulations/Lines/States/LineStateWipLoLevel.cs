using System.Diagnostics.Tracing;

namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateWipLoLevel : LineState
    {
    
        public LineStateWipLoLevel(BaseLine line) : base(line, "Starved by WIP Low Level")
        {
            line.StartEquipmentEvent($"Line Stop by Low Level in {line.WIPTank?.Name}");

           
            

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

            }
            else if (!Line.IsWipTankLoLevel)
            {

                Line.SetLineStateState(new LineStateRun(Line));
                Line.CloseCurrentEvent();
            



            }
            else if (Line.IsProductionPlanAchieved)
            {
                Line.SetLineStateState(new LineStateNotScheduled(Line));
            }
        }
    }
}
