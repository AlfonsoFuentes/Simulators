using UnitSystem;

namespace Caldera
{
    public class Combustible : Compound
    {

        public MolarFlow O2Required { get; set; } = new MolarFlow(MolarFlowUnits.Kgmol_hr);
        public MolarFlow CO2Produced { get; set; } = new MolarFlow(MolarFlowUnits.Kgmol_hr);
        public MolarFlow H2OProduced { get; set; } = new MolarFlow(MolarFlowUnits.Kgmol_hr);
        public Combustible(
            string nombre,
            string formula,
            double pm,
            double porc_v,
            double molarcombustion,
            double a, double b, double c, double d, int temperatureformula) : base(nombre, formula, pm, a, b, c, d, temperatureformula)
        {
            var massenergy = molarcombustion / pm;
            MassEntalpy.SetValue(massenergy, MassEnergyUnits.KJ_g);
            Molar_Percentage = porc_v;
            SetMolarFlow();



        }
        private (int C, int H, int O) ParsearFormula(string formula)
        {
            int C = 0, H = 0, O = 0;

            if (formula.StartsWith("i") || formula.StartsWith("n"))
            {
                formula = formula.Remove(0, 1);
            }
            if (formula.Contains("CH")) C = 1;
            else if (formula.StartsWith("C2")) C = 2;
            else if (formula.StartsWith("C3")) C = 3;
            else if (formula.StartsWith("C4")) C = 4;
            else if (formula.StartsWith("C5")) C = 5;
            else if (formula.StartsWith("C6")) C = 6;

            var hPart = formula.Split('H');
            if (hPart.Length > 1)
            {
                string hStr = "";
                foreach (char c in hPart[1])
                {
                    if (char.IsDigit(c))
                        hStr += c;
                    else break;
                }

                if (int.TryParse(hStr, out int h))
                    H = h;
            }

            return (C, H, O);
        }
        public void SetMolarFlow(MolarFlow molarflow = null!)
        {
            double localmolarflow = 1;
            if (molarflow != null)
            {
                MolarFlow = molarflow;
                localmolarflow = MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr);

            }


            double massflow = localmolarflow * MolecularWeight;
            MassFlow.SetValue(massflow, MassFlowUnits.Kg_hr);
            var (C, H, _) = ParsearFormula(Formula);

            if (C > 0 && H > 0)
            {
                double o2_requerido = localmolarflow * (C + H / 4.0);
                double co2_producido = localmolarflow * C;
                double h2o_producido = localmolarflow * H / 2.0;
                O2Required.SetValue(o2_requerido, MolarFlowUnits.Kgmol_hr);
                CO2Produced.SetValue(co2_producido, MolarFlowUnits.Kgmol_hr);
                H2OProduced.SetValue(h2o_producido, MolarFlowUnits.Kgmol_hr);

            }
        }
    }
}
