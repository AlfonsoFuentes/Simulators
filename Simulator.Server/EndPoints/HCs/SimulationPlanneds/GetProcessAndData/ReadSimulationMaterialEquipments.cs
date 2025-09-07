using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.MixerPlanneds;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationMaterialEquipments
    {
        public static async Task ReadMaterialEquipments(this NewSimulationDTO simulation, Guid MainProcessId, IQueryRepository Repository)
        {

            Expression<Func<Databases.Entities.HC.MaterialEquipment, bool>> Criteria = x => x.MainProcessId == MainProcessId;
            string CacheKey = StaticClass.MaterialEquipments.Cache.GetAll(MainProcessId);
            var rows = await Repository.GetAllAsync<Databases.Entities.HC.MaterialEquipment>(Cache: CacheKey, Criteria: Criteria);
            if (rows != null && rows.Count > 0)
            {
                rows = rows.Where(x => x.ProccesEquipmentId != Guid.Empty && x.MaterialId != Guid.Empty).ToList();
                simulation.MaterialEquipments = rows.Select(x => x.Map(simulation)).ToList();
            }
        }
        public static Shared.Models.HCs.MaterialEquipments.MaterialEquipmentRecord Map(this Databases.Entities.HC.MaterialEquipment entity, NewSimulationDTO simulation)
        {
            return new MaterialEquipmentRecord()
            {
                MaterialId = entity.MaterialId,
                EquipmentId = entity.ProccesEquipmentId,
                CapacityValue = entity.CapacityValue,
                CapacityUnitName = entity.CapacityUnit,
            };

        }
    }
}
