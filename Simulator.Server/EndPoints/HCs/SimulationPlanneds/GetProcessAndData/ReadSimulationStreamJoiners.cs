using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.StreamJoiners;
using Simulator.Shared.Simulations;

namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationStreamJoiners
    {
        public static async Task ReadStreamJoiners(this NewSimulationDTO simulation, IServerCrudService service)
        {
            StreamJoinerDTO dto = new()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<StreamJoiner>(dto, parentId: $"{dto.MainProcessId}");

            if (rows != null && rows.Count > 0)
            {
                simulation.StreamJoiners = rows.Select(x => x.MapToDto<StreamJoinerDTO>()).ToList();
            }


        }
    }
}
