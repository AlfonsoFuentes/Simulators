namespace Simulator.Shared.Simulations
{
    public static class SimulationMappers
    {
        public static NewSimulation MapProcess(this NewSimulationDTO DTO)
        {
            NewSimulation result = new NewSimulation();


            result.MapMaterials(DTO.Materials);
            result.MapSKUS(DTO.SKUs);
            result.MapWashouts(DTO.WashouTimes);
            result.AddLines(DTO.Lines);
            result.AddTanks(DTO.Tanks);
            result.AddMixers(DTO.Mixers);
            result.AddPumps(DTO.Pumps);
            result.AddSkids(DTO.Skids);
            result.AddOperators(DTO.Operators);
            result.MapMaterialEquipments(DTO.MaterialEquipments);
            result.MapConnectors(DTO.Connectors);
            result.CreateProcess(DTO.MaterialEquipments);
            result.SKULines = DTO.SKULines;

            result.AnalyzeProductionCapabilities();
            return result;
        }
    }
}
