using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSKULiness
    {
        public static async Task ReadSkuLinesSimulation(this NewSimulationDTO simulation, IServerCrudService service)
        {
            foreach (var line in simulation.Lines)
            {
                var result = await GetAllSKULines(line.Id, service);
                if (result != null && result.Count > 0)
                {
                    simulation.SKULines.AddRange(result);
                }
            }

        }

        public static async Task<List<SKULineDTO>> GetAllSKULines(Guid LineId, IServerCrudService service)
        {
            SKULineDTO dto = new()
            {
                LineId = LineId
            };

            var rows = await service.GetAllAsync<SKULine>(dto, parentId: $"{dto.LineId}");
            

            if (rows == null)
            {
                return null!;
            }

            var maps = rows.Select(x => x.MapToDto< SKULineDTO>()).ToList();
            return maps;
        }
    }
}
