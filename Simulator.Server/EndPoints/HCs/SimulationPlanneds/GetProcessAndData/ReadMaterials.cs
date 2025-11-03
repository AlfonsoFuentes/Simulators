using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadMaterials
    {
        public static async Task ReadSimulationMaterials(this NewSimulationDTO simulation, IServerCrudService service)
        {
            var dto = new CompletedMaterialDTO()
            {
                FocusFactory = simulation.FocusFactory,
                
            };
            var query = await service.GetAllAsync<Material>(dto, querysuffix: $"{dto.FocusFactory}");
            if (query != null && query.Count > 0)
            {
                simulation.Materials = query.Select(x => x.MapToDto<MaterialDTO>()).ToList();
                var productsbackbones = simulation.Materials.Where(x =>
                x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone).ToList();

                foreach (var row in productsbackbones)
                {
                    if (query.Any(x => x.Id == row.Id && x.BackBoneSteps != null && x.BackBoneSteps.Count > 0))
                    {
                        row.BackBoneSteps = query.First(x => x.Id == row.Id).BackBoneSteps.OrderBy(x => x.Order).Select(x => x.MapToDto<BackBoneStepDTO>()).ToList();
                    }

                }

            }
        }

        static async Task ReadBackboneSteps(this MaterialDTO material, IServerCrudService service)
        {
            var stepdto = new BackBoneStepDTO() { MaterialId = material.Id };
            var rows = await service.GetAllAsync<BackBoneStep>(stepdto, parentId: $"{material.Id}");


            if (rows != null && rows.Count > 0)
            {
                material.BackBoneSteps = rows.Select(x => x.MapToDto<BackBoneStepDTO>()).ToList();


            }
        }

    }
}
