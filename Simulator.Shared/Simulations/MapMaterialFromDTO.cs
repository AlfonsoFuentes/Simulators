using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Simulations.Materials;

namespace Simulator.Shared.Simulations
{
    public static class MapMaterialFromDTO
    {
        public static void MapMaterials(this NewSimulation simulation, List<MaterialDTO> materials)
        {
            simulation.RawMaterialSimulations = materials.Where(x => x.MaterialType == MaterialType.RawMaterial).Select(x => x.MapRawMaterial(simulation)).ToList();
            simulation.BackBoneRawMaterialSimulations = materials.Where(x => x.MaterialType == MaterialType.RawMaterialBackBone).Select(x => x.MapBackBoneRawMaterial(simulation)).ToList();
            simulation.ProductBackBoneSimulations = materials.Where(x => x.MaterialType == MaterialType.ProductBackBone).Select(x => x.MapBackBone(simulation)).ToList();
            simulation.MaterialSimulations.AddRange(simulation.RawMaterialSimulations);
            simulation.MaterialSimulations.AddRange(simulation.BackBoneRawMaterialSimulations);
            simulation.MaterialSimulations.AddRange(simulation.ProductBackBoneSimulations);
            simulation.BackBoneSimulations.AddRange(simulation.BackBoneRawMaterialSimulations);
            simulation.BackBoneSimulations.AddRange(simulation.ProductBackBoneSimulations);
        }
        public static RawMaterialSimulation MapRawMaterial(this MaterialDTO material, NewSimulation simulation)
        {
            return new RawMaterialSimulation(simulation)
            {
                MaterialType = material.MaterialType,
                CommonName = material.CommonName,
                Id = material.Id,
                IsForWashing = material.IsForWashing,
                M_Number = material.M_Number,
                PhysicalState = material.PhysicalState,
                SAPName = material.SAPName,
                Type = material.MaterialType,
                ProductCategory = material.ProductCategory,




            };
        }
        public static BackBoneRawMaterialSimulation MapBackBoneRawMaterial(this MaterialDTO material,NewSimulation simulation)
        {
            var result = new BackBoneRawMaterialSimulation(simulation)
            {
                MaterialType = material.MaterialType,
                CommonName = material.CommonName,
                Id = material.Id,
                IsForWashing = material.IsForWashing,
                M_Number = material.M_Number,
                PhysicalState = material.PhysicalState,
                SAPName = material.SAPName,
                Type = material.MaterialType,
                ProductCategory = material.ProductCategory
            };
            foreach (var item in material.BackBoneSteps.OrderBy(x => x.Order))
            {
                result.AddBackBoneStep(item.MapBackBoneStep(result, simulation));
            }
            return result;
        }
        public static ProductBackBoneSimulation MapBackBone(this MaterialDTO material, NewSimulation simulation)
        {
            var result = new ProductBackBoneSimulation(simulation)
            {
                MaterialType = material.MaterialType,
                CommonName = material.CommonName,
                Id = material.Id,
                IsForWashing = material.IsForWashing,
                M_Number = material.M_Number,
                PhysicalState = material.PhysicalState,
                SAPName = material.SAPName,
                Type = material.MaterialType,
                ProductCategory = material.ProductCategory,


            };
            foreach (var item in material.BackBoneSteps.OrderBy(x => x.Order))
            {
                result.AddBackBoneStep(item.MapBackBoneStep(result, simulation));
            }
            return result;
        }
        public static BackBoneStepSimulation MapBackBoneStep(this BackBoneStepDTO step, MaterialSimulation material, NewSimulation simulation)
        {
            return new()
            {
                Id = step.Id,
                Material = material,
                StepRawMaterial = step.StepRawMaterial == null ? null! : step.StepRawMaterial.MapRawMaterial(simulation),
                BackBoneStepType = step.BackBoneStepType,
                Order = step.Order,
                Percentage = step.Percentage,
                Time = step.Time,


            };
        }

    }
}
