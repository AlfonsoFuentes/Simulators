using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Washouts;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadWashoutTimes
    {
        public static async Task ReadWashoutTime(this NewSimulationDTO simulation, IServerCrudService service)
        {
            var dto = new WashoutDTO()
            {
                FocusFactory = simulation.FocusFactory
            };
            var rows = await service.GetAllAsync<Washout>(dto, querysuffix: $"{dto.FocusFactory}");


            if (rows != null && rows.Count > 0)
            {

                simulation.WashouTimes = rows.Select(x => x.MapToDto<WashoutDTO>()).ToList();
            }
        }

    }
}
