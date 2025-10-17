using UnitSystem;

namespace Caldera
{
    public abstract class CompoundList: CompoundBase
    {
        public virtual List<Compound> List { get; } = new List<Compound>();

       

       
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
        public void CalculateMassPercentage()
        {
            double massflow = List.Sum(x => x.MassFlow.GetValue(MassFlowUnits.Kg_hr));
            double molarflow = List.Sum(x => x.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr));
            List.ForEach(x =>
            {
                x.Mass_Percentage = x.MassFlow.GetValue(MassFlowUnits.Kg_hr) / massflow;
                x.Molar_Percentage = x.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr) / molarflow;
            });
            MassFlow.SetValue(massflow, MassFlowUnits.Kg_hr);
            MolarFlow.SetValue(molarflow, MolarFlowUnits.Kgmol_hr);
        }
        public void CalculateDensity()
        {
            double presion = Pressure.GetValue(PressureUnits.Atmosphere);
            double temperatura = Temperature.GetValue(TemperatureUnits.Kelvin);
            double R = 0.082;//lt*atm/mol K
            double densidad = presion * MolecularWeight / (temperatura * R);
            Density.SetValue(densidad, MassDensityUnits.g_L);

        }
    }
}
