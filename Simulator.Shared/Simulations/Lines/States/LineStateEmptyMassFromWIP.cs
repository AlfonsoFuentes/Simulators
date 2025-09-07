namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateEmptyMassFromWIP : LineState
    {
        public LineStateEmptyMassFromWIP(BaseLine line) : base(line, "Change Over to Clean WIP")
        {
        }

        public override void Run()
        {
            
            Line.CalculateOneSecond();
        }

        protected override void CheckStatus()
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
