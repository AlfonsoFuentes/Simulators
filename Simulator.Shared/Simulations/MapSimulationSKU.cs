using Simulator.Shared.Models.HCs.SKUs;
using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations
{
    public static class MapSimulationSKU
    {
        public static void MapSKUS(this NewSimulation simulation, List<SKUDTO> skus)
        {
            simulation.SkuSimulations = skus.Select(x => x.MapSKU(simulation)).ToList();
     
        }
        public static SKUSimulation MapSKU(this SKUDTO sku, NewSimulation simulation)
        {
            return new()
            {
                Id = sku.Id,
                BackBoneSimulation = sku.BackBone == null ? null! : simulation.ProductBackBoneSimulations.FirstOrDefault(x => x.Id == sku.BackBone.Id)!,
                EA_Case = sku.EA_Case,
                Name = sku.Name,
                PackageType = sku.PackageType,
                ProductCategory = sku.ProductCategory,
                Size = sku.Size,
                Weigth = sku.Weigth,
                SkuCode = sku.SkuCode,




            };
        }
    }
}
