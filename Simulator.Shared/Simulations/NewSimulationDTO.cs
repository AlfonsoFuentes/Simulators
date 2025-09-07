using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Shared.Simulations
{
    public class NewSimulationDTO : BaseResponse, IMessageResponse, IRequest
    {
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.CreateUpdate;
        public string ActionType => "Get simulation";
        public string Legend => Name;
        public string ClassName => StaticClass.SimulationPlanneds.ClassName;
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);

        public List<MaterialDTO> Materials { get; set; } = new();
        public List<SKUDTO> SKUs { get; set; } = new();
        public List<WashoutDTO> WashouTimes { get; set; } = new();
        public List<LineDTO> Lines { get; set; } = new();
        public List<SKULineDTO> SKULines { get; set; } = new();
        public List<TankDTO> Tanks { get; set; } = new();
        public List<MixerDTO> Mixers { get; set; } = new();
        public List<PumpDTO> Pumps { get; set; } = new();
        public List<ContinuousSystemDTO> Skids { get; set; } = new();
        public List<OperatorDTO> Operators { get; set; } = new();
        public List<MaterialEquipmentRecord> MaterialEquipments { get; set; } = new();
        public List<ConnectorRecord> Connectors { get; set; } = new();
        public List<BaseEquipmentDTO> AllEquipments => [.. Lines, .. Tanks, .. Mixers, .. Pumps, .. Skids, .. Operators];


    }
}
