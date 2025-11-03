using Simulator.Server.Databases.Contracts;
using Simulator.Server.EndPoints.Properties;
using Simulator.Shared.Intefaces;
using Simulator.Shared.NewModels.Compounds;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.Equilibrio
{
    public class CompoundProperty : Entity, IQueryHandler<CompoundProperty>, IMapper, ICreator<CompoundProperty>
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
        static Func<IQueryable<CompoundProperty>, IIncludableQueryable<CompoundProperty, object>> IQueryHandler<CompoundProperty>.GetIncludesBy(IDto dto)
        {
            return dto switch
            {
                NewCompoundPropertyDTO => x => x
                    .Include(y => y.VapourPressure)
                    .Include(y => y.HeatOfVaporization)
                    .Include(y => y.LiquidCp)
                    .Include(y => y.GasCp)
                    .Include(y => y.LiquidViscosity)
                    .Include(y => y.GasViscosity)
                    .Include(y => y.LiquidThermalConductivity)
                    .Include(y => y.GasThermalConductivity)
                    .Include(y => y.LiquidDensity)
                    .Include(y => y.SuperficialTension),
                _ => null!
            };
        }
        static Expression<Func<CompoundProperty, object>> IQueryHandler<CompoundProperty>.GetOrderBy(IDto dto)
        {
            return dto switch
            {
                NewCompoundPropertyDTO => f => f.Name,

                _ => null!
            };
        }

        // ✅ GetFilterBy también tipado
        static Expression<Func<CompoundProperty, bool>> IQueryHandler<CompoundProperty>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                NewCompoundPropertyDTO compounddto when compounddto.Id != Guid.Empty
                    => f => f.Id == compounddto.Id,
                _ => null!
            };
        }
       
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case NewCompoundPropertyDTO request:
                    {
                        Name = request.Name;
                        Formula = request.Formula;
                        StructuralFormula = request.StructuralFormula;
                        MainFamily = request.MainFamily;
                        SecondaryFamily = request.SecondaryFamily;
                        MolecularWeight = request.MolecularWeight;
                        Critical_Z = request.Critical_Z;
                        Acentric_Factor = request.Acentric_Factor;
                        Acentric_Factor_SRK = request.Acentric_Factor_SRK;
                        Critical_Temperature = request.Critical_Temperature.Value;
                        Critical_Temperature_Unit = request.Critical_Temperature.UnitName;
                        Critical_Pressure = request.Critical_Pressure.Value;
                        Critical_Pressure_Unit = request.Critical_Pressure.UnitName;
                        Critical_Volume = request.Critical_Volume.Value;
                        Critical_Volume_Unit = request.Critical_Volume.UnitName;
                        Boiling_Temperature = request.Boiling_Temperature.Value;
                        Boiling_Temperature_Unit = request.Boiling_Temperature.UnitName;
                        Melting_Temperature = request.Melting_Temperature.Value;
                        Melting_Temperature_Unit = request.Melting_Temperature.UnitName;
                        Asterisk_Volume = request.Asterisk_Volume.Value;
                        Asterisk_Volume_Unit = request.Asterisk_Volume.UnitName;
                        Gibbs_Energy_Formation = request.Gibbs_Energy_Formation.Value;
                        Gibbs_Energy_Formation_Unit = request.Gibbs_Energy_Formation.UnitName;
                        Enthalpy_Formation = request.Enthalpy_Formation.Value;
                        Enthalpy_Formation_Unit = request.Enthalpy_Formation.UnitName;
                        Entropy_Formation = request.Entropy_Formation.Value;
                        Entropy_Formation_Unit = request.Entropy_Formation.UnitName;
                        Enthalpy_Combustion = request.Enthalpy_Combustion.Value;
                        Enthalpy_Combustion_Unit = request.Enthalpy_Combustion.UnitName;

                        VapourPressure = request.VapourPressure.Map(VapourPressure);
                        HeatOfVaporization = request.HeatOfVaporization.Map(HeatOfVaporization);
                        LiquidCp = request.LiquidCp.Map(LiquidCp);
                        GasCp = request.GasCp.Map(GasCp);
                        LiquidViscosity = request.LiquidViscosity.Map(LiquidViscosity);
                        GasViscosity = request.GasViscosity.Map(GasViscosity);
                        LiquidThermalConductivity = request.LiquidThermalConductivity.Map(LiquidThermalConductivity);
                        GasThermalConductivity = request.GasThermalConductivity.Map(GasThermalConductivity);
                        LiquidDensity = request.LiquidDensity.Map(LiquidDensity);
                        SuperficialTension = request.SuperficialTension.Map(SuperficialTension);
                    }
                    break;

                default:
                    break;

            }
        }

        public T MapToDto<T>() where T : IDto, new()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(NewCompoundPropertyDTO) => (T)(object)new NewCompoundPropertyDTO
                {
                    
                    Acentric_Factor = Acentric_Factor,
                    Acentric_Factor_SRK = Acentric_Factor_SRK,
                    Formula = Formula,
                    StructuralFormula = StructuralFormula,
                    MainFamily = MainFamily,
                    SecondaryFamily = SecondaryFamily,
                    MolecularWeight = MolecularWeight,
                    Name = Name,
                    Critical_Z = Critical_Z,
                    Critical_Temperature_Unit = Critical_Temperature_Unit,
                    Critical_Temperature_Value = Critical_Temperature,

                    Critical_Pressure_Unit = Critical_Pressure_Unit,
                    Critical_Pressure_Value = Critical_Pressure,

                    Critical_Volume_Unit = Critical_Volume_Unit,
                    Critical_Volume_Value = Critical_Volume,

                    Boiling_Temperature_Unit = Boiling_Temperature_Unit,
                    Boiling_Temperature_Value = Boiling_Temperature,

                    Melting_Temperature_Unit = Melting_Temperature_Unit,
                    Melting_Temperature_Value = Melting_Temperature,

                    Asterisk_Volume_Unit = Asterisk_Volume_Unit,
                    Asterisk_Volume_Value = Asterisk_Volume,

                    Gibbs_Energy_Formation_Unit = Gibbs_Energy_Formation_Unit,
                    Gibbs_Energy_Formation_Value = Gibbs_Energy_Formation,

                    Enthalpy_Combustion_Unit = Enthalpy_Combustion_Unit,
                    Enthalpy_Combustion_Value = Enthalpy_Combustion,
                    Enthalpy_Formation_Unit = Enthalpy_Formation_Unit,
                    Enthalpy_Formation_Value = Enthalpy_Formation,
                    Entropy_Formation_Unit = Entropy_Formation_Unit,
                    Entropy_Formation_Value = Entropy_Formation,


                    VapourPressure = VapourPressure == null ? new() : VapourPressure.Map(),
                    HeatOfVaporization = HeatOfVaporization == null ? new() : HeatOfVaporization.Map(),
                    LiquidCp = LiquidCp == null ? new() : LiquidCp.Map(),
                    GasCp = GasCp == null ? new() : GasCp.Map(),
                    LiquidViscosity = LiquidViscosity == null ? new() : LiquidViscosity.Map(),
                    GasViscosity = GasViscosity == null ? new() : GasViscosity.Map(),
                    LiquidThermalConductivity = LiquidThermalConductivity == null ? new() : LiquidThermalConductivity.Map(),
                    GasThermalConductivity = GasThermalConductivity == null ? new() : GasThermalConductivity.Map(),
                    LiquidDensity = LiquidDensity == null ? new() : LiquidDensity.Map(),
                    SuperficialTension = SuperficialTension == null ? new() : SuperficialTension.Map(),
                },
                
                _ => default(T)!
            };
        }
        
        public static CompoundProperty Create(IDto dto)
        {
           if(dto is NewCompoundPropertyDTO)
            {
                return Create();
            }
            return null!;
        }
    }

    public class CompoundConstant : Entity
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
