using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simulator.Server.Databases.Entities.Equilibrio;

namespace Simulator.Server.Databases.Configurations.Properties
{
    public class CompoundPropertyConfiguration : IEntityTypeConfiguration<CompoundProperty>
    {
        public void Configure(EntityTypeBuilder<CompoundProperty> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasOne(e => e.VapourPressure)
                     .WithMany(h => h.VaporPressures)
                     .HasForeignKey(e => e.VapourPressureId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.HeatOfVaporization)
                    .WithMany(h => h.HeatOfVaporizations)
                    .HasForeignKey(e => e.HeatOfVaporizationId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LiquidCp)
                   .WithMany(h => h.LiquidCps)
                   .HasForeignKey(e => e.LiquidCpId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.GasCp)
                   .WithMany(h => h.GasCps)
                   .HasForeignKey(e => e.GasCpId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LiquidViscosity)
                   .WithMany(h => h.LiquidViscosities)
                   .HasForeignKey(e => e.LiquidViscosityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.GasViscosity)
                    .WithMany(h => h.GasViscosities)
                    .HasForeignKey(e => e.GasViscosityId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LiquidThermalConductivity)
                   .WithMany(h => h.LiquidThermalConductivities)
                   .HasForeignKey(e => e.LiquidThermalConductivityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.GasThermalConductivity)
                   .WithMany(h => h.GasThermalConductivities)
                   .HasForeignKey(e => e.GasThermalConductivityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LiquidDensity)
                   .WithMany(h => h.LiquidDensities)
                   .HasForeignKey(e => e.LiquidDensityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SuperficialTension)
                   .WithMany(h => h.SuperficialTensions)
                   .HasForeignKey(e => e.SuperficialTensionId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
