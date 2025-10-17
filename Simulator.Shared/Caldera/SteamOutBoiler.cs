using UnitSystem;

namespace Caldera
{
    public class SteamOutBoiler : CompoundBase
    {
        WaterProperties properties = new();
        //Datos de entrada Presion y Flujo masico
        public SteamOutBoiler()
        {
            Name = "Steam Out Boiler";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double massenthalpy = properties.EnthalpySatVapPW(presssure);
            double tsat = properties.TSatW(presssure);
            double massdensity = properties.DensSatVapPW(presssure);
            double cpmsas = properties.CpSatVapPW(presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);
            Temperature.SetValue(tsat, TemperatureUnits.Kelvin);
            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);

           
            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

            double volumeflow = massflow / massdensity;

            VolumetricFlow.SetValue(volumeflow, VolumetricFlowUnits.m3_hr);



        }
    }
    public class PurgeOutBoiler : CompoundBase
    {
        WaterProperties properties = new();
        //Datos de entrada Presion y Flujo masico
        public PurgeOutBoiler()
        {
            Name = "Purge Out Boiler";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double massenthalpy = properties.EnthalpySatLiqPW(presssure);
            double tsat = properties.TSatW(presssure);
            double massdensity = properties.DensSatLiqPW(presssure);
            double cpmsas = properties.CpSatLiqPW(presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);
            Temperature.SetValue(tsat, TemperatureUnits.Kelvin);
            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);

    
            double volumetricflow = VolumetricFlow.GetValue(VolumetricFlowUnits.m3_hr);

            double massflow = volumetricflow * massdensity;

            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

           

            MassFlow.SetValue(massflow, MassFlowUnits.Kg_hr);



        }
    }
    public class WaterInletToBoiler : CompoundBase
    {
        WaterProperties properties = new();
        //Datos de entrada Presion y Flujo masico
        public WaterInletToBoiler()
        {
            Name = "Water Inlet to Boiler";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double temperature = Temperature.GetValue(TemperatureUnits.Kelvin);

            double massenthalpy = properties.EnthalpyW(temperature, presssure);

            double massdensity = properties.DensW(temperature, presssure);
            double cpmsas = properties.CpW(temperature, presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);

            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);

           
            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

            double volumeflow = massflow / massdensity;

            VolumetricFlow.SetValue(volumeflow, VolumetricFlowUnits.m3_hr);



        }
        public override void SetMassFlow(double kghr)
        {
            MassFlow.SetValue(kghr,MassFlowUnits.Kg_hr);
        }
        
    }
    public class WaterInletToEconomizador : CompoundBase
    {
        WaterProperties properties = new();
        //Datos de entrada Presion y Flujo masico
        public WaterInletToEconomizador()
        {
            Name = "Water Inlet to Economizador";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double temperature = Temperature.GetValue(TemperatureUnits.Kelvin);

            double massenthalpy = properties.EnthalpyW(temperature, presssure);

            double massdensity = properties.DensW(temperature, presssure);
            double cpmsas = properties.CpW(temperature, presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);

            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);

 
            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

            double volumeflow = massflow / massdensity;

            VolumetricFlow.SetValue(volumeflow, VolumetricFlowUnits.m3_hr);



        }
        public override void SetMassFlow(double kghr)
        {
            MassFlow.SetValue(kghr, MassFlowUnits.Kg_hr);
        }

    }

    public class FreshWater: CompoundBase
    {
        WaterProperties properties = new();
        public FreshWater()
        {
            Name = "Fresh Water";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double temperature = Temperature.GetValue(TemperatureUnits.Kelvin);

            double massenthalpy = properties.EnthalpyW(temperature, presssure);

            double massdensity = properties.DensW(temperature, presssure);
            double cpmsas = properties.CpW(temperature, presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);

            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);


            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

            double volumeflow = massflow / massdensity;

            VolumetricFlow.SetValue(volumeflow, VolumetricFlowUnits.m3_hr);
        }
    }
    public class RecoveredCondensate : CompoundBase
    {
        WaterProperties properties = new();
        public RecoveredCondensate()
        {
            Name = "Recovered Condensate";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double temperature = Temperature.GetValue(TemperatureUnits.Kelvin);

            double massenthalpy = properties.EnthalpyW(temperature, presssure);

            double massdensity = properties.DensW(temperature, presssure);
            double cpmsas = properties.CpW(temperature, presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);

            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);


            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

            double volumeflow = massflow / massdensity;

            VolumetricFlow.SetValue(volumeflow, VolumetricFlowUnits.m3_hr);
        }
    }
    public class SteamToDearetor : CompoundBase
    {
        WaterProperties properties = new();
        //Datos de entrada Presion y Flujo masico
        public SteamToDearetor()
        {
            Name = "Steam To Dearetor";
            MolecularWeight = 18;
        }
        public override void CalculateEnergyChanges()
        {
            double presssure = Pressure.GetValue(PressureUnits.Bar);
            double massenthalpy = properties.EnthalpySatVapPW(presssure);
            double tsat = properties.TSatW(presssure);
            double massdensity = properties.DensSatVapPW(presssure);
            double cpmsas = properties.CpSatVapPW(presssure);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);
            Temperature.SetValue(tsat, TemperatureUnits.Kelvin);
            Density.SetValue(massdensity, MassDensityUnits.Kg_m3);
            SpecificHeat.SetValue(cpmsas, MassEntropyUnits.KJ_Kg_C);

           
            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double entalpyflow = massflow * massenthalpy;

            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);

            double volumeflow = massflow / massdensity;

            VolumetricFlow.SetValue(volumeflow, VolumetricFlowUnits.m3_hr);



        }
    }

}
