using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.CompoundProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Simulator.Shared.NewModels.Compounds
{
    public class NewCompoundPropertyDTO    :Dto
    {
        public string Name {  get; set; }=string.Empty;
        public string Formula { get; set; } = string.Empty;
        public string StructuralFormula { get; set; } = string.Empty;
        public string MainFamily { get; set; } = string.Empty;
        public string SecondaryFamily { get; set; } = string.Empty;
        public double MolecularWeight { get; set; }
        public double Critical_Z { get; set; }
        public double Acentric_Factor { get; set; }
        public double Acentric_Factor_SRK { get; set; }
        [JsonIgnore]
        public Amount Critical_Temperature { get; set; } = new Amount(TemperatureUnits.Kelvin);
        public string Critical_Temperature_Unit
        {
            get
            {
                return Critical_Temperature.UnitName;
            }
            set
            {
                if (Critical_Temperature == null) Critical_Temperature = new Amount(value);
                Critical_Temperature.UnitName = value;
            }
        }
        public double Critical_Temperature_Value
        {
            get
            {
                return Critical_Temperature.GetValue(Critical_Temperature.Unit);
            }
            set
            {
                if (Critical_Temperature == null) Critical_Temperature = new Amount(Critical_Temperature_Unit);
                Critical_Temperature.SetValue(value, Critical_Temperature.Unit);
            }
        }
        [JsonIgnore]
        public Amount Critical_Pressure { get; set; } = new Amount(PressureUnits.KiloPascal);
        public string Critical_Pressure_Unit
        {
            get
            {
                return Critical_Pressure.UnitName;
            }
            set
            {
                if (Critical_Pressure == null) Critical_Pressure = new Amount(value);
                Critical_Pressure.UnitName = value;
            }
        }
        public double Critical_Pressure_Value
        {
            get
            {
                return Critical_Pressure.GetValue(Critical_Pressure.Unit);
            }
            set
            {
                if (Critical_Pressure == null) Critical_Pressure = new Amount(Critical_Pressure_Unit);
                Critical_Pressure.SetValue(value, Critical_Pressure.Unit);
            }
        }

        [JsonIgnore]
        public Amount Critical_Volume { get; set; } = new Amount(MolarVolumeSpecificUnits.cm3_gmol);
        public string Critical_Volume_Unit
        {
            get
            {
                return Critical_Volume.UnitName;
            }
            set
            {
                if (Critical_Volume == null) Critical_Volume = new Amount(value);
                Critical_Volume.UnitName = value;
            }
        }
        public double Critical_Volume_Value
        {
            get
            {
                return Critical_Volume.GetValue(Critical_Volume.Unit);
            }
            set
            {
                if (Critical_Volume == null) Critical_Volume = new Amount(Critical_Volume_Unit);
                Critical_Volume.SetValue(value, Critical_Volume.Unit);
            }
        }

        [JsonIgnore]
        public Amount Boiling_Temperature { get; set; } = new Amount(TemperatureUnits.Kelvin);
        public string Boiling_Temperature_Unit
        {
            get
            {
                return Boiling_Temperature.UnitName;
            }
            set
            {
                if (Boiling_Temperature == null) Boiling_Temperature = new Amount(value);
                Boiling_Temperature.UnitName = value;
            }
        }
        public double Boiling_Temperature_Value
        {
            get
            {
                return Boiling_Temperature.GetValue(Boiling_Temperature.Unit);
            }
            set
            {
                if (Boiling_Temperature == null) Boiling_Temperature = new Amount(Boiling_Temperature_Unit);
                Boiling_Temperature.SetValue(value, Boiling_Temperature.Unit);
            }
        }
        [JsonIgnore]

        public Amount Melting_Temperature { get; set; } = new Amount(TemperatureUnits.Kelvin);
        public string Melting_Temperature_Unit
        {
            get
            {
                return Melting_Temperature.UnitName;
            }
            set
            {
                if (Melting_Temperature == null) Melting_Temperature = new Amount(value);
                Melting_Temperature.UnitName = value;
            }
        }
        public double Melting_Temperature_Value
        {
            get
            {
                return Melting_Temperature.GetValue(Melting_Temperature.Unit);
            }
            set
            {
                if (Melting_Temperature == null) Melting_Temperature = new Amount(Melting_Temperature_Unit);
                Melting_Temperature.SetValue(value, Melting_Temperature.Unit);
            }
        }
        [JsonIgnore]

        public Amount Asterisk_Volume { get; set; } = new Amount(MolarVolumeSpecificUnits.cm3_gmol);
        public string Asterisk_Volume_Unit
        {
            get
            {
                return Asterisk_Volume.UnitName;
            }
            set
            {
                if (Asterisk_Volume == null) Asterisk_Volume = new Amount(value);
                Asterisk_Volume.UnitName = value;
            }
        }
        public double Asterisk_Volume_Value
        {
            get
            {
                return Asterisk_Volume.GetValue(Asterisk_Volume.Unit);
            }
            set
            {
                if (Asterisk_Volume == null) Asterisk_Volume = new Amount(Asterisk_Volume_Unit);
                Asterisk_Volume.SetValue(value, Asterisk_Volume.Unit);
            }
        }
        [JsonIgnore]

        public Amount Gibbs_Energy_Formation { get; set; } = new Amount(MolarEnergyUnits.KJ_gmol);
        public string Gibbs_Energy_Formation_Unit
        {
            get
            {
                return Gibbs_Energy_Formation.UnitName;
            }
            set
            {
                if (Gibbs_Energy_Formation == null) Gibbs_Energy_Formation = new Amount(value);
                Gibbs_Energy_Formation.UnitName = value;
            }
        }
        public double Gibbs_Energy_Formation_Value
        {
            get
            {
                return Gibbs_Energy_Formation.GetValue(Gibbs_Energy_Formation.Unit);
            }
            set
            {
                if (Gibbs_Energy_Formation == null) Gibbs_Energy_Formation = new Amount(Gibbs_Energy_Formation_Unit);
                Gibbs_Energy_Formation.SetValue(value, Gibbs_Energy_Formation.Unit);
            }
        }
        [JsonIgnore]

        public Amount Enthalpy_Formation { get; set; } = new Amount(MolarEnergyUnits.KJ_gmol);
        public string Enthalpy_Formation_Unit
        {
            get
            {
                return Enthalpy_Formation.UnitName;
            }
            set
            {
                if (Enthalpy_Formation == null) Enthalpy_Formation = new Amount(value);
                Enthalpy_Formation.UnitName = value;
            }
        }
        public double Enthalpy_Formation_Value
        {
            get
            {
                return Enthalpy_Formation.GetValue(Enthalpy_Formation.Unit);
            }
            set
            {
                if (Enthalpy_Formation == null) Enthalpy_Formation = new Amount(Enthalpy_Formation_Unit);
                Enthalpy_Formation.SetValue(value, Enthalpy_Formation.Unit);
            }
        }
        [JsonIgnore]

        public Amount Entropy_Formation { get; set; } = new Amount(MolarEntropyUnits.KJ_Kgmol_C);
        public string Entropy_Formation_Unit
        {
            get
            {
                return Entropy_Formation.UnitName;
            }
            set
            {
                if (Entropy_Formation == null) Entropy_Formation = new Amount(value);
                Entropy_Formation.UnitName = value;
            }
        }
        public double Entropy_Formation_Value
        {
            get
            {
                return Entropy_Formation.GetValue(Entropy_Formation.Unit);
            }
            set
            {
                if (Entropy_Formation == null) Entropy_Formation = new Amount(Entropy_Formation_Unit);
                Entropy_Formation.SetValue(value, Entropy_Formation.Unit);
            }
        }

        [JsonIgnore]

        public Amount Enthalpy_Combustion { get; set; } = new Amount(MolarEntropyUnits.KJ_Kgmol_C);
        public string Enthalpy_Combustion_Unit
        {
            get
            {
                return Enthalpy_Combustion.UnitName;
            }
            set
            {
                if (Enthalpy_Combustion == null) Enthalpy_Combustion = new Amount(value);
                Enthalpy_Combustion.UnitName = value;
            }
        }
        public double Enthalpy_Combustion_Value
        {
            get
            {
                return Enthalpy_Combustion.GetValue(Enthalpy_Combustion.Unit);
            }
            set
            {
                if (Enthalpy_Combustion == null) Enthalpy_Combustion = new Amount(Enthalpy_Combustion_Unit);
                Enthalpy_Combustion.SetValue(value, Enthalpy_Combustion.Unit);
            }
        }

        public CompoundConstantDTO VapourPressure { get; set; } = new();

        public CompoundConstantDTO HeatOfVaporization { get; set; } = new();

        public CompoundConstantDTO LiquidCp { get; set; } = new();

        public CompoundConstantDTO GasCp { get; set; } = new();

        public CompoundConstantDTO LiquidViscosity { get; set; } = new();

        public CompoundConstantDTO GasViscosity { get; set; } = new();


        public CompoundConstantDTO LiquidThermalConductivity { get; set; } = new();

        public CompoundConstantDTO GasThermalConductivity { get; set; } = new();

        public CompoundConstantDTO LiquidDensity { get; set; } = new();

        public CompoundConstantDTO SuperficialTension { get; set; } = new();
    }
}
