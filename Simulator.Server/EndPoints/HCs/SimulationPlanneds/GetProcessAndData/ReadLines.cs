using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationLines
    {
        public static async Task ReadLines(this NewSimulationDTO simulation, IServerCrudService service)
        {
            LineDTO dto=new LineDTO
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<Line>(dto,parentId:$"{dto.MainProcessId}");
           

            if (rows != null && rows.Count > 0)
            {
                simulation.Lines = rows.Select(x => x.MapToDto<LineDTO>()).ToList();
            }



        }
        
    }
}
