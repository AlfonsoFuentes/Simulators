using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators
{
    public class ProcessOperator : ManufactureFeeder, ILiveReportable
    {
        // Propiedades específicas
        public List<ProcessMixer> OutletMixers =>OutletEquipments.OfType<ProcessMixer>().ToList();

        public ReportColumn ReportColumn => ReportColumn.Column1_OperatorsAndRawMaterialTanks;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.High;

        // 👇 Define si es para lavado o no
        public override bool IsForWashout { get; set; } = false;

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            OutletState = new FeederAvailableState(this);
        }

        public override bool IsAnyTankInletStarved()
        {
            // ProcessOperator no tiene tanques de entrada que lo starven
            return false;
        }
    }
}
