using UnitSystem;

namespace Caldera
{
    public abstract class CompoundBase
    {
        public string Name { get; init; } = string.Empty;
        public MassFlow MassFlow { get; set; } = new MassFlow(MassFlowUnits.Kg_hr);
        public MolarFlow MolarFlow { get; set; } = new MolarFlow(MolarFlowUnits.Kgmol_hr);
        public Temperature Temperature { get; set; } = new Temperature(TemperatureUnits.DegreeCelcius);
        public MassEntropy SpecificHeat { get; private set; } = new MassEntropy(MassEntropyUnits.KJ_Kg_C);
        public MassEnergy MassEntalpy { get; private set; } = new MassEnergy(MassEnergyUnits.KJ_Kg); // kJ/kg
        public EnergyFlow EnthalpyFlow { get; private set; } = new EnergyFlow(EnergyFlowUnits.KJ_hr); // kJ/hr
        public double MolecularWeight { get; set; } // Peso molecular en g/mol
        public Pressure Pressure { get; set; } = new Pressure(PressureUnits.Atmosphere);
        public MassDensity Density { get; private set; } = new MassDensity(MassDensityUnits.Kg_m3);
        public VolumetricFlow VolumetricFlow { get;  set; } = new VolumetricFlow(VolumetricFlowUnits.m3_hr);
        public abstract void CalculateEnergyChanges();
        public virtual void SetMolarFlow(double kgmolhr)
        {

        }
        public virtual void SetMassFlow(double kghr)
        {

        }
    }
    public class Compound : CompoundBase
    {
        public int temperatureformula = 0;

        public string Formula { get; init; } = string.Empty;



        double _Molar_Percentage = 0;
        double _Mass_Percentage = 0;
        public double Molar_Percentage
        {
            get { return _Molar_Percentage; }
            set
            {
                _Molar_Percentage = value;
            }
        }
        public double Mass_Percentage
        {
            get { return _Mass_Percentage; }
            set { _Mass_Percentage = value; }
        }



        //Cp=a+b(T)+c(T)^2+d(T)^3
        public double A { get; set; } //a
        public double B { get; set; } //b*102
        public double C { get; set; } //c*105
        public double D { get; set; } //d*109

        public Compound(
            string nombre,
            string formula,
            double pm,
            double a, double b, double c, double d, int temperatureformula)
        {
            Name = nombre;
            Formula = formula;
            A = a;
            B = b * 1e-2;
            C = c * 1e-5;
            D = d * 1e-9;
            this.temperatureformula = temperatureformula;
            MolecularWeight = pm;
        }
        public override void CalculateEnergyChanges()
        {
            double actualTemp = temperatureformula == 1 ? Temperature.GetValue(TemperatureUnits.Kelvin) : Temperature.GetValue(TemperatureUnits.DegreeCelcius);
            double cpkjmol = A + B * actualTemp + C * Math.Pow(actualTemp, 2) + D * Math.Pow(actualTemp, 3);
            double cpkjgr = cpkjmol / MolecularWeight;

            SpecificHeat.SetValue(cpkjgr, MassEntropyUnits.KJ_Kg_C);
            double TREF = temperatureformula == 1 ? 298.15 : 25;

            double molarenthalpy =
                A * (actualTemp - TREF) +
                B * (Math.Pow(actualTemp, 2) - Math.Pow(TREF, 2)) / 2 +
                C * (Math.Pow(actualTemp, 3) - Math.Pow(TREF, 3)) / 3 +
                D * (Math.Pow(actualTemp, 4) - Math.Pow(TREF, 4)) / 4;
            double massenthalpy = molarenthalpy / MolecularWeight;
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);


        }
        public override void SetMolarFlow(double kgmolhr)
        {
            MolarFlow.SetValue(kgmolhr, MolarFlowUnits.Kgmol_hr);
            MassFlow.SetValue(kgmolhr * MolecularWeight, MassFlowUnits.Kg_hr);
        }


    }
    public class CompundH2O : Compound
    {
        WaterProperties wp = new();
        public double AL { get; set; } = 18.2964;
        public double BL { get; set; } = 47.212 * 1e-2;
        public double CL { get; set; } = -133.88 * 1e-5;
        public double DL { get; set; } = 1314.2 * 1e-9;
        public CompundH2O(string nombre, string formula, double pm, double a, double b, double c, double d, int temperatureformula) : base(nombre, formula, pm, a, b, c, d, temperatureformula)
        {
        }
        public override void CalculateEnergyChanges()
        {
            var TempC = Temperature.GetValue(TemperatureUnits.DegreeCelcius);
            var TempK = Temperature.GetValue(TemperatureUnits.Kelvin);
            double cpkjmol = 0;
            double molarenthalpy = 0;
            double TREFK = 298.15;
        
            if (TempC <= 100)
            {

                cpkjmol = A + B * TempK + C * Math.Pow(TempK, 2) + D * Math.Pow(TempK, 3);
                molarenthalpy =
                AL * (TempK - TREFK) +
                BL * (Math.Pow(TempK, 2) - Math.Pow(TREFK, 2)) / 2 +
                CL * (Math.Pow(TempK, 3) - Math.Pow(TREFK, 3)) / 3 +
                DL * (Math.Pow(TempK, 4) - Math.Pow(TREFK, 4)) / 4;
            }
            else
            {
                double templC = 100;
                double templK = templC + 273.15;
                cpkjmol = A + B * TempC + C * Math.Pow(TempC, 2) + D * Math.Pow(TempC, 3);
                var molarentalpylwp = wp.EnthalpyW(templK, 4) * MolecularWeight;
               
                var molarenthalpyl =
                       AL * (templK - TREFK) +
                       BL * (Math.Pow(templK, 2) - Math.Pow(TREFK, 2)) / 2 +
                       CL * (Math.Pow(templK, 3) - Math.Pow(TREFK, 3)) / 3 +
                       DL * (Math.Pow(templK, 4) - Math.Pow(TREFK, 4)) / 4;
                var me = (wp.EnthalpySatVapTW(templK) - wp.EnthalpySatLiqTW(templK));
                var molarenthalpyv = me * MolecularWeight;

                var molarenthalpyg =
                A * (TempC - templC) +
                B * (Math.Pow(TempC, 2) - Math.Pow(templC, 2)) / 2 +
                C * (Math.Pow(TempC, 3) - Math.Pow(templC, 3)) / 3 +
                D * (Math.Pow(TempC, 4) - Math.Pow(templC, 4)) / 4;
                var molarentalpygwp = wp.EnthalpyW(150 + 273.15, 1.03215) * MolecularWeight; 
                molarenthalpy = molarenthalpyl + molarenthalpyv + molarenthalpyg;

            }


            double cpkjgr = cpkjmol / MolecularWeight;

            SpecificHeat.SetValue(cpkjgr, MassEntropyUnits.KJ_Kg_C);



            double massenthalpy = molarenthalpy / MolecularWeight;
            MassEntalpy.SetValue(massenthalpy, MassEnergyUnits.KJ_Kg);


        }
    }
}
