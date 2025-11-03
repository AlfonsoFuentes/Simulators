using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.SKUs;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSKUs
    {
        public static async Task ReadSkuSimulation(this NewSimulationDTO simulation, IServerCrudService service)
        {
            SKUDTO dto = new SKUDTO()
            {
                FocusFactory = simulation.FocusFactory
            };
            var rows = await service.GetAllAsync<SKU>(dto, querysuffix: $"{dto.FocusFactory}");

           

            if (rows != null && rows.Count > 0)
            {
                simulation.SKUs = rows.Select(x => x.MapToDto<SKUDTO>()).ToList();
            }
        }

    }
}
