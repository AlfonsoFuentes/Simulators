using System.Diagnostics.Tracing;

namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateWipLoLevel : LineState
    {
    
        public LineStateWipLoLevel(BaseLine line) : base(line, "Starved by WIP Low Level")
        {
            var eventId = line.StartEquipmentEvent(
             "LineStop",  // Tipo de evento
             "WIP Low Level",  // Razón específica
             $"Line {line.Name} stopped due to low WIP level",
             $"WIP Tank: {line.WIPTank?.Name ?? "Unknown"} is below minimum level",
             "Error"
         );

            // Guardar el ID en el contexto para usarlo al salir
            line.CurrentEventId = eventId;

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
                if (!string.IsNullOrEmpty(Line.CurrentEventId))
                {
                    Line.EndEquipmentEvent(
                        Line.CurrentEventId,
                        "WIP Level Restored",
                        $"Line {Line.Name} resumed operation - WIP tank level restored at {Line.Simulation?.CurrentDate:HH:mm:ss}"
                    );

                    Line.CurrentEventId = null!;
                }



            }
            else if (Line.IsProductionPlanAchieved)
            {
                Line.SetLineStateState(new LineStateNotScheduled(Line));
            }
        }
    }
}
