using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public abstract class ProcessBaseTankForRawMaterial : ProcessBaseTank, ISetMaterialsAtOutlet,ILiveReportable
    {
        public ReportColumn ReportColumn => ReportColumn.Column1_OperatorsAndRawMaterialTanks;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.Low;

        public List<LiveReportItem> GetLiveReportItems()
        {
            var reports = new List<LiveReportItem>()
            {
                new LiveReportItem
            {
                Label = "Tank",
                Value = Name,
                Style = new ReportStyle()
            },
            new LiveReportItem
            {
                Label = "Level",
                Value = $"{Math.Round(CurrentLevel.GetValue(MassUnits.KiloGram))} Kg",
                Style = GetLevelStyle()
            },
            new LiveReportItem
            {
                Label = "State",
                Value = $"{OutletState?.StateLabel}"??"Unknown",
                Style = GetLevelStyle()
            },
            };
            foreach (var outlet in OutletPumps)
            {
                reports.Add(new LiveReportItem()
                {
                    Label = outlet.Name,
                    Value = outlet.OutletState?.StateLabel ?? "Unknown",
                    Style = new ReportStyle()

                });
            }
            return reports;

        }

        private ReportStyle GetLevelStyle()
        {
            if (CurrentLevel < LoLolevel) return new ReportStyle { Color = "Red", FontEmphasis = "Bold" };
            if (CurrentLevel < LoLevel) return new ReportStyle { Color = "Orange" };
            return new ReportStyle { Color = "Green" };
        }

        private ReportStyle GetStateStyle() => new ReportStyle();
        public virtual void SetMaterialsAtOutlet(IMaterial material)
        {
            foreach (var outlet in OutletPumps)
            {
                outlet.AddMaterial(material);

            }
        }
        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            CurrentLevel = InitialLevel;
            OutletState = new TankOutletInitializeTankState(this);



        }
        

    }


}
