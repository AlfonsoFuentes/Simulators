using Simulator.Server.Databases.Entities.HC;

using Simulator.Server.EndPoints.HCs.Pumps;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationPumps
    {
        public static async Task ReadPumps(this NewSimulationDTO simulation, IServerCrudService service)
        {
            PumpDTO dto = new()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<Pump>(dto, parentId: $"{dto.MainProcessId}");

            if (rows != null && rows.Count > 0)
            {
                simulation.Pumps = rows.Select(x => x.MapToDto<PumpDTO>()).ToList();

            }



        }
    }
}
