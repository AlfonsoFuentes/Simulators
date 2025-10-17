using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators
{
    public class ProcessOperator : ManufactureFeeder, ILiveReportable
    {
        // Propiedades específicas
        public List<ProcessMixer> OutletMixers => OutletEquipments.OfType<ProcessMixer>().ToList();

        // 👇 Define si es para lavado o no
        public override bool IsForWashout { get; set; } = false;

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            OutletState = new FeederAvailableState(this);
        }
        public bool OperatorHasNotRestrictionToInitBatch { get; set; } = true;
        public Amount MaxRestrictionTime { get; set; } = new(TimeUnits.Minute);
    }
}
