using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.BackBoneSteps;
using Simulator.Server.EndPoints.HCs.Lines;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Simulations;
using Simulator.Server.EndPoints.HCs.StreamJoiners;

namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationStreamJoiners
    {
        public static async Task ReadStreamJoiners(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<StreamJoiner, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.StreamJoiners.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<StreamJoiner>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.StreamJoiners = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();
            }



        }
    }
}
