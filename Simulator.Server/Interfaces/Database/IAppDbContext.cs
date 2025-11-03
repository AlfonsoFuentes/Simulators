using Simulator.Server.Databases.Entities.Equilibrio;
using Simulator.Server.Databases.Entities.HC;

namespace Simulator.Server.Interfaces.Database
{
    public interface IAppDbContext
    {
      
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
      
        DbSet<Material> Materials { get; set; }
        DbSet<BackBoneStep> BackBoneSteps { get; set; }
        DbSet<SKU> HCSKUs { get; set; }
        DbSet<Washout> Washouts { get; set; }
        DbSet<Conector> Conectors { get; set; }
        DbSet<ProcessFlowDiagram> MainProceses { get; set; }

        DbSet<ContinuousSystem> ContinuousSystems { get; set; }
        DbSet<Line> Lines { get; set; }
        DbSet<Mixer> HCMixers { get; set; }
        DbSet<Pump> Pumps { get; set; }
        DbSet<Operator> Operators { get; set; }
        DbSet<Tank> Tanks { get; set; }
        DbSet<MaterialEquipment> MaterialEquipments { get; set; }
        DbSet<SKULine> SKULines { get; set; }
        DbSet<EquipmentPlannedDownTime> EquipmentPlannedDownTimes { get; set; }
        DbSet<SimulationPlanned> SimulationPlanneds { get; set; }
        DbSet<LinePlanned> LinePlanneds { get; set; }
        DbSet<MixerPlanned> MixerPlanneds { get; set; }
        DbSet<PlannedSKU> PlannedSKUs { get; set; }
        DbSet<PreferedMixer> PreferedMixer { get; set; }
        DbSet<CompoundConstant> CompoundConstants { get; set; }
        DbSet<CompoundProperty> CompoundProperties { get; set; }
        DbSet<StreamJoiner> StreamJoiners { get; set; }

        //Task<int> SaveChangesAndRemoveCacheAsync(params string[] cacheKeys);
        //Task<T> GetOrAddCacheAsync<T>(string key, Func<Task<T>> addItemFactory);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
