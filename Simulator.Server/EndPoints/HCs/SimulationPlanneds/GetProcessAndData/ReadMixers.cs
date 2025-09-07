
using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Simulations;
using Simulator.Server.EndPoints.HCs.Mixers;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationMixers
    {
        public static async Task ReadMixers(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<Mixer, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.Mixers.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Mixer>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.Mixers = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();

            }



        }
    }
}
