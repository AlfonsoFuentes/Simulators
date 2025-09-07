using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Server.EndPoints.HCs.SKULines;
using Simulator.Server.EndPoints.HCs.SKUs;
using Simulator.Server.EndPoints.HCs.Washouts;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSKUs
    {
        public static async Task ReadSkuSimulation(this NewSimulationDTO simulation, IQueryRepository Repository)
        {
            Func<IQueryable<SKU>, IIncludableQueryable<SKU, object>> includes = x => x
                   .Include(y => y.Material!);

            string CacheKey = StaticClass.SKUs.Cache.GetAll;
            var rows = await Repository.GetAllAsync<SKU>(Cache: CacheKey, Includes: includes);

            if (rows != null && rows.Count > 0)
            {
                simulation.SKUs = rows.Select(x => x.Map()).ToList();
            }
        }

    }
}
