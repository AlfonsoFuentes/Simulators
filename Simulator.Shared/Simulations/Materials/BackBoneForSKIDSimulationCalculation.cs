using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.Skids;

namespace Simulator.Shared.Simulations.Materials
{
    public class BackBoneForSKIDSimulationCalculation
    {
        List<BackBoneStepSimulation> steps = null!;
        List<BackBoneAddSKIDSimulationCalculation> FormulaCalculation { get; set; } = new();

        BaseSKID SKID { get; set; } = null!;

        public BackBoneForSKIDSimulationCalculation(BaseSKID sKID, List<BackBoneStepSimulation> steps)
        {
            SKID = sKID;
            this.steps = steps;
            InitFormula();
        }
        void InitFormula()
        {
            var flowSKID = SKID.SKIDFlow;
            foreach (var step in steps)
            {
                if (step.BackBoneStepType == BackBoneStepType.Add && step.StepRawMaterial != null)
                {
                    var equipment= SKID.GetEquipmentAtInlet(step.StepRawMaterial);
                    if (equipment != null) {
                        if(equipment is BasePump pump)
                        {
                            var flow = flowSKID * step.Percentage / 100;
                            if (pump != null)
                            {
                                BackBoneAddSKIDSimulationCalculation flowcalculation = new(pump, flow);

                                FormulaCalculation.Add(flowcalculation);
                            }
                            else
                            {

                            }
                        }
                    }
                  
                    

                }

            }
        }
        public void CalculateNormalFlow(DateTime dateTime)
        {
           
            FormulaCalculation.ForEach(x => x.Pump.SetInletFlow(x.Flow));
            FormulaCalculation.ForEach(x => x.Pump.CalculateAtachedInlets(dateTime));
        }
        Amount ZeroFlow = new(MassFlowUnits.Kg_hr);
        public void CalculateZeroFlow(DateTime dateTime)
        {
           
            FormulaCalculation.ForEach(x => x.Pump.SetInletFlow(ZeroFlow));
            FormulaCalculation.ForEach(x => x.Pump.CalculateAtachedInlets(dateTime));
        }
    }

}
