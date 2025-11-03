using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationPlannedDownTimes
    {
        public static async Task ReadPlannedDowntimes(this NewSimulationDTO simulation, IServerCrudService service)
        {
            EquipmentPlannedDownTimeDTO dto = new()
            {
                MainProcessId = simulation.Id
            };
            var rows = await service.GetAllAsync<EquipmentPlannedDownTime>(dto, parentId: $"{dto.MainProcessId}");


           
            simulation.AllEquipments.ForEach(equi =>
            {
                equi.PlannedDownTimes = rows.Where(x => x.BaseEquipmentId == equi.Id).Select(y => y.MapToDto<EquipmentPlannedDownTimeDTO>()).ToList();
            });


        }
        static async Task ReadPlannedDowntimes(this BaseEquipmentDTO equipment, IServerCrudService service)
        {
            EquipmentPlannedDownTimeDTO dto = new()
            {
                BaseEquipmentId = equipment.Id
            };
            var rows = await service.GetAllAsync<EquipmentPlannedDownTime>(dto, parentId: $"{dto.BaseEquipmentId}");


            if (rows != null && rows.Count > 0)
            {
                equipment.PlannedDownTimes = rows.Select(x => x.MapToDto<EquipmentPlannedDownTimeDTO>()).ToList();
            }


        }



    }
}
