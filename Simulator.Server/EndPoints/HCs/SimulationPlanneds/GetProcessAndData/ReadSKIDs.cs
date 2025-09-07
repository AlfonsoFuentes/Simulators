using Simulator.Server.Databases.Entities.HC;

using Simulator.Server.EndPoints.HCs.ContinuousSystems;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationSKIDs
    {
        public static async Task ReadSkids(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<ContinuousSystem, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.ContinuousSystems.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<ContinuousSystem>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.Skids = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();
            }







        }
    }
}
