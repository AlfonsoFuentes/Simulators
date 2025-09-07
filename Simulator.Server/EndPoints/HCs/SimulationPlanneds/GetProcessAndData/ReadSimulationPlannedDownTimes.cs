using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.LinePlanneds;
using Simulator.Server.EndPoints.HCs.MixerPlanneds;
using Simulator.Server.EndPoints.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationPlannedDownTimes
    {
        public static async Task ReadPlannedDowntimes(this NewSimulationDTO simulation, IQueryRepository Repository)
        {

            foreach (var row in simulation.AllEquipments)
            {
                await row.ReadPlannedDowntimes(Repository);
            }


        }
        static async Task ReadPlannedDowntimes(this BaseEquipmentDTO equipment, IQueryRepository Repository)
        {
            Expression<Func<EquipmentPlannedDownTime, bool>> Criteria = x => x.BaseEquipmentId == equipment.Id;
            string CacheKey = StaticClass.EquipmentPlannedDownTimes.Cache.GetAll(equipment.Id);
            var rows = await Repository.GetAllAsync<EquipmentPlannedDownTime>(Cache: CacheKey, Criteria: Criteria);

            if (rows != null && rows.Count > 0)
            {
                equipment.PlannedDownTimes = rows.OrderBy(x => x.StartTime).Select(x => x.Map()).ToList();
            }


        }



    }
}
