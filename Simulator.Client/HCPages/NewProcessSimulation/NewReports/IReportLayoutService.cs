using Simulator.Shared.NuevaSimlationconQwen;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Client.HCPages.NewProcessSimulation.NewReports
{
    public interface IReportLayoutService
    {
        List<ReportLayoutItem> GetLayout();
    }

    public class ReportLayoutItem
    {
        public ILiveReportable Equipment { get; set; } = null!;
        public int RowIndex { get; set; } // ← Para alineación vertical
        public ReportColumn Column { get; set; }
    }
    public class ReportLayoutService : IReportLayoutService
    {
        private readonly GeneralSimulation _simulation;

        public ReportLayoutService(GeneralSimulation simulation)
        {
            _simulation = simulation;
        }

        public List<ReportLayoutItem> GetLayout()
        {
            var reportable = _simulation.Equipments.OfType<ILiveReportable>().ToList();
            var layout = new List<ReportLayoutItem>();

            // Procesar columnas en orden
            foreach (var column in new[] {
            ReportColumn.Column1_OperatorsAndRawMaterialTanks,
            ReportColumn.Column2_SkidsAndMixers,
            ReportColumn.Column3_WipTanks,
            ReportColumn.Column4_Lines
        })
            {
                var items = reportable.Where(e => e.ReportColumn == column).ToList();

                // Para columna 3 (WIP), ordenar: SKIDs arriba, luego por tipo de producto
                if (column == ReportColumn.Column3_WipTanks)
                {
                    items = OrderWipTanks(items);
                }

                // Asignar índices de fila
                for (int i = 0; i < items.Count; i++)
                {
                    layout.Add(new ReportLayoutItem
                    {
                        Equipment = items[i],
                        RowIndex = i,
                        Column = column
                    });
                }
            }

            // Alinear líneas con su primer tanque WIP
            AlignLinesWithWipTanks(layout);

            return layout;
        }

        private List<ILiveReportable> OrderWipTanks(List<ILiveReportable> wipTanks)
        {
            var skidWips = new List<ILiveReportable>();
            var lineWips = new List<ILiveReportable>();

            foreach (var wip in wipTanks)
            {
                if (wip is ProcessWipTankForLine wipTank)
                {
                    var skid = wipTank.InletSKIDS.FirstOrDefault();
                    if (skid != null)
                    {
                        skidWips.Add(wip);
                    }
                    else
                    {
                        lineWips.Add(wip);
                    }
                }
            }

            // Ordenar WIP de líneas por tipo de producto
            var productTypeOrder = new Dictionary<string, int>
        {
            { "botellas", 1 },
            { "doypacks", 2 },
            { "sachets", 3 },
            { "tubos", 4 }
        };

           

            return skidWips.Concat(lineWips).ToList();
        }

        private void AlignLinesWithWipTanks(List<ReportLayoutItem> layout)
        {
            var wipItems = layout.Where(x => x.Column == ReportColumn.Column3_WipTanks).ToList();
            var lineItems = layout.Where(x => x.Column == ReportColumn.Column4_Lines).ToList();

            foreach (var lineItem in lineItems)
            {
                if (lineItem.Equipment is ProcessLine line)
                {
                    var wipTanks = line.InletPumps.SelectMany(p => p.InletWipTanks).Distinct().ToList();
                    if (wipTanks.Any())
                    {
                        var firstWip = wipTanks.First();
                        var wipLayoutItem = layout.FirstOrDefault(x => x.Equipment is ProcessWipTankForLine w && w.Id == firstWip.Id);
                        if (wipLayoutItem != null)
                        {
                            lineItem.RowIndex = wipLayoutItem.RowIndex; // ← Alinear con el primer WIP
                        }
                    }
                }
            }
        }
    }
}
