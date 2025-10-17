using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.BackBoneSteps;
using Simulator.Server.EndPoints.HCs.Lines;
using Simulator.Server.EndPoints.HCs.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadMaterials
    {
        public static async Task ReadSimulationMaterials(this NewSimulationDTO simulation, IQueryRepository Repository)
        {
            string CacheKey = StaticClass.Materials.Cache.GetAll(simulation.FocusFactory);
            var rows = await Repository.GetAllAsync<Material>(CacheKey);
            if (rows != null && rows.Count > 0)
            {
                simulation.Materials = rows.Select(x => x.MapMaterial()).ToList();
                var productsbackbones = simulation.Materials.Where(x => x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone).ToList();

                foreach (var row in productsbackbones)
                {
                    await row.ReadBackboneSteps(Repository);
                }

            }
        }

        static async Task ReadBackboneSteps(this MaterialDTO material, IQueryRepository Repository)
        {
            Func<IQueryable<BackBoneStep>, IIncludableQueryable<BackBoneStep, object>> includes = x => x
                   .Include(y => y.RawMaterial!);
            string CacheKey = StaticClass.BackBoneSteps.Cache.GetAll(material.Id);
            Expression<Func<BackBoneStep, bool>> Criteria = x => x.MaterialId == material.Id;
            var rows = await Repository.GetAllAsync(Cache: CacheKey, Includes: includes, Criteria: Criteria);
            if (rows != null && rows.Count > 0)
            {
                material.BackBoneSteps = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();


            }
        }

    }
}
