using Simulator.Server.Databases.Entities.HC;

using Simulator.Server.EndPoints.HCs.Pumps;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationPumps
    {
        public static async Task ReadPumps(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<Pump, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.Pumps.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Pump>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.Pumps = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();
            }





        }
    }
}
