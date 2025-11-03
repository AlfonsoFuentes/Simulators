using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationMaterialEquipments
    {
        public static async Task ReadMaterialEquipments(this NewSimulationDTO simulation, IServerCrudService service)
        {
            MaterialEquipmentDTO dto = new()
            {
                MainProcessId = simulation.Id,
            };
            var rows = await service.GetAllAsync<MaterialEquipment>(dto, parentId: $"{dto.MainProcessId}");

           
            if (rows != null && rows.Count > 0)
            {
                rows = rows.Where(x => x.ProccesEquipmentId != Guid.Empty && x.MaterialId != Guid.Empty).ToList();
                simulation.MaterialEquipments = rows.Select(x => x.Map(simulation)).ToList();
            }
        }
        public static MaterialEquipmentRecord Map(this Databases.Entities.HC.MaterialEquipment entity, NewSimulationDTO simulation)
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
