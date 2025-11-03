using Simulator.Shared.NuevaSimlationconQwen;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Client.HCPages.MainProcesses.Process
{
    public partial class SimulationTabs
    {
        [Parameter]
        public GeneralSimulation Simulation { get; set; } = null!;
        protected override void OnInitialized()
        {
            Simulation.UpdateModel = UpdateSimulation;
        }

        int Showvelocity => 500 - velocity > 0 ? 500 - velocity : 1;
        int velocity = 499;
        private void RefreshReportValues()
        {
            // Columnas 1 y 2 (sin cambios)
            foreach (var wrapper in Column1Items)
                wrapper.Items = LiveReportFactory.CreateReport(wrapper.Equipment).Items;

            foreach (var wrapper in Column2Items)
                wrapper.Items = LiveReportFactory.CreateReport(wrapper.Equipment).Items;

            // Columna 3: Actualizar filas planas
            foreach (var row in FlatRows)
            {
                if (row.Wip != null)
                    row.Wip.Items = LiveReportFactory.CreateReport(row.Wip.Equipment).Items;

                if (row.Line != null)
                    row.Line.Items = LiveReportFactory.CreateReport(row.Line.Equipment).Items;
            }
        }

        async Task UpdateSimulation()
        {

            await Task.Delay(Showvelocity);
            RefreshReportValues();
            StateHasChanged();
        }

        async Task StartSimulation()
        {
            await Simulation.RunSimulationAsync();
        }

        void PauseSimulation()
        {
            Simulation.PauseSimulation();
        }

        void ResumeSimulation()
        {
            Simulation.ResumeSimulation();
        }

        void StopSimulation()
        {
            Simulation.StopSimulation();
        }

        void ResetSimulation()
        {
            Simulation.ResetSimulation();
        }

        private List<LiveReportableWrapper> Column1Items { get; set; } = new();
        private List<LiveReportableWrapper> Column2Items { get; set; } = new();
        private List<FlatRow> FlatRows { get; set; } = new(); // ← Reemplaza LineDisplayItems

        private DateTime _lastInitDate = DateTime.MinValue;

        protected override void OnParametersSet()
        {
            if (Simulation?.InitDate != _lastInitDate)
            {
                _lastInitDate = Simulation?.CurrentDate ?? DateTime.MinValue;
                BuildLayoutV2();
            }
        }

        private void BuildLayoutV2()
        {
            var equipments = Simulation.Equipments.ToList();
            var wrappers = equipments.OfType<ILiveReportable>().Select(eq => new LiveReportableWrapper(eq, LiveReportFactory.CreateReport(eq))).ToList();

            // Columna 1: Operarios y Tanques MP
            Column1Items = wrappers
                .Where(x => x.Column == ReportColumn.Column1_OperatorsAndRawMaterialTanks)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.EquipmentName)
                .ToList();

            // Columna 2: SKIDs y Mezcladores
            Column2Items = wrappers
                .Where(x => x.Column == ReportColumn.Column2_SkidsAndMixers)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.EquipmentName)
                .ToList();

            // Columna 3: WIPs y Líneas
            // En BuildLayoutV2()
            var wipWrappers = wrappers
                .Where(x => x.Column == ReportColumn.Column3_WipTanks)
                .OrderByDescending(x => x.Priority == ReportPriorityInColumn.High)
                .ThenBy(x => x.EquipmentName)
                .ToList();

            var lineWrappers = wrappers
                .Where(x => x.Column == ReportColumn.Column4_Lines)
                .OrderBy(x => x.EquipmentName)
                .ToList();

            // Mapear: Línea -> Lista de WIPs
            var lineToWipsMap = new Dictionary<Guid, List<LiveReportableWrapper>>();
            foreach (var lineWrapper in lineWrappers)
            {
                if (lineWrapper.Equipment is ProcessLine processLine)
                {
                    var wipTanksForThisLine = processLine.WIPTanksAttached;
                    var wipsForLine = wipWrappers
                        .Where(w => wipTanksForThisLine.Any(wip => wip.Id == w.Equipment.Id))
                        .ToList();

                    lineToWipsMap[lineWrapper.Id] = wipsForLine;
                }
            }

            // Crear lista plana de filas
            var flatRows = new List<FlatRow>();
            var usedWips = new HashSet<Guid>();

            // Agregar líneas con sus WIPs
            foreach (var lineWrapper in lineWrappers)
            {
                if (lineToWipsMap.TryGetValue(lineWrapper.Id, out var wipsForLine))
                {
                    if (wipsForLine.Any())
                    {
                        // Primera fila: línea + primer WIP
                        flatRows.Add(new FlatRow
                        {
                            Wip = wipsForLine[0],
                            Line = lineWrapper
                        });

                        // Filas adicionales: solo WIPs
                        for (int i = 1; i < wipsForLine.Count; i++)
                        {
                            flatRows.Add(new FlatRow
                            {
                                Wip = wipsForLine[i],
                                Line = null
                            });
                        }
                    }
                    else
                    {
                        // Línea sin WIPs
                        flatRows.Add(new FlatRow
                        {
                            Wip = null,
                            Line = lineWrapper
                        });
                    }

                    foreach (var wip in wipsForLine)
                        usedWips.Add(wip.Id);
                }
            }

            // Agregar WIPs huérfanos
            var orphanWips = wipWrappers.Where(w => !usedWips.Contains(w.Id)).ToList();
            foreach (var wip in orphanWips)
            {
                flatRows.Add(new FlatRow
                {
                    Wip = wip,
                    Line = null
                });
            }

            FlatRows = flatRows; // Nueva propiedad
        }

        public class FlatRow
        {
            public LiveReportableWrapper? Wip { get; set; }
            public LiveReportableWrapper? Line { get; set; }
        }
    }
}
