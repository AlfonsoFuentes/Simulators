using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Vml.Office;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.SKULines;

namespace Simulator.Server.Databases.Entities.HC
{
    public static class BaseEquipmentExtensions
    {
        public static void AddInletConnector(this BaseEquipment equipment, List<InletConnectorDTO> inletconnectors)
        {
            foreach (var inletconnector in inletconnectors)
            {
                Conector con = new Conector
                {
                    Id = Guid.NewGuid(),
                    ToId = equipment.Id,
                    MainProcessId = equipment.MainProcessId,
                    FromId = inletconnector.FromId,

                };
                equipment.Froms.Add(con);
            }


        }
        public static void AddOutletConnector(this BaseEquipment equipment, List<OutletConnectorDTO> outletconnectors)
        {
            foreach (var outletconnector in outletconnectors)
            {
                Conector con = new Conector
                {
                    Id = Guid.NewGuid(),
                    ToId = outletconnector.ToId,
                    MainProcessId = equipment.MainProcessId,
                    FromId = equipment.Id,

                };
                equipment.Froms.Add(con);
            }


        }
        public static void AddPlannedDowntime(this BaseEquipment equipment, List<EquipmentPlannedDownTimeDTO> PlannedDownTimes)
        {
            foreach (var planned in PlannedDownTimes)
            {
                EquipmentPlannedDownTime plannedtime = new EquipmentPlannedDownTime
                {
                    Id = Guid.NewGuid(),
                    BaseEquipmentId = equipment.Id,
                    StartTime = planned.StartTime,
                    EndTime = planned.EndTime,
                };
                equipment.PlannedDownTimes.Add(plannedtime);
            }


        }
        public static void AddMaterialEquipment(this BaseEquipment equipment, List<MaterialEquipmentDTO> MaterialEquipments)
        {
            foreach (var planned in MaterialEquipments)
            {
                MaterialEquipment materialequipment = new MaterialEquipment
                {
                    Id = Guid.NewGuid(),
                    ProccesEquipmentId = equipment.Id,
                    MainProcessId = equipment.MainProcessId,
                    MaterialId = planned.MaterialId,
                    CapacityValue = planned.CapacityValue,
                    CapacityUnit = planned.CapacityUnitName,
                    IsMixer = planned.IsMixer,
                };
                equipment.Materials.Add(materialequipment);
            }


        }
        public static void AddLineSKUs(this Line equipment, List<SKULineDTO> LineSKUs)
        {
            foreach (var linesku in LineSKUs)
            {
                SKULine skuLine = new SKULine
                {
                    Id = Guid.NewGuid(),
                    LineId = equipment.Id,
                    SKUId = linesku.SKUId,
                    LineSpeedValue = linesku.LineSpeedValue,
                    LineSpeedUnit = linesku.LineSpeedUnitName,
                    Case_Shift = linesku.Case_Shift,
                };
                equipment.SKULines.Add(skuLine);

            }

        }
    }

}

