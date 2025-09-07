using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.EndPoints.HCs.EquipmentPlannedDownTimes;
using Simulator.Server.EndPoints.HCs.LinePlanneds;
using Simulator.Server.EndPoints.HCs.MixerPlanneds;
using Simulator.Server.EndPoints.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class ReadSimulationPlannedss
    {
        public static async Task ReadPlannedLines(this SimulationPlannedDTO planned, IQueryRepository Repository)
        {
            Func<IQueryable<LinePlanned>, IIncludableQueryable<LinePlanned, object>> includes = x => x
                  .Include(y => y.Line);
            Expression<Func<LinePlanned, bool>> Criteria = x => x.SimulationPlannedId == planned.Id;
            string CacheKey = StaticClass.LinePlanneds.Cache.GetAll(planned.Id);
            var rows = await Repository.GetAllAsync<LinePlanned>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

            if (rows != null && rows.Count > 0)
            {
                planned.PlannedLines = rows.Select(x => x.Map()).ToList();
                if (planned.PlannedLines != null)
                {
                    foreach (var row in planned.PlannedLines)
                    {
                        await row.ReadPlannedSKU(Repository);
                    }
                }
            }


        }
        public static async Task ReadPlannedMixers(this SimulationPlannedDTO planned, IQueryRepository Repository)
        {
            Func<IQueryable<MixerPlanned>, IIncludableQueryable<MixerPlanned, object>> includes = x => x
                     .Include(x => x.SimulationPlanned)
                     .Include(y => y.Mixer!)
                     .Include(x => x.BackBone!)
                     .Include(x => x.BackBoneStep!)
                     .Include(x => x.ProducingTo!);
            Expression<Func<MixerPlanned, bool>> Criteria = x => x.SimulationPlannedId == planned.Id;
            string CacheKey = StaticClass.MixerPlanneds.Cache.GetAll(planned.Id);
            var rows = await Repository.GetAllAsync<MixerPlanned>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

            if (rows != null && rows.Count > 0)
            {
                planned.PlannedMixers = rows.Select(x => x.Map()).ToList();
            }





        }
        public static async Task ReadPlannedSKU(this LinePlannedDTO plannedLine, IQueryRepository Repository)
        {
            Func<IQueryable<PlannedSKU>, IIncludableQueryable<PlannedSKU, object>> includes = x => x
                  .Include(y => y.SKU)
                  .Include(x => x.LinePlanned); ;
            Expression<Func<PlannedSKU, bool>> Criteria = x => x.LinePlannedId == plannedLine.Id;
            string CacheKey = StaticClass.PlannedSKUs.Cache.GetAll(plannedLine.Id);
            var rows = await Repository.GetAllAsync<PlannedSKU>(Cache: CacheKey, Criteria: Criteria, Includes: includes);

            if (rows != null && rows.Count > 0)
            {
                plannedLine.PlannedSKUDTOs = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();
            }







        }

    }
}
