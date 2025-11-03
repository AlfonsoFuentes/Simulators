using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Server.EndPoints.HCs.Operators;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationOperators
    {
        public static async Task ReadOperators(this NewSimulationDTO simulation, IServerCrudService service)
        {
            OperatorDTO dto = new ()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<Operator>(dto, parentId: $"{dto.MainProcessId}");

            if (rows != null && rows.Count > 0)
            {
                simulation.Operators = rows.Select(x => x.MapToDto<OperatorDTO>()).ToList();
            }


        }
    }
}
