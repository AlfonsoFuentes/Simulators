using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Tanks;

namespace Simulator.Shared.Simulations.Pumps
{
    public class BasePump : NewBaseEquipment
    {
        public override Guid Id => PumpDTO == null ? Guid.Empty : PumpDTO.Id;
        public override string Name => PumpDTO == null ? string.Empty : PumpDTO.Name;
        protected PumpDTO PumpDTO { get; set; }
        public int MaxNumberEquipmentToWash { get; set; } = 2;
        public BaseLine Line { get; set; } = null!;
        public Amount Flow => PumpDTO.Flow;
        public string FlowString => Flow.GetValue(MassFlowUnits.Kg_hr).ToString();

        

        private bool _materialsCached = false;
        List<MaterialSimulation> _InletMaterials = null!;
       
        public List<MaterialSimulation> InletMaterials
        {
            get
            {
                if (!_materialsCached)
                {
                    
                    _InletMaterials = ConnectedInletEquipments.SelectMany(x=>x.MaterialSimulations).ToList();
                    _materialsCached = true;
                }
                return _InletMaterials;
            }
        }
        public BasePump(PumpDTO pumpDTO)
        {
            PumpDTO = pumpDTO;
            EquipmentType = Enums.HCEnums.Enums.ProccesEquipmentType.Pump;

        }


        public override void Init()
        {
            InitInletConnectedEquipment();

        }





        public override void Calculate(DateTime currentdate)
        {
            foreach (var row in ConnectedInletEquipments)
            {
                if (row is RawMaterialTank)
                {
                    row.Calculate(currentdate);
                }

            }


        }
        Amount SKIDflow = null!;
        public void SetSKIDFlow(Amount flow)
        {
            SKIDflow = flow;



        }
        public void SetNormalSKIDFlow()
        {
            Amount flow = SKIDflow;
            foreach (var InletEquipment in ConnectedInletEquipments)
            {
                if (InletEquipment is BaseTank tank)
                {

                    tank.SetOutletFlow(flow);
                }
            }



        }
        public void SetZeroFlow()
        {
            Amount flow = new(MassFlowUnits.Kg_hr);
            foreach (var InletEquipment in ConnectedInletEquipments)
            {
                if (InletEquipment is BaseTank tank)
                {

                    tank.SetOutletFlow(flow);
                }
            }



        }
        public void SetInletFlow(Amount flow)
        {
            foreach (var InletEquipment in ConnectedInletEquipments)
            {
                if (InletEquipment is BaseTank tank)
                {

                    tank.SetOutletFlow(flow);
                }
            }

        }
        public void SetOutletFlow(Amount flow)
        {
            foreach (BaseTank row in ConnectedOutletEquipments)
            {
                row.SetInletFlow(flow);
            }


        }



    }


}
