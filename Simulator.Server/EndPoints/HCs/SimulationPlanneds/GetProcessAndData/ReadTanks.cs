
using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationTanks
    {
        public static async Task ReadTanks(this NewSimulationDTO simulation, IServerCrudService service)
        {
            TankDTO dto = new()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<Tank>(dto, parentId: $"{dto.MainProcessId}");

            if (rows != null && rows.Count > 0)
            {
                simulation.Tanks = rows.Select(x => x.MapToDto<TankDTO>()).ToList();

            }



        }
    }
}
