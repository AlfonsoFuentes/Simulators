using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.MaterialEquipments;
using Simulator.Server.EndPoints.HCs.Operators;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationOperators
    {
        public static async Task ReadOperators(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {
            Expression<Func<Operator, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.Operators.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Operator>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                simulation.Operators = rows.OrderBy(x => x.Name).Select(x => x.Map()).ToList();
            }


        }
    }
}
