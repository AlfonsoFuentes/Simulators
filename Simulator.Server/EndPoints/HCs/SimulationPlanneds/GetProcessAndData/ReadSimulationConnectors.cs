using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationConnectors
    {
        public static async Task ReadConnectors(this NewSimulationDTO simulation, IServerCrudService service)
        {
            ConectorDTO dto = new ConectorDTO()
            {
                MainProcessId = simulation.Id
            };

            var rows= await service.GetAllAsync<Conector>(dto, parentId: $"{simulation.Id}");
          
            if (rows != null && rows.Count > 0)
            {
                simulation.Connectors = rows.Select(x => x.Map(simulation)).ToList();
            }
        }
        public static ConnectorRecord Map(this Conector entity, NewSimulationDTO simulation)
        {
            return new ConnectorRecord(entity.FromId, entity.ToId);
            
        }
    }
}
