using Simulator.Shared.Simulations.AStates.Lines;
using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations.States.Lines
{
    public class LineStateEmptyMassFromWIP : LineState
    {
        public LineStateEmptyMassFromWIP(BaseLine line) : base(line )
        {
            StateLabel = "Change Over to Clean WIP";
        }

        public override void Run()
        {
            
            Line.CalculateOneSecond();
        }

        public override void CheckStatus()
        {
            if (!Line.IsPlannedShift)
            {
                Line.SetLineStateState(new LineStateShiftNotPlanned(Line));

            }
            else if (Line.IsWipTankLoLevel)
            {
               
                if (Line.GetWashoutPump() is null)
                {
                    Line.SetLineStateState(new LineStateStarvedTimeToChangeSKU(Line));
                }
                else
                {
                    Line.SetLineStateState(new LineStateTimeToChangeSKU(Line));
                }
              
            }
            else if(Line.IsTimeProducingAchieved)
            {
                Line.SetLineStateState(new LineStateStarvedDuringWIPEmptyByAU(Line));
            }
        }
    }
}
