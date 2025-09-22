using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.States.Lines;
using System.Diagnostics.Tracing;

namespace Simulator.Shared.Simulations.AStates.Lines
{
    public class LineStateWipLoLevel : LineState
    {
    
        public LineStateWipLoLevel(BaseLine line) : base(line)
        {
            line.StartEquipmentEvent($"Line Stop by Low Level in {line.WIPTank?.Name}");
            StateLabel = "Starved by WIP Low Level";



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
