using UnitSystem;

namespace Caldera
{
    public class CombustionGases : CompoundList
    {
        public Compound CO2 = new("Carbon Dioxide", "CO2", 44.01, 36.11, 4.233, -2.887, 7.464, 2);
        public Compound O2 = new("Oxigen", "O2", 32, 29.1, 1.158, -0.6076, 1.311, 2);
        public Compound N2 = new("Nitrogen", "N2", 28.02, 29, 0.2199, 0.5723, -2.871, 2);
        public CompundH2O H2O = new("Water", "H2O", 18.016, 33.46, 0.688, 0.7604, -3.593, 2);
        public CombustionGases(string name)
        {
            Name = name;

            List.Add(CO2);
            List.Add(O2);
            List.Add(N2);
            List.Add(H2O);
        }
    }
    public class GasParaFlamaAdiabatic : CombustionGases
    {
        WaterProperties water = new();

        public GasParaFlamaAdiabatic(string name):base(name) 
        {
          
        }
        public override void CalculateEnergyChanges()
        {
            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);

            double masscp = 0;
            double massenthalpy = 0;
            foreach (var item in List)
            {
                item.Temperature = Temperature;
                item.CalculateEnergyChanges();
                masscp += item.Mass_Percentage * item.SpecificHeat.GetValue(MassEntropyUnits.KJ_Kg_C);
                massenthalpy += item.Mass_Percentage * item.MassEntalpy.GetValue(MassEnergyUnits.KJ_Kg);
            }

            SpecificHeat.SetValue(masscp, MassEntropyUnits.KJ_Kg_C);
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);
            double energyflow = massflow * massenthalpy;
            EnthalpyFlow.SetValue(energyflow, EnergyFlowUnits.KJ_hr);
            CalculateDensity();
        }
    }
}
