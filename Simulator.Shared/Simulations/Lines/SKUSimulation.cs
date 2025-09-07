using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SKUs;
using Simulator.Shared.Simulations.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Simulations.Lines
{
    public class SKUSimulation
    {
        public Guid Id { get; set; }

        public void SetBackBone(ProductBackBoneSimulation _BackBoneSimulation)
        {
            BackBoneSimulation=_BackBoneSimulation;

        }
        public ProductBackBoneSimulation BackBoneSimulation { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string SkuCode { get; set; } = string.Empty;

        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;


        public string BackBoneName => BackBoneSimulation == null ? string.Empty : BackBoneSimulation.CommonName;
        public string BackBoneM_Number => BackBoneSimulation == null ? string.Empty : BackBoneSimulation.M_Number;

       
        public Amount Size { get; set; } = new Amount(VolumeUnits.MilliLiter);
       
        public Amount Weigth { get; set; } = new Amount(MassUnits.KiloGram);
        public int EA_Case { get; set; }


        public string SKUCodeName => $"{SkuCode} - {Name}";

        public PackageType PackageType { get; set; } = PackageType.None;
       
    }

}
