
using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Tanks;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationTanks
    {
        public static async Task ReadTanks(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<Tank, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.Tanks.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Tank>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.Tanks = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();
            }


        }
    }
}
