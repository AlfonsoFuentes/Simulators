using LazyCache;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.Databases.Entities.Equilibrio;
using Simulator.Server.Databases.Entities.HC;
using Simulator.Server.Interfaces.Database;
using Simulator.Server.Interfaces.UserServices;
using System.Reflection;

namespace Simulator.Server.Implementations.Databases
{
    public class BlazorHeroContext : AuditableContext, IAppDbContext
    {
        private readonly ICache _cache2;
        string _tenantId => _cache2._tenantId;


   


        public BlazorHeroContext(DbContextOptions<BlazorHeroContext> options,  ICache  cache2)
            : base(options)
        {

          
            _cache2 = cache2;
        }


        public DbSet<Material> Materials { get; set; }
        public DbSet<BackBoneStep> BackBoneSteps { get; set; }
        public DbSet<SKU> HCSKUs { get; set; }
        public DbSet<Washout> Washouts { get; set; }
        public DbSet<Conector> Conectors { get; set; }
        public DbSet<ProcessFlowDiagram> MainProceses { get; set; }

        public DbSet<ContinuousSystem> ContinuousSystems { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Mixer> HCMixers { get; set; }
        public DbSet<Pump> Pumps { get; set; }
        public DbSet<StreamJoiner> StreamJoiners { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Tank> Tanks { get; set; }
        public DbSet<MaterialEquipment> MaterialEquipments { get; set; }
        public DbSet<SKULine> SKULines { get; set; }
        public DbSet<EquipmentPlannedDownTime> EquipmentPlannedDownTimes { get; set; }
        public DbSet<SimulationPlanned> SimulationPlanneds { get; set; }
        public DbSet<LinePlanned> LinePlanneds { get; set; }
        public DbSet<MixerPlanned> MixerPlanneds { get; set; }
        public DbSet<PlannedSKU> PlannedSKUs { get; set; }
        public DbSet<PreferedMixer> PreferedMixer { get; set; }
        public DbSet<CompoundProperty> CompoundProperties { get; set; }
        public DbSet<CompoundConstant> CompoundConstants { get; set; }
        void ConfiguerQueryFilters(ModelBuilder builder)
        {




            builder.Entity<Material>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<BackBoneStep>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<SKU>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<Washout>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<Conector>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<ProcessFlowDiagram>().HasQueryFilter(p => p.IsDeleted == false);

            builder.Entity<BaseEquipment>().HasQueryFilter(p => p.IsDeleted == false);

            builder.Entity<MaterialEquipment>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<SKULine>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<EquipmentPlannedDownTime>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<SimulationPlanned>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<LinePlanned>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<MixerPlanned>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<PlannedSKU>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<PreferedMixer>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<CompoundProperty>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<CompoundConstant>().HasQueryFilter(p => p.IsDeleted == false);
            builder.Entity<BaseEquipment>().UseTpcMappingStrategy();

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            ConfigureDatatTypes(builder);

            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            ConfiguerQueryFilters(builder);


        }

        void ConfigureDatatTypes(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
           .SelectMany(t => t.GetProperties())
           .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }
        }

        //public async Task<int> SaveChangesAndRemoveCacheAsync(params string[] cacheKeys)
        //{
        //    var result = await SaveChangesAsync();

        //    if (cacheKeys == null) return result;

        //    if (result > 0)
        //    {
        //        foreach (var cacheKey in cacheKeys)
        //        {
        //            var key = $"{cacheKey}-{_tenantId}";
        //            _cache.Remove(key);
        //        }
        //    }

        //    return result;
        //}

        //public Task<T> GetOrAddCacheAsync<T>(string key, Func<Task<T>> addItemFactory)
        //{
        //    if (_cache == null)
        //    {
        //        throw new ArgumentNullException("cache");
        //    }
        //    key = $"{key}";
        //    return _cache.GetOrAddAsync(key, addItemFactory);
        //}
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            try
            {

                var entittes = ChangeTracker.Entries<IEntity>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).ToList();


                foreach (var row in entittes)
                {
                    if (row.State == EntityState.Added)
                    {
                        if (row.Entity.IsTenanted)
                        {
                            var entity = row.Entity as ITennant;
                            entity!.TenantId = _tenantId;
                        }

                    }

                }


                var result = await base.SaveChangesAsync();

                return result; // ✅ devuelve el valor real
            }
            catch (Exception ex)
            {
                string exm = ex.Message;
            }

            return 0;
        }
        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(_tenantId))
        //        {
        //            return await base.SaveChangesAsync();
        //        }
        //        var AddedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is ITennant);
        //        foreach (var item in AddedEntities)
        //        {
        //            var entity = item.Entity as ITennant;
        //            entity!.TenantId = _tenantId;

        //        }
        //        var entittes = ChangeTracker.Entries<IEntity>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).ToList();


        //        foreach (var row in entittes)
        //        {
        //            if (row.State == EntityState.Added)
        //            {
        //                row.Entity.CreatedOn = DateTime.Now;
        //                row.Entity.CreatedBy = _tenantId;

        //            }

        //            //if (row.State == EntityState.Modified)
        //            //{
        //            //    row.Entity.LastModifiedOn = DateTime.Now;
        //            //    row.Entity.LastModifiedBy = _tenantId;
        //            //}

        //        }


        //        return await base.SaveChangesAsync();

        //    }
        //    catch (Exception ex)
        //    {
        //        string exm = ex.Message;
        //    }

        //    return 0;
        //}


    }
}
