using UnitSystem;

namespace Caldera
{
    public class CombustibleList : CompoundList
    {
       
        public MassDensity DensityNormalConditions { get; private set; } = new MassDensity(MassDensityUnits.Kg_m3);
        public VolumeEnergy VolumeEnthalpy { get; private set; } = new VolumeEnergy(VolumeEnergyUnits.KJ_m3);
        public override List<Compound> List => GasesList.Select(x=>x as Compound).ToList();
        public List<Combustible> GasesList { get; set; } = new();
        public MolarFlow O2Required => new MolarFlow(GasesList.Sum(x => x.O2Required.GetValue(MolarFlowUnits.Kgmol_hr)), MolarFlowUnits.Kgmol_hr);
        public MolarFlow CO2Produced => new MolarFlow(GasesList.Sum(x => x.CO2Produced.GetValue(MolarFlowUnits.Kgmol_hr)), MolarFlowUnits.Kgmol_hr);
        public MolarFlow H2OProduced => new MolarFlow(GasesList.Sum(x => x.H2OProduced.GetValue(MolarFlowUnits.Kgmol_hr)), MolarFlowUnits.Kgmol_hr);


    }
}
