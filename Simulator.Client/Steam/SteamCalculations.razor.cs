using Caldera;
using UnitSystem;

namespace Simulator.Client.Steam;
public partial class SteamCalculations
{
    MassEnergyBalance balance = new();
    void Calcular()
    {
        balance.Calculate();
    }
    protected override void OnInitialized()
    {
        balance.gn.VolumetricFlow.SetValue(296.48, VolumetricFlowUnits.m3_hr);
        balance.gn.Temperature.SetValue(25, TemperatureUnits.DegreeCelcius);
        balance.air.Temperature.SetValue(25, TemperatureUnits.DegreeCelcius);
        balance.steamproduced.Pressure.SetValueManometric(129.82, PressureUnits.psi);
        balance.steamproduced.MassFlow.SetValue(4313.18, MassFlowUnits.Kg_hr);

        balance.purge.VolumetricFlow.SetValue(0.135, VolumetricFlowUnits.m3_hr);

        balance.waterInletToBoiler.Temperature.SetValue(135, TemperatureUnits.DegreeCelcius);

        balance.waterInletToEconomizador.Temperature.SetValue(105, TemperatureUnits.DegreeCelcius);

        balance.SteamToDearetor.Pressure.SetValueManometric(5, PressureUnits.psi);
        balance.RecoveredCondensate.Temperature.SetValue(90, TemperatureUnits.DegreeCelcius);
        balance.RecoveredCondensate.Pressure.SetValueManometric(2, PressureUnits.Bar);
        balance.FreshWater.Temperature.SetValue(25, TemperatureUnits.DegreeCelcius);
        balance.FreshWater.Pressure.SetValueManometric(2, PressureUnits.Bar);
        balance.FreshWater.MassFlow.SetValue(49.71, MassFlowUnits.Kg_min);
        balance.cgSaliendoEconomizador.Temperature.SetValue(150, TemperatureUnits.DegreeCelcius);


        StateHasChanged();
        base.OnInitialized();
    }
}
