using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators
{
    public class ProcessOperator : Equipment, ILiveReportable, IManufactureFeeder
    {
        public List<ProcessMixer> OutletMixers => OutletEquipments.OfType<ProcessMixer>().ToList();
        public ReportColumn ReportColumn => ReportColumn.Column1_OperatorsAndRawMaterialTanks;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.High;

        public bool IsForWashout { get; set; }=false;
        public Amount Flow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public Amount ActualFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public string OcuppiedBy {  get; set; }=string.Empty;
        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            OutletState = new FeederAvailableState(this);
        }
        public List<LiveReportItem> GetLiveReportItems()
        {
            return new List<LiveReportItem>
        {
            new LiveReportItem
            {
                Label = "Operator",
                Value = Name,
                Style = new ReportStyle()
            },
            new LiveReportItem
            {
                Label = "State",
                Value = OutletState?.StateLabel ?? "Unknown",
                Style = GetStateStyle()
            }
        };
        }

        private ReportStyle GetStateStyle()
        {
            return new ReportStyle
            {
                Color = OutletState?.StateLabel.Contains("Starved") == true ? "Red" : "Black",
                FontEmphasis = OutletState?.StateLabel.Contains("Starved") == true ? "Bold" : "Normal"
            };
        }
        public bool IsAnyTankInletStarved()
        {
            
            return false;
        }

        public bool IsInUse()
        {
            if (ProcessFeederManager.IsFeederInUse(this))
            {
                return true;
            }
            return false;
        }
    }


}
