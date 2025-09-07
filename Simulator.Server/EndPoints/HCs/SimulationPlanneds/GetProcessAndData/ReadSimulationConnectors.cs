using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationConnectors
    {
        public static async Task ReadConnectors(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {

            Expression<Func<Conector, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.Conectors.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Conector>(Cache: CacheKey, Criteria: Criteria);
            if (rows != null && rows.Count > 0)
            {
                simulation.Connectors = rows.Select(x => x.Map(simulation)).ToList();
            }
        }
        public static ConnectorRecord Map(this Conector entity, NewSimulationDTO simulation)
        {
            return new ConnectorRecord(entity.FromId, entity.ToId);
            //{
            //    MainProcessId = entity.MainProcessId,
            //    From = simulation.AllEquipments.FirstOrDefault(x => x.Id == entity.FromId),
            //    To = simulation.AllEquipments.FirstOrDefault(x => x.Id == entity.ToId),
            //    FromId = entity.FromId,
            //    ToId = entity.ToId,

            //};
        }
    }
}
