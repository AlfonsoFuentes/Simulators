using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simulator.Server.Databases.Entities.HC;

namespace Simulator.Server.Databases.Configurations.HC
{
    internal class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.HasKey(ci => ci.Id);




            builder.HasMany(e => e.BackBoneSteps)
                 .WithOne(e => e.HCMaterial)
                 .HasForeignKey(e => e.MaterialId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);

        }
    }
    internal class BackBoneStepConfiguration : IEntityTypeConfiguration<BackBoneStep>
    {
        public void Configure(EntityTypeBuilder<BackBoneStep> builder)
        {
            builder.HasKey(ci => ci.Id);



            builder.HasOne(e => e.RawMaterial)
                     .WithMany(h => h.RawMaterials)
                     .HasForeignKey(e => e.MaterialId)
                     .OnDelete(DeleteBehavior.Restrict);

        }
    }
    internal class SKUConfiguration : IEntityTypeConfiguration<SKU>
    {
        public void Configure(EntityTypeBuilder<SKU> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasOne(e => e.Material)
                  .WithMany(e => e.SKUs)
                  .HasForeignKey(e => e.MaterialId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);


        }
    }
    internal class SKULineConfiguration : IEntityTypeConfiguration<SKULine>
    {
        public void Configure(EntityTypeBuilder<SKULine> builder)
        {
            // Relación: BudgetItem -> NewGanttTasks
            builder
                .HasOne(td => td.SKU)
                .WithMany(t => t.SKULines) // Fix: Ensure the navigation property matches the type
                .HasForeignKey(td => td.SKUId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            // Relación: NewGanttTask -> BudgetItems
            builder
                .HasOne(td => td.Line)
                .WithMany(t => t.SKULines) // Fix: Ensure the navigation property matches the type
                .HasForeignKey(td => td.LineId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada
        }
    }
    internal class ConectorConfiguration : IEntityTypeConfiguration<Conector>
    {
        public void Configure(EntityTypeBuilder<Conector> builder)
        {
            builder.HasKey(ci => ci.Id);


            builder.HasOne(e => e.From)
                       .WithMany(h => h.Froms)
                       .HasForeignKey(e => e.FromId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.To)
                       .WithMany(h => h.Tos)
                       .HasForeignKey(e => e.ToId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict);

        }
    }
    internal class HCBaseEquipmentConfiguration : IEntityTypeConfiguration<BaseEquipment>
    {
        public void Configure(EntityTypeBuilder<BaseEquipment> builder)
        {
            builder.HasKey(ci => ci.Id);


            builder.HasMany(e => e.PlannedDownTimes)
        .WithOne(e => e.BaseEquipment)
        .HasForeignKey(e => e.BaseEquipmentId).IsRequired()

        .OnDelete(DeleteBehavior.Cascade);


        }
    }
    internal class HCMaterialEquipmentConfiguration : IEntityTypeConfiguration<MaterialEquipment>
    {
        public void Configure(EntityTypeBuilder<MaterialEquipment> builder)
        {
            builder.HasKey(ci => ci.Id);





            builder.HasOne(e => e.Material)
                   .WithMany(h => h.ProcessEquipments)
                   .HasForeignKey(e => e.MaterialId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ProccesEquipment)
                      .WithMany(h => h.Materials)
                      .HasForeignKey(e => e.ProccesEquipmentId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

        }
    }



    internal class ProcessFlowDiagramConfiguration : IEntityTypeConfiguration<ProcessFlowDiagram>
    {
        public void Configure(EntityTypeBuilder<ProcessFlowDiagram> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasMany(e => e.ProccesEquipments)
               .WithOne(e => e.MainProcess)
               .HasForeignKey(e => e.MainProcessId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.SimulationPlanneds)
           .WithOne(e => e.MainProcess)
           .HasForeignKey(e => e.MainProcessId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);



        }
    }
    internal class SimulationPlannedConfiguration : IEntityTypeConfiguration<SimulationPlanned>
    {
        public void Configure(EntityTypeBuilder<SimulationPlanned> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasMany(e => e.LinePlanneds)
               .WithOne(e => e.HCSimulationPlanned)
               .HasForeignKey(e => e.SimulationPlannedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.MixerPlanneds)
           .WithOne(e => e.SimulationPlanned)
           .HasForeignKey(e => e.SimulationPlannedId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

        }
    }
    internal class LinePlannedConfiguration : IEntityTypeConfiguration<LinePlanned>
    {
        public void Configure(EntityTypeBuilder<LinePlanned> builder)
        {
            builder.HasKey(ci => ci.Id);


            builder.HasOne(e => e.Line)
                     .WithMany(h => h.LinePlanneds)
                     .HasForeignKey(e => e.LineId)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.Restrict);



        }
    }

    internal class MixerPlannedConfiguration : IEntityTypeConfiguration<MixerPlanned>
    {
        public void Configure(EntityTypeBuilder<MixerPlanned> builder)
        {
            builder.HasKey(ci => ci.Id);


            builder.HasOne(e => e.Mixer)
                 .WithMany(h => h.MixerPlanneds)
                 .HasForeignKey(e => e.MixerId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.BackBone)
               .WithMany(h => h.MixerPlanneds)
               .HasForeignKey(e => e.BackBoneId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.BackBoneStep)
               .WithMany(h => h.MixerPlanneds)
               .HasForeignKey(e => e.BackBoneStepId)

               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ProducingTo)
            .WithMany(e => e.MixerPlanneds)
            .HasForeignKey(e => e.ProducingToId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class SKUPlannedConfiguration : IEntityTypeConfiguration<PlannedSKU>
    {
        public void Configure(EntityTypeBuilder<PlannedSKU> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasOne(e => e.SKU)
                 .WithMany(e => e.PlannedSKUs)
                 .HasForeignKey(e => e.SKUId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LinePlanned)
               .WithMany(e => e.SKUPlanneds)
               .HasForeignKey(e => e.LinePlannedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
    internal class PreferedMixerConfiguration : IEntityTypeConfiguration<PreferedMixer>
    {
        public void Configure(EntityTypeBuilder<PreferedMixer> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasOne(e => e.Mixer)
                 .WithMany(e => e.PreferedMixers)
                 .HasForeignKey(e => e.MixerId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.LinePlanned)
               .WithMany(e => e.PreferedMixers)
               .HasForeignKey(e => e.LinePlannedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        }
    }


    internal class PumpConfiguration : IEntityTypeConfiguration<Pump>
    {
        public void Configure(EntityTypeBuilder<Pump> builder)
        {
            //builder.HasKey(ci => ci.Id);




        }
    }



    internal class WashoutTimeConfiguration : IEntityTypeConfiguration<Washout>
    {
        public void Configure(EntityTypeBuilder<Washout> builder)
        {
            builder.HasKey(ci => ci.Id);




        }
    }
}
