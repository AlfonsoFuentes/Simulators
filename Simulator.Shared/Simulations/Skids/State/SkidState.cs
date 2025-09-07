using Simulator.Shared.Simulations.Materials;

namespace Simulator.Shared.Simulations.Skids.State
{
    public abstract class SkidState
    {
        protected Amount ZeroFlow = new(MassFlowUnits.Kg_hr);
        public string LabelState { get; private set; }
        public abstract void CalculateSKID(DateTime currentDate);
   
        protected BaseSKID SKID { get; set; } = null!;

        protected BackBoneForSKIDSimulationCalculation CurrentFormula { get; set; } = null!;
        public SkidState(BaseSKID sKID, string labelState)
        {

            SKID = sKID;
            LabelState = labelState;
            CurrentFormula = SKID.CurrentFormula;
          

        }
        public void Calculate(DateTime dateTime)
        {
            CalculateSKID(dateTime);
            
        }
    }
}
