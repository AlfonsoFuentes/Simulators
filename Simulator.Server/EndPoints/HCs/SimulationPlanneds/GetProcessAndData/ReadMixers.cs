
using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Mixers;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationMixers
    {
        public static async Task ReadMixers(this NewSimulationDTO simulation, IServerCrudService service)
        {
            MixerDTO dto = new ()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<Mixer>(dto, parentId: $"{dto.MainProcessId}");

            if (rows != null && rows.Count > 0)
            {
                simulation.Mixers = rows.Select(x => x.MapToDto<MixerDTO>()).ToList();

            }



        }
    }
}
