using UnitSystem;

namespace Caldera
{
    public class GasNatural : CombustibleList
    {
        public Combustible metano = new Combustible("Methane", "CH4", 16.041, 83.2789 / 100.0, 890.4, 34.31, 5.469, 0.3661, -11, 2);
        public Combustible etano = new Combustible("Ethane", "C2H6", 30.07, 10.2605 / 100.0, 1559.9, 49.37, 13.92, -5.816, 7.280, 2);
        public Combustible propano = new Combustible("Propane", "C3H8", 44.09, 3.1277 / 100.0, 2220, 68.032, 22.59, -13.11, 31.71, 2);
        public Combustible i_butano = new Combustible("i-Butane", "iC4H10", 58.12, 0.4609 / 100.0, 2868.8, 89.46, 30.13, -18.91, 49.87, 2);
        public Combustible n_butano = new Combustible("n-Butane", "nC4H10", 58.12, 0.4316 / 100.0, 2878.52, 92.30, 27.88, -15.47, 34.98, 2);
        public Combustible i_pentano = new Combustible("i-Pentane", "iC5H12", 72.15, 0.1093 / 100.0, 3536.15, 114.8, 34.09, -18.99, 42.26, 2);
        public Combustible n_pentano = new Combustible("n-Pentane", "nC5H12", 72.15, 0.0506 / 100.0, 3536.15, 114.8, 34.09, -18.99, 42.26, 2);
        public Combustible n_hexano = new Combustible("n-Hexane", "C6H14", 86.17, 0.0280 / 100.0, 4194.753, 137.44, 40.85, -23.92, 57.66, 2);
        public Combustible N2 = new("Nitrogen", "N2", 28.02, 0.4709 / 100.0, 0, 29, 0.2199, 0.5723, -2.871, 2);
        public Combustible CO2 = new("Carbon Dioxide", "CO2", 44.01, 1.7816 / 100.0, 0, 36.11, 4.233, -2.887, 7.464, 2);

        public GasNatural()
        {
            Name = "Natural Gas Valle del Cauca";

            GasesList.Add(metano);
            GasesList.Add(etano);
            GasesList.Add(propano);
            GasesList.Add(i_butano);
            GasesList.Add(n_butano);
            GasesList.Add(n_pentano);
            GasesList.Add(i_pentano);
            GasesList.Add(n_hexano);
            GasesList.Add(N2);
            GasesList.Add(CO2);
            CalculateMolecularWeight();
            CalculateDensityNormalCondictions();
            CalculateEnthalpy();



        }
        public void SetMolarFlow(MolarFlow molarFlow)
        {
            MolarFlow = molarFlow;
            double molarflow = MolarFlow.GetValue(MolarFlowUnits.gmol_hr);
            double massflow = 0;
            foreach (var item in GasesList)
            {
                double molesi = molarflow * item.Molar_Percentage;
                MolarFlow molarflowi = new MolarFlow(molesi, MolarFlowUnits.gmol_hr);
                item.SetMolarFlow(molarflowi);
                double massflowi = item.MassFlow.GetValue(MassFlowUnits.Kg_hr);
                massflow += massflowi;

            }
            MassFlow.SetValue(massflow, MassFlowUnits.Kg_hr);
        }
        void CalculateMolecularWeight()
        {

            double molar = List.Sum(x => x.Molar_Percentage);
            List.ForEach(x =>
            {
                x.Molar_Percentage = x.Molar_Percentage / molar;

            });
            molar = List.Sum(x => x.Molar_Percentage);
            double mass = List.Sum(x => x.Molar_Percentage * x.MolecularWeight);
            List.ForEach(x =>
            {

                x.Mass_Percentage = x.Molar_Percentage * x.MolecularWeight / mass;

            });

            MolecularWeight = mass / molar;
        }
        void CalculateEnthalpy()
        {
            double masentalpy = List.Sum(x => x.Mass_Percentage * x.MassEntalpy.GetValue(MassEnergyUnits.KJ_Kg));


            MassEntalpy.SetValue(masentalpy, MassEnergyUnits.KJ_Kg);
            double densitynormalconditions = DensityNormalConditions.GetValue(MassDensityUnits.Kg_m3);
            double volumenenthalpi = masentalpy * densitynormalconditions;
            VolumeEnthalpy.SetValue(volumenenthalpi, VolumeEnergyUnits.KJ_m3);
            double btup3 = VolumeEnthalpy.GetValue(VolumeEnergyUnits.BTU_ft3);
        }
        void CalculateDensityNormalCondictions()
        {
            double P = 101.325; // kPa – condición normal
            double T = 273.15;  // K – condición normal
            double R = 8.314;   // kJ/(kmol·K)
            double M = MolecularWeight; // g/mol → kg/kmol

            double rho = (P * M) / (R * T); // kg/m³
            DensityNormalConditions.SetValue(rho, MassDensityUnits.Kg_m3);
        }
        public void CalculateEnthalpyFlow()
        {
            double massflow = MassFlow.GetValue(MassFlowUnits.Kg_hr);
            double massentalpy = MassEntalpy.GetValue(MassEnergyUnits.KJ_Kg);
            double entalpyflow = massentalpy * massflow;
            EnthalpyFlow.SetValue(entalpyflow, EnergyFlowUnits.KJ_hr);
        }
    }
}
