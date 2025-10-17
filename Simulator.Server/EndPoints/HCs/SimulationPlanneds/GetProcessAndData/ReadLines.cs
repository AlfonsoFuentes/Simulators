using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.BackBoneSteps;
using Simulator.Server.EndPoints.HCs.Lines;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationLines
    {
        public static async Task ReadLines(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<Line, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.Lines.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Line>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.Lines = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();
            }



        }
        
    }
}
