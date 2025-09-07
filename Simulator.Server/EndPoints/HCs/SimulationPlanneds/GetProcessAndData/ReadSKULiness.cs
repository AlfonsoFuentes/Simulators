using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Server.EndPoints.HCs.SKULines;
using Simulator.Server.EndPoints.HCs.SKUs;
using Simulator.Server.EndPoints.HCs.Washouts;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSKULiness
    {
        public static async Task ReadSkuLinesSimulation(this NewSimulationDTO simulation, IQueryRepository Repository)
        {
            foreach (var line in simulation.Lines)
            {
                var result = await GetAllSKULines(line.Id, Repository);
                if (result != null && result.Count > 0)
                {
                    simulation.SKULines.AddRange(result);
                }
            }

        }

        public static async Task<List<SKULineDTO>> GetAllSKULines(Guid LineId, IQueryRepository Repository)
        {
            Func<IQueryable<SKULine>, IIncludableQueryable<SKULine, object>> includes = x => x
                    .Include(y => y.SKU).ThenInclude(x => x.Material)
                    ;
            Expression<Func<SKULine, bool>> Criteria = x => x.LineId == LineId;
            string CacheKey = StaticClass.SKULines.Cache.GetAll(LineId);
            var rows = await Repository.GetAllAsync<SKULine>(Cache: CacheKey, Includes: includes, Criteria: Criteria);

            if (rows == null)
            {
                return null!;
            }

            var maps = rows.Select(x => x.Map()).ToList();
            return maps;
        }
    }
}
