using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Shared.Models.HCs.BaseEquipments
{

    public class PortConnection
    {
        public BaseEquipmentDTO Equipment { get; private set; }

        public PortConnection(BaseEquipmentDTO equipment)
        {
            Equipment = equipment;
        }
    }
    public class BaseDTO : Dto
    {

        public bool IsExisting { get; set; } = false;
    }
    public class BaseEquipmentDTO : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public override string ToString()
        {
            return Name;
        }
        public BaseEquipmentDTO() { }
        public Guid MainProcessId { get; set; }
        public bool CopyData { get; set; } = true;

        public virtual ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.None;
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        //public List<PortConnection> OutletConnections { get; set; } = new List<PortConnection>();
        //public List<PortConnection> InletConnections { get; set; } = new List<PortConnection>();
      
        //public List<BaseEquipmentDTO> OutletEquipmentConnections { get; set; } = new List<BaseEquipmentDTO>();
        //public List<BaseEquipmentDTO> InletEquipmentConnections { get; set; } = new List<BaseEquipmentDTO>();
        public List<MaterialDTO> Materials => MaterialEquipments == null || MaterialEquipments.Count == 0 ? new() : MaterialEquipments.Select(x => x.Material!).ToList();
        public List<EquipmentPlannedDownTimeDTO> PlannedDownTimes { get; set; } = new();

        public List<InletConnectorDTO> InletConnectors { get; set; } = new();
        public List<OutletConnectorDTO> OutletConnectors { get; set; } = new();

        public List<MaterialEquipmentDTO> MaterialEquipments { get; set; } = new();
    }
    //public class BaseEquipmentList : IResponseAll
    //{
    //    public List<BaseEquipmentDTO?> Items { get; set; } = new();
    //}
    //public class BaseEquipmentGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.BaseEquipments.EndPoint.GetAll;
    //    public Guid MainProcessId { set; get; }
    //    public ProccesEquipmentType ProccesEquipmentType { set; get; }= ProccesEquipmentType.None;
    //}
    
}
