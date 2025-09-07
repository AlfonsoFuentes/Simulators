using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Shared.Simulations.Operators
{
    public class BaseOperator : NewBaseEquipment
    {
        OperatorDTO OperatorDTO { get; set; }
        public override Guid Id => OperatorDTO == null ? Guid.Empty : OperatorDTO.Id;
        public override string Name => OperatorDTO == null ? string.Empty : OperatorDTO.Name;

        public BaseOperator(OperatorDTO operatorDTO) 
        {
            OperatorDTO = operatorDTO;
            EquipmentType = Enums.HCEnums.Enums.ProccesEquipmentType.Operator;

        }

        public override void Init()
        {

        }

        public override void Calculate(DateTime currentdate)
        {

        }

      
    }


}
