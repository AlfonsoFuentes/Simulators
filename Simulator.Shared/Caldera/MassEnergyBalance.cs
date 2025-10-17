using UnitSystem;

namespace Caldera
{
    public class MassEnergyBalance
    {
        public GasNatural gn { get; set; } = new();
        public Air air { get; set; } = new();
        public GasParaFlamaAdiabatic cgFlama { get; set; } = new("Gases para Flama adiabatica");
        public CombustionGases cgSaliendoCaldera { get; set; } = new("Gases saliendo de caldera");
        public CombustionGases cgSaliendoEconomizador { get; set; } = new("Gases de chimenea");
        public SteamOutBoiler steamproduced { get; set; } = new SteamOutBoiler();
        public PurgeOutBoiler purge { get; set; } = new PurgeOutBoiler();
        public WaterInletToBoiler waterInletToBoiler { get; set; } = new WaterInletToBoiler();
        public WaterInletToEconomizador waterInletToEconomizador { get; set; } = new();
        public EnergyFlow EnthalpyFlowBoilerLosses { get; set; } = new();
        public FreshWater FreshWater { get; set; } = new();
        public RecoveredCondensate RecoveredCondensate { get; set; } = new();
        public SteamToDearetor SteamToDearetor { get; set; } = new();
        double FlujoGN_Nm3_hr { get; set; } = 250;
        public double ExcesoAire { get; set; } = 57;
        public Length Altura { get; set; } = new Length(1000, LengthUnits.Meter);
        public double HumedadRelativa { get; set; } = 70;
        public double HumedadMolar { get; set; } = 0;
        public Temperature TemperatureAmbiente { get; set; } = new Temperature(25, TemperatureUnits.DegreeCelcius);
        public void Calculate()
        {
            FlujoGN_Nm3_hr = gn.VolumetricFlow.GetValue(VolumetricFlowUnits.m3_hr);


            const double VOLUMEN_MOLAR_NORMAL = 22.4; // Nm³/kmol
            double molesTotalesGN = FlujoGN_Nm3_hr / VOLUMEN_MOLAR_NORMAL;
            MolarFlow molarFlow = new MolarFlow(molesTotalesGN, MolarFlowUnits.Kgmol_hr);
            gn.SetMolarFlow(molarFlow);

            CalcularComposicionBaseHumedaddesdeBaseSeca(gn.O2Required);
            CalcularGasesdeCombustion();
            CalculateEnergyStreams();

        }

        public Pressure PresionAtmosferica { get; set; } = new Pressure(101.325, PressureUnits.KiloPascal); // Presión atmosférica en kPa
        public double PresionVapor(double Temp)
        {
            double D = 0;
            double temp;

            temp = Temp * 1.8 + 32;
            if (Temp > 0 && Temp < 60)
                D = Math.Pow(10, 8.10765 - 1750.286 / (Temp + 235));
            if (Temp > 61 && Temp < 150)
                D = Math.Pow(10, 7.96681 - 1668.21 / (Temp + 228));

            return D / 760 * 101.3249966;
        }
        void CalcularHumedadMolar()
        {
            double altura = Altura.GetValue(LengthUnits.Meter); // Convertir altura a metros

            double pAtm = 1 / Math.Exp(altura / (8430.15 - altura * 0.09514)) * 101.3249966;
            PresionAtmosferica.SetValue(pAtm, PressureUnits.KiloPascal);
            double tempAtm = TemperatureAmbiente.GetValue(TemperatureUnits.DegreeCelcius);
            double Pv = PresionVapor(tempAtm);//en KiloPascales
            double PH2O = HumedadRelativa * Pv / 100.0;
            HumedadMolar = PH2O / (pAtm);

        }
        public void CalcularComposicionBaseHumedaddesdeBaseSeca(MolarFlow oxigenMolarFlow)
        {
            // 1. Calcular humedad molar del ambiente
            CalcularHumedadMolar();

            // 2. Obtener flujo molar base seca del O2
            double O2FlujoMolar = oxigenMolarFlow.GetValue(MolarFlowUnits.Kgmol_hr) * (1 + ExcesoAire / 100.0);

            // 3. Calcular N2 según proporción estequiométrica del aire (O2:N2 = 21:79)
            double N2FlujoMolar = O2FlujoMolar * 79 / 21;

            // 4. Flujo total de aire seco
            double AireSecoFlujoMolar = O2FlujoMolar + N2FlujoMolar;

            // 5. H2O según humedad molar
            double H2OFlujoMolar = AireSecoFlujoMolar * HumedadMolar;


            // 7. Asignar flujos molares a cada componente
            air.O2.SetMolarFlow(O2FlujoMolar);
            air.N2.SetMolarFlow(N2FlujoMolar);
            air.H2O.SetMolarFlow(H2OFlujoMolar);

            air.CalculateMassPercentage();

            // 9. Calcular porcentajes molares y másicos

        }
        void CalcularGasesdeCombustion()
        {
            double co2producid = gn.CO2Produced.GetValue(MolarFlowUnits.Kgmol_hr);
            double co2inGas = gn.CO2.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr);
            double o2air = air.O2.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr);
            double o2teorico = gn.O2Required.GetValue(MolarFlowUnits.Kgmol_hr);
            double n2gas = gn.N2.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr);
            double n2air = air.N2.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr);
            double h2oair = air.H2O.MolarFlow.GetValue(MolarFlowUnits.Kgmol_hr);
            double h2oproducido = gn.H2OProduced.GetValue(MolarFlowUnits.Kgmol_hr);

            cgFlama.CO2.SetMolarFlow(co2inGas + co2producid);
            cgFlama.O2.SetMolarFlow(o2air - o2teorico);
            cgFlama.N2.SetMolarFlow(n2gas + n2air);
            cgFlama.H2O.SetMolarFlow(h2oair + h2oproducido);

            cgFlama.CalculateMassPercentage();

            cgSaliendoCaldera.CO2.SetMolarFlow(co2inGas + co2producid);
            cgSaliendoCaldera.O2.SetMolarFlow(o2air - o2teorico);
            cgSaliendoCaldera.N2.SetMolarFlow(n2gas + n2air);
            cgSaliendoCaldera.H2O.SetMolarFlow(h2oair + h2oproducido);

            cgSaliendoCaldera.CalculateMassPercentage();

            cgSaliendoEconomizador.CO2.SetMolarFlow(co2inGas + co2producid);
            cgSaliendoEconomizador.O2.SetMolarFlow(o2air - o2teorico);
            cgSaliendoEconomizador.N2.SetMolarFlow(n2gas + n2air);
            cgSaliendoEconomizador.H2O.SetMolarFlow(h2oair + h2oproducido);

            cgSaliendoEconomizador.CalculateMassPercentage();
            cgSaliendoEconomizador.Temperature.SetValue(150, TemperatureUnits.DegreeCelcius);
            cgSaliendoEconomizador.CalculateEnergyChanges();

        }
        void CalculateEnergyStreams()
        {
            gn.CalculateEnthalpyFlow();

            air.CalculateEnergyChanges();

            ResolverTemperaturaGases(out double tempSolucionK, out double errorFinal);

            SteamMassBalance();
        }
        double Objetivefunction(double tempGuessK)
        {
            var combustibleEnergy = gn.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var airenergy = air.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            cgFlama.Temperature.SetValue(tempGuessK, TemperatureUnits.DegreeCelcius);
            cgFlama.CalculateEnergyChanges();

            double energiaSalida = cgFlama.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var result = energiaSalida - (combustibleEnergy + airenergy);
            return result;
        }
        void ResolverTemperaturaGases(out double tempSolucionK, out double errorFinal)
        {
            double tempInicialK = 1500;
            NewtonRaphsonSolver.IterarParaTemperatura(
                Objetivefunction,
                tempInicialK,
                out tempSolucionK,
                out errorFinal);
        }
        void ResolverTemperaturaEconomizador(out double tempSolucionK, out double errorFinal)
        {
            double tempInicialK = 130;
            NewtonRaphsonSolver.IterarParaTemperatura(
                ObjectivefuncionEcomizador,
                tempInicialK,
                out tempSolucionK,
                out errorFinal);
        }
        void ResolverTemperaturaBoiler(out double tempSolucionK, out double errorFinal)
        {
            double tempInicialK = 280;
            NewtonRaphsonSolver.IterarParaTemperatura(
                ObjectivefuncionBoiler,
                tempInicialK,
                out tempSolucionK,
                out errorFinal);
        }
        void ResolverVaporDearetor(out double massdearetor, out double errorFinal)
        {
            double massInicial = 450;
            NewtonRaphsonSolver.IterarParaTemperatura(
                funcionObjetivoDesaireador,
                massInicial,
                out massdearetor,
                out errorFinal);
        }
        void SteamMassBalance()
        {
            steamproduced.CalculateEnergyChanges();
            purge.Pressure = steamproduced.Pressure;
            purge.CalculateEnergyChanges();
            waterInletToBoiler.Pressure=steamproduced.Pressure;
            var manometricpressure = steamproduced.Pressure.GetValueManometric(PressureUnits.psi);
            waterInletToEconomizador.Pressure.SetValueManometric(manometricpressure - 5, PressureUnits.psi);
            //Suposicion Temperatura Entrada Agua
            double massinlet = purge.MassFlow.GetValue(MassFlowUnits.Kg_hr) + steamproduced.MassFlow.GetValue(MassFlowUnits.Kg_hr);

            waterInletToBoiler.MassFlow.SetValue(massinlet, MassFlowUnits.Kg_hr);

            waterInletToBoiler.CalculateEnergyChanges();

            waterInletToEconomizador.MassFlow.SetValue(massinlet, MassFlowUnits.Kg_hr);


            waterInletToEconomizador.CalculateEnergyChanges();

            ResolverTemperaturaBoiler(out double tempSolucionK, out double errorFinal);
            CalcularGasesNoJustificada();
            FreshWater.CalculateEnergyChanges();
            ResolverVaporDearetor(out double masssteam, out double errorfinal);


        }
        void CalcularGasesNoJustificada()
        {
            var energyWaterInletEconomizador = waterInletToEconomizador.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energywateroutleEconomizador = waterInletToBoiler.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);

            var energygasesInletEconomizador = cgSaliendoCaldera.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energygasesOutletEconomizador = cgSaliendoEconomizador.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energywater = energywateroutleEconomizador - energyWaterInletEconomizador;
            var energygases = energygasesInletEconomizador - energygasesOutletEconomizador;

            var result = energygases - energywater;
            EnthalpyFlowBoilerLosses.SetValue(result, EnergyFlowUnits.KJ_hr);
        }
        double ObjectivefuncionEcomizador(double tempGuessK)
        {
            double result = 0;
            cgSaliendoEconomizador.Temperature.SetValue(tempGuessK, TemperatureUnits.DegreeCelcius);
            cgSaliendoEconomizador.CalculateEnergyChanges();

            var energyWaterInletEconomizador = waterInletToEconomizador.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energywateroutleEconomizador = waterInletToBoiler.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);

            var energygasesInletEconomizador = cgSaliendoCaldera.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energygasesOutletEconomizador = cgSaliendoEconomizador.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);




            var energywater = energywateroutleEconomizador - energyWaterInletEconomizador;
            var energygases = energygasesInletEconomizador - energygasesOutletEconomizador;
            result = energywater - energygases;


            return result;
        }
        double ObjectivefuncionBoiler(double tempGuessK)
        {
            double result = 0;
            cgSaliendoCaldera.Temperature.SetValue(tempGuessK, TemperatureUnits.DegreeCelcius);
            cgSaliendoCaldera.CalculateEnergyChanges();


            var energywatersteamBoiler = steamproduced.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energywaterpurge = purge.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energywaterinletboiler = waterInletToBoiler.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);

            var energywater = energywatersteamBoiler + energywaterpurge - energywaterinletboiler;

            var energygasesFlame = cgFlama.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);
            var energygasesSaliendoBoiler = cgSaliendoCaldera.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);


            var energygases = energygasesFlame - energygasesSaliendoBoiler;
            result = energygases - energywater;
            return result;
        }
        double funcionObjetivoDesaireador(double massflowsteam)
        {
            double result = 0;
            var massouletDeartor = waterInletToEconomizador.MassFlow.GetValue(MassFlowUnits.Kg_hr);
            var energyouletDeartor = waterInletToEconomizador.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);

            var massouletfreshwater = FreshWater.MassFlow.GetValue(MassFlowUnits.Kg_hr);
            var energyfreshwater = FreshWater.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);


            SteamToDearetor.MassFlow.SetValue(massflowsteam, MassFlowUnits.Kg_hr);
            SteamToDearetor.CalculateEnergyChanges();
            var energysteamdearotor = SteamToDearetor.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);

            var massrecoveredcondensate = massouletDeartor - massouletfreshwater - massflowsteam;

            RecoveredCondensate.MassFlow.SetValue(massrecoveredcondensate, MassFlowUnits.Kg_hr);
            RecoveredCondensate.CalculateEnergyChanges();
            var energyrecoveredcondensate = RecoveredCondensate.EnthalpyFlow.GetValue(EnergyFlowUnits.KJ_hr);

            result = energyouletDeartor - (energyrecoveredcondensate + energysteamdearotor + energyfreshwater);
            return result;
        }

    }
}
