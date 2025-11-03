using Simulator.Server.Databases.Entities.HC;

using Simulator.Server.EndPoints.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationSKIDs
    {
        public static async Task ReadSkids(this NewSimulationDTO simulation, IServerCrudService service)
        {
            ContinuousSystemDTO dto = new()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<ContinuousSystem>(dto, parentId: $"{dto.MainProcessId}");

            if (rows != null && rows.Count > 0)
            {
                simulation.Skids = rows.Select(x => x.MapToDto<ContinuousSystemDTO>()).ToList();

            }



        }
    }
}
