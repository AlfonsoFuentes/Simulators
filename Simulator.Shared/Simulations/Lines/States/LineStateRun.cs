namespace Simulator.Shared.Simulations.Lines.States
{
    public class LineStateRun : LineState
    {
        public LineStateRun(BaseLine line) : base(line, "Producing")
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
                Line.SetLineStateState(new LineStateWipLoLevel(Line));
            }
            else if (Line.IsTimeProducingAchieved)
            {
               Line.SetLineStateState(new LineStateStarvedByAU(Line));
            }
            else if(Line.IsPlannedDowTime)
            {
                Line.SetLineStateState(new LineStatePlannedDowTime(Line));
            }
            else if (Line.IsPlannedCasesAchieved)
            {
                if (Line.IsProductionPlanAchieved)
                {

                    SetToEmptyTank();
                    Line.SetLineStateState(new LineStateEmptyMassFromWIP(Line));
                }
                else if (Line.IsNextBackBoneSameAsCurrent)
                {
                    Line.SetLineStateState(new LineStateTimeToChangeSKUSameBackBone(Line));


                }
                else
                {
                    Line.SetLineStateState(new LineStateEmptyMassFromWIP(Line));
                }



            }

        }
        public void SetToEmptyTank()
        {
            Line.WIPTank.SetToEmptyTank();
        }
    }
}
