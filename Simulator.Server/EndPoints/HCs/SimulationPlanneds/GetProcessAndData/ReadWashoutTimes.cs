using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Server.EndPoints.HCs.SKUs;
using Simulator.Server.EndPoints.HCs.Washouts;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadWashoutTimes
    {
        public static async Task ReadWashoutTime(this NewSimulationDTO simulation, IQueryRepository Repository)
        {
            string CacheKey = StaticClass.Washouts.Cache.GetAll;
            var rows = await Repository.GetAllAsync<Washout>(Cache: CacheKey/*, Criteria: Criteria*/);

            if (rows != null && rows.Count > 0)
            {
                
                simulation.WashouTimes = rows.Select(x => x.Map()).ToList();
            }
        }

    }
}
