using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Operators;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.SimulationResults.SKids;
using Simulator.Shared.Simulations.Skids.State;
using Simulator.Shared.Simulations.Tanks.WIPInletSKIDs;

namespace Simulator.Shared.Simulations.Skids
{
    public class BaseSKID : NewBaseEquipment
    {
        public override Guid Id => SKIDDTO == null ? Guid.Empty : SKIDDTO.Id;
        public override string Name => SKIDDTO == null ? string.Empty : SKIDDTO.Name;
        ContinuousSystemDTO SKIDDTO { get; set; } = null!;

        public Amount SKIDFlow => SKIDDTO.Flow;
        public string LabelState => SkidState == null ? "Not initiated" : SkidState.LabelState;
        SkidState SkidState { get; set; } = null!;
        public Amount CurrentFlow { get; set; } = new(MassFlowUnits.Kg_hr);
        public BackBoneForSKIDSimulationCalculation CurrentFormula { get; set; } = null!;
        public BaseSKID(ContinuousSystemDTO continuousSystemDTO)
        {
            SKIDDTO = continuousSystemDTO;
           
            EquipmentType = Enums.HCEnums.Enums.ProccesEquipmentType.ContinuousSystem;

        }
        public BackBoneSimulation BackBoneSimulation { get; set; } = null!;
        public string Producingto => WIPTank == null ? string.Empty : WIPTank.Name;
        
       
        public override void SetCurrentMaterialSimulation(MaterialSimulation materialSimulation)
        {
            base.SetCurrentMaterialSimulation(materialSimulation);
            BackBoneSimulation = (BackBoneSimulation)materialSimulation;

            if(BackBoneSimulation.FlowDataSKID.ContainsKey(this))
            {
                CurrentFormula = BackBoneSimulation.FlowDataSKID[this];
                SkidState = new SkidRunningState(this);
            }
            else
            {
                SkidState = new SkidAvailableState(this);
            }
        
            
        }

        WIPInletSKID WIPTank => (WIPInletSKID)GetEquipmentAtOutlet();


        DateTime CurrentDate;
        public override void Calculate(DateTime currentdate)
        {
            CurrentDate = currentdate;
            SkidState.Calculate(currentdate);
            WIPTank.SetInletFlow(CurrentFlow);


        }

        public void SetNormalFlowState()
        {

            CurrentFlow = SKIDFlow;
            SkidState = new SkidRunningState(this);
            StorageDataToSimulation();

        }
        Amount ZeroFlow = new(MassFlowUnits.Kg_hr);
        public void SetZeroFlowState()
        {

            CurrentFlow = ZeroFlow;
            SkidState = new SkidHiLevelState(this);
            StorageDataToSimulation();
        }
        List<SkidResult> SkidResult = new();
        public void StorageDataToSimulation()
        {

            SkidResult.Add(new()
            {
                CurrentDate = CurrentDate,
                BackBoneSimulation = BackBoneSimulation,
                Flow = CurrentFlow,
                SkidState = SkidState,
                WIPTank = WIPTank,


            });
        }
        public bool HasExcelresult => SkidResult.Count > 0;
        
    }


}
