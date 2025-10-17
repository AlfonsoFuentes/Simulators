using Simulator.Server.Databases.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.Equilibrio
{
    public class CompoundProperty : AuditableEntity<Guid>, ITenantCommon
    {
        public static CompoundProperty Create()
        {
            var row = new CompoundProperty();
            row.Id = Guid.NewGuid();
            row.VapourPressure = new();
            row.HeatOfVaporization = new();
            row.LiquidCp = new();
            row.GasCp = new();
            row.LiquidViscosity = new();
            row.GasViscosity = new();
            row.GasThermalConductivity = new();
            row.LiquidThermalConductivity = new();
            row.LiquidDensity = new();
            row.SuperficialTension = new();


            return row;
        }
        public string Name { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;
        public string StructuralFormula { get; set; } = string.Empty;
        public string MainFamily { get; set; } = string.Empty;
        public string SecondaryFamily { get; set; } = string.Empty;
        public double MolecularWeight { get; set; }
        public double Critical_Z { get; set; }
        public double Acentric_Factor { get; set; }
        public double Acentric_Factor_SRK { get; set; }
        public double Critical_Temperature { get; set; }
        public string Critical_Temperature_Unit { get; set; } = string.Empty;

        public double Critical_Pressure { get; set; }
        public string Critical_Pressure_Unit { get; set; } = string.Empty;
        public double Critical_Volume { get; set; }
        public string Critical_Volume_Unit { get; set; } = string.Empty;

        public double Boiling_Temperature { get; set; }
        public string Boiling_Temperature_Unit { get; set; } = string.Empty;

        public double Melting_Temperature { get; set; }
        public string Melting_Temperature_Unit { get; set; } = string.Empty;

        public double Asterisk_Volume { get; set; }
        public string Asterisk_Volume_Unit { get; set; } = string.Empty;

        public CompoundConstant VapourPressure { get; set; } = null!;
        public Guid VapourPressureId { get; set; }
        public CompoundConstant HeatOfVaporization { get; set; } = null!;
        public Guid HeatOfVaporizationId { get; set; }
        public CompoundConstant LiquidCp { get; set; } = null!;
        public Guid LiquidCpId { get; set; }
        public CompoundConstant GasCp { get; set; } = null!;
        public Guid GasCpId { get; set; }
        public CompoundConstant LiquidViscosity { get; set; } = null!;
        public Guid LiquidViscosityId { get; set; }
        public CompoundConstant GasViscosity { get; set; } = null!;
        public Guid GasViscosityId { get; set; }

        public CompoundConstant LiquidThermalConductivity { get; set; } = null!;
        public Guid LiquidThermalConductivityId { get; set; }
        public CompoundConstant GasThermalConductivity { get; set; } = null!;
        public Guid GasThermalConductivityId { get; set; }
        public CompoundConstant LiquidDensity { get; set; } = null!;
        public Guid LiquidDensityId { get; set; }
        public CompoundConstant SuperficialTension { get; set; } = null!;
        public Guid SuperficialTensionId { get; set; }

        public double Gibbs_Energy_Formation { get; set; }
        public string Gibbs_Energy_Formation_Unit { get; set; } = string.Empty;
        public double Enthalpy_Formation { get; set; }
        public string Enthalpy_Formation_Unit { get; set; } = string.Empty;
        public double Entropy_Formation { get; set; }
        public string Entropy_Formation_Unit { get; set; } = string.Empty;
        public double Enthalpy_Combustion { get; set; }
        public string Enthalpy_Combustion_Unit { get; set; } = string.Empty;

    }

    public class CompoundConstant : AuditableEntity<Guid>, ITenantCommon
    {
        public double C1 { get; set; }
        public double C2 { get; set; }
        public double C3 { get; set; }
        public double C4 { get; set; }
        public double C5 { get; set; }
        public double C6 { get; set; }
        public double C7 { get; set; }
        public double Minimal_Temperature { get; set; }
        public string Minimal_Temperature_Unit { get; set; } = string.Empty;
        public double Maximum_Temperature { get; set; }
        public string Maximum_Temperature_Unit { get; set; } = string.Empty;

        [ForeignKey("VapourPressureId")]
        public List<CompoundProperty> VaporPressures { get; set; } = [];
        [ForeignKey("HeatOfVaporizationId")]
        public List<CompoundProperty> HeatOfVaporizations { get; set; } = [];
        [ForeignKey("LiquidCpId")]
        public List<CompoundProperty> LiquidCps { get; set; } = [];
        [ForeignKey("GasCpId")]
        public List<CompoundProperty> GasCps { get; set; } = [];
        [ForeignKey("LiquidViscosityId")]
        public List<CompoundProperty> LiquidViscosities { get; set; } = [];
        [ForeignKey("GasViscosityId")]
        public List<CompoundProperty> GasViscosities { get; set; } = [];
        [ForeignKey("LiquidThermalConductivityId")]
        public List<CompoundProperty> LiquidThermalConductivities { get; set; } = [];
        [ForeignKey("GasThermalConductivityId")]
        public List<CompoundProperty> GasThermalConductivities { get; set; } = [];
        [ForeignKey("LiquidDensityId")]
        public List<CompoundProperty> LiquidDensities { get; set; } = [];
        [ForeignKey("SuperficialTensionId")]
        public List<CompoundProperty> SuperficialTensions { get; set; } = [];

    }
}
