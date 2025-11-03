using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;

namespace Simulator.Shared.NuevaSimlationconQwen.Reports
{
    public enum ReportColumn
    {
        Column1_OperatorsAndRawMaterialTanks,
        Column2_SkidsAndMixers,
        Column3_WipTanks,
        Column4_Lines
    }

    public enum ReportPriorityInColumn
    {
        High = 1, // ← Arriba en la columna
        Medium = 2,
        Low = 3   // ← Abajo en la columna
    }
    public interface ILiveReportable
    {
        Guid Id { get; } // ← Agrega esta línea



    }
    public class LiveReportItem
    {
        public string Label { get; set; } = string.Empty; // ← Leyenda (ej: "State", "Level")
        public string Value { get; set; } = string.Empty;// ← Valor actual como string (con unidades)
        public ReportStyle Style { get; set; } = new ReportStyle(); // ← Estilo para la UI
    }
    public class ReportStyle
    {
        public string Color { get; set; } = "Black";
        public string FontEmphasis { get; set; } = "Normal";
        public string BackgroundColor { get; set; } = "Transparent";
    }




    // Clase de metadatos para el reporte
    public class EquipmentReportMetadata
    {
        public required ReportColumn Column { get; init; }
        public required ReportPriorityInColumn Priority { get; init; }
        public required List<LiveReportItem> Items { get; init; }
    }

    // Fábrica centralizada
    public static class LiveReportFactory
    {
        public static EquipmentReportMetadata CreateReport(ILiveReportable equipmentreportable)
        {
            if (equipmentreportable is IEquipment equipment)
            {
                return equipment switch
                {
                    ProcessOperator op => CreateOperatorReport(op),
                    ProcessRecipedTank receipedtank => CreatRecipedMaterialTankReport(receipedtank),
                    ProcessBaseTankForRawMaterial tank => CreateRawMaterialTankReport(tank),
                    ProcessContinuousSystem skid => CreateSkidReport(skid),
                    ProcessMixer mixer => CreateMixerReport(mixer),
                    ProcessWipTankForLine wip => CreateWipTankReport(wip),
                    ProcessLine line => CreateProcessLineReport(line),
                    _ => CreateDefaultReport(equipment)
                };
            }
            return null!;

        }

        // =============== OPERADORES ===============
        private static EquipmentReportMetadata CreateOperatorReport(ProcessOperator op)
        {
            var reports = new List<LiveReportItem>
        {
                new() { Label = "Operator", Value = op.Name, Style = new() },
                new() { Label = "State", Value = op.OutletState?.StateLabel ?? "Unknown", Style = GetOperatorStateStyle(op) }
        };
            if (op.WaitingQueue.Count > 0)
            {
                reports.Add(new LiveReportItem
                {
                    Label = "Waiting Queue",
                    Value = op.WaitingQueue.Count.ToString(),
                    Style = new() { Color = "Orange" }
                });
                foreach (var eq in op.WaitingQueue)
                {
                    reports.Add(new LiveReportItem
                    {
                        Label = "Equipment",
                        Value = eq.Name,
                        Style = new() { Color = "Orange" }
                    });
                }
            }
            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column1_OperatorsAndRawMaterialTanks,
                Priority = ReportPriorityInColumn.High,
                Items = reports,
            };
        }

        private static ReportStyle GetOperatorStateStyle(ProcessOperator op)
        {
            return op.OutletState?.StateLabel.Contains("Starved") == true
                ? new() { Color = "Red", FontEmphasis = "Bold" }
                : new() { Color = "Black" };
        }

        // =============== TANQUES DE MATERIA PRIMA ===============
        private static EquipmentReportMetadata CreateRawMaterialTankReport(ProcessBaseTankForRawMaterial tank)
        {
            var reports = new List<LiveReportItem>
        {
            new() { Label = "Tank", Value = tank.Name, Style = new() },
            new() { Label = "Level", Value = $"{Math.Round(tank.CurrentLevel.GetValue(MassUnits.KiloGram))} Kg", Style = GetTankLevelStyle(tank) },
            new() { Label = "State", Value = tank.OutletState?.StateLabel ?? "Unknown", Style = GetTankLevelStyle(tank) }
        };

            foreach (var outlet in tank.OutletPumps.OrderBy(x => x.Name))
            {
                reports.Add(new LiveReportItem
                {
                    Label = outlet.Name,
                    Value = outlet.OutletState?.StateLabel ?? "Unknown",
                    Style = new()
                });
                if (outlet.WaitingQueue.Count > 0)
                {
                    reports.Add(new LiveReportItem
                    {
                        Label = "Waiting Queue",
                        Value = outlet.WaitingQueue.Count.ToString(),
                        Style = new() { Color = "Orange" }
                    });
                    foreach (var eq in outlet.WaitingQueue)
                    {
                        reports.Add(new LiveReportItem
                        {
                            Label = "Equipment",
                            Value = eq.Name,
                            Style = new() { Color = "Orange" }
                        });
                    }
                }
            }

            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column1_OperatorsAndRawMaterialTanks,
                Priority = ReportPriorityInColumn.Low,
                Items = reports
            };
        }
        private static EquipmentReportMetadata CreatRecipedMaterialTankReport(ProcessRecipedTank tank)
        {
            var reports = new List<LiveReportItem>
        {
            new() { Label = "Tank", Value = tank.Name, Style = new() },
            new() { Label = "Level", Value = $"{Math.Round(tank.CurrentLevel.GetValue(MassUnits.KiloGram))} Kg", Style = GetTankLevelStyle(tank) },
            new() { Label = "State", Value = tank.OutletState?.StateLabel ?? "Unknown", Style = GetTankLevelStyle(tank) }
        };

            foreach (var outlet in tank.OutletPumps.OrderBy(x => x.Name))
            {
                reports.Add(new LiveReportItem
                {
                    Label = outlet.Name,
                    Value = outlet.OutletState?.StateLabel ?? "Unknown",
                    Style = new()
                });
                if (outlet.WaitingQueue.Count > 0)
                {
                    reports.Add(new LiveReportItem
                    {
                        Label = "Waiting Queue",
                        Value = outlet.WaitingQueue.Count.ToString(),
                        Style = new() { Color = "Orange" }
                    });
                    foreach (var eq in outlet.WaitingQueue)
                    {
                        reports.Add(new LiveReportItem
                        {
                            Label = "Equipment",
                            Value = eq.Name,
                            Style = new() { Color = "Orange" }
                        });
                    }
                }
            }
            if (tank.CurrentTankManufactureOrder != null)
            {
                if (tank.CurrentTankManufactureOrder.TimeToEmptyMassInProcess.Value > 0)
                {
                    reports.Add(new LiveReportItem
                    {
                        Label = "Time Empty Vessel",
                        Value = $"{Math.Round(tank.CurrentTankManufactureOrder.TimeToEmptyMassInProcess.GetValue(TimeUnits.Minute), 2)}, min",
                        Style = new()
                    });
                }
                if (tank.CurrentTankManufactureOrder.ManufactureOrdersFromMixers.Count > 0)
                {
                    foreach (var order in tank.CurrentTankManufactureOrder.ManufactureOrdersFromMixers)
                    {
                        reports.Add(new LiveReportItem
                        {
                            Label = "Mixer",
                            Value = $"{order.ManufaturingEquipment.Name}, {order.BatchSize.GetValue(MassUnits.KiloGram)}, kg",
                            Style = new()
                        });

                    }

                }

            }


            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column1_OperatorsAndRawMaterialTanks,
                Priority = ReportPriorityInColumn.Medium,
                Items = reports
            };
        }

        private static ReportStyle GetTankLevelStyle(ProcessBaseTank tank)
        {
            if (tank.CurrentLevel < tank.LoLolevel) return new() { Color = "Red", FontEmphasis = "Bold" };
            if (tank.CurrentLevel < tank.LoLevel) return new() { Color = "Orange" };
            return new() { Color = "Green" };
        }


        // =============== SKIDS ===============
        private static EquipmentReportMetadata CreateSkidReport(ProcessContinuousSystem skid)
        {
            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column2_SkidsAndMixers,
                Priority = ReportPriorityInColumn.High,
                Items = new List<LiveReportItem>
            {
                new() { Label = "SKID", Value = skid.Name, Style = new() },
                new() { Label = "Outlet State", Value = skid.OutletState?.StateLabel ?? "Unknown", Style = new() },
                new() { Label = "Inlet State", Value = skid.InletState?.StateLabel ?? "Unknown", Style = new() },
                new() { Label = "Product", Value = skid.CurrentMaterial?.CommonName ?? "Unknown", Style = new() },
                new() { Label = "Flow", Value = skid.ActualFlow.ToString() ?? "Unknown", Style = new() }
            }
            };
        }

        // =============== MEZCLADORES ===============
        private static EquipmentReportMetadata CreateMixerReport(ProcessMixer mixer)
        {
            var items = new List<LiveReportItem>
        {
            new() { Label = "Mixer", Value = mixer.Name, Style = new() },
            new() { Label = "Current Level", Value = mixer.CurrentLevel.ToString(), Style = new() },
            new() { Label = "Last Material", Value = mixer.LastMaterial?.CommonName ?? "No previous material", Style = new() }
        };

            if (mixer.CurrentTransferRequest != null && mixer.CurrentManufactureOrder != null)
            {
                items.AddRange(new[]
                {
                new LiveReportItem { Label = "Transfering to", Value = mixer.CurrentTransferRequest.DestinationWip.Name, Style = new() },
                new LiveReportItem { Label = "Material", Value = mixer.CurrentManufactureOrder.Material.CommonName, Style = new() },
                new LiveReportItem { Label = "State", Value = mixer.OutletState?.StateLabel??"null", Style = new() }
            });
            }
            else if (mixer.CurrentManufactureOrder != null)
            {
                items.AddRange(new[]
                {
                new LiveReportItem { Label = "Batch Size", Value = mixer.CurrentManufactureOrder.BatchSize.ToString(), Style = new() },
                new LiveReportItem { Label = "Material", Value = mixer.CurrentManufactureOrder.Material.CommonName, Style = new() },
                new LiveReportItem { Label = "Producing To", Value = mixer.CurrentManufactureOrder.WIPOrder.Tank.Name, Style = new() },
                new LiveReportItem { Label = "Step", Value = mixer.InletState ?.StateLabel ?? "null", Style = new() }
            });

                if (mixer.InletState is IShortLabelInletStateMixer label && !string.IsNullOrEmpty(label.ShortLabel))
                {
                    items.Add(new LiveReportItem { Label = "Step State", Value = label.ShortLabel, Style = new() });
                }

                items.AddRange(new[]
                {
                new LiveReportItem { Label = "Current Batch Time", Value = mixer.CurrentManufactureOrder.CurrentBatchTime.ToString(), Style = new() },
                new LiveReportItem { Label = "Starved Time", Value = mixer.CurrentManufactureOrder.CurrentStarvedTime.ToString(), Style = new() { Color = "Orange", FontEmphasis = "Bold" } }
            });
            }
            if (mixer.ManufacturingOrders.Count > 0)
            {
                items.Add(new LiveReportItem { Label = "Queued Batches", Value = mixer.ManufacturingOrders.Count.ToString(), Style = new() });
                foreach (var order in mixer.ManufacturingOrders)
                {
                    items.Add(new LiveReportItem { Label = "Next Scheduled", Value = $"{order.Material.CommonName} to {order.WIPOrder.Tank.Name}", Style = new() });
                }
            }
            if (mixer.ProcessOperator != null && !mixer.ProcessOperator.OperatorHasNotRestrictionToInitBatch)
            {
                items.Add(new LiveReportItem { Label = "Operator Attached", Value = mixer.ProcessOperator.Name, Style = new() });
                if (mixer.ProcessOperator.MaxRestrictionTime.Value > 0)
                {
                    items.Add(new LiveReportItem { Label = "Operator will be realease in: ", Value = $"{Math.Round(mixer.OperatorStarvedPendingTime.GetValue(TimeUnits.Minute), 2)}, min" , Style = new() });
                }
                else
                {
                    items.Add(new LiveReportItem { Label = "Operator will be realease", Value = $"When init Transfer to WIP", Style = new() });
                }
            }

            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column2_SkidsAndMixers,
                Priority = ReportPriorityInColumn.Low,
                Items = items
            };
        }

        // =============== TANQUES WIP ===============
        private static EquipmentReportMetadata CreateWipTankReport(ProcessWipTankForLine wip)
        {
            var items = new List<LiveReportItem>
        {
            new() { Label = "WIP Tank", Value = wip.Name, Style = new() },
            new() { Label = "Capacity", Value = wip.Capacity.ToString(), Style = new() },
            new() { Label = "Level", Value = wip.CurrentLevel.ToString(), Style = GetWipLevelStyle(wip) },
            new() { Label = "Outlet State", Value = wip.OutletState?.StateLabel ?? "Unknown", Style = new() },
            new() { Label = "Inlet State", Value = wip.InletState?.StateLabel ?? "Unknown", Style = new() }
        };

            if (wip.CurrentOrder != null)
            {
                items.AddRange(new[]
                {
                new LiveReportItem { Label = "Producing to", Value = wip.CurrentOrder.LineName ?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Material", Value = wip.CurrentOrder.MaterialName ?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Mass delivered", Value = wip.CurrentOrder.MassDelivered.ToString() ?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Mass Pending to deliver", Value = wip.CurrentOrder.MassPendingToDeliver.ToString() ?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Mass Pending to produce", Value = wip.CurrentOrder.MassPendingToProduce.ToString() ?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Mass Produced", Value = wip.CurrentOrder.MassProduced.ToString() ?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Average Outlet flow", Value = $"{Math.Round(wip.CurrentOrder.AverageOutletFlow.GetValue(MassFlowUnits.Kg_min),2)}, Kg/min"?? "Unknown", Style = new() },
                new LiveReportItem { Label = "Time to empty Mass in process", Value = wip.CurrentOrder.TimeToEmptyMassInProcess.ToString() ?? "Unknown", Style = new() }
            });
                if (wip.CurrentOrder.ManufactureOrdersFromMixers.Count > 0)
                {
                    items.Add(new LiveReportItem { Label = "Batches in Order", Value = wip.CurrentOrder.ManufactureOrdersFromMixers.Count.ToString(), Style = new() });
                    foreach (var order in wip.CurrentOrder.ManufactureOrdersFromMixers)
                    {
                        items.Add(new LiveReportItem { Label = "Batch", Value = $"{order.ManufaturingEquipment.Name}, {order.BatchSize.GetValue(MassUnits.KiloGram)} kg", Style = new() });
                    }
                }

            }

            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column3_WipTanks,
                Priority = ReportPriorityInColumn.Low,
                Items = items
            };
        }

        private static ReportStyle GetWipLevelStyle(ProcessWipTankForLine wip)
        {
            if (wip.CurrentLevel < wip.LoLolevel) return new() { Color = "Red", FontEmphasis = "Bold" };
            if (wip.CurrentLevel < wip.LoLevel) return new() { Color = "Orange" };
            return new() { Color = "Green" };
        }

        // =============== LÍNEAS ===============
        private static EquipmentReportMetadata CreateProcessLineReport(ProcessLine line)
        {
            var items = new List<LiveReportItem>
        {
            new() { Label = "Line", Value = line.Name, Style = new() },
            new() { Label = "State", Value = line.OutletState?.StateLabel ?? "Unknown", Style = GetLineStateStyle(line) },

        };

            if (line.CurrentProductionOrder != null && line.CurrentProductionOrder.ProductionSKURun != null)
            {
                items.AddRange(new[]
                {
                new LiveReportItem { Label = "BackBone", Value = line.CurrentProductionOrder.SKU?.Material.CommonName ?? "None", Style = new() },
                new LiveReportItem { Label = "Planned Cases", Value = line.CurrentProductionOrder.ProductionSKURun?.PlannedCases.ToString() ?? "None", Style = new() },
                new LiveReportItem { Label = "Produced Cases", Value = line.CurrentProductionOrder.ProductionSKURun?.ProducedCases.ToString() ?? "None", Style = new() },
                new LiveReportItem { Label = "Pending Cases", Value = line.CurrentProductionOrder.ProductionSKURun?.RemainingCases.ToString() ?? "None", Style = new() },
                new LiveReportItem { Label = "Current Flow", Value = line.CurrentProductionOrder.ProductionSKURun?.CurrentFlow.ToString() ?? "None", Style = new() },
                new LiveReportItem { Label = "Average Flow", Value = $"{Math.Round(line.CurrentProductionOrder.ProductionSKURun?.AverageMassFlow.GetValue(MassFlowUnits.Kg_min) ?? 0, 2)} Kg/min", Style = new() },
                new LiveReportItem { Label = "Planned mass", Value = line.CurrentProductionOrder.ProductionSKURun?.TotalPlannedMass.ToString() ?? "None", Style = new() },
                new LiveReportItem { Label = "Mass packed", Value = $"{Math.Round(line.CurrentProductionOrder.ProductionSKURun?.ProducedMass.GetValue(MassUnits.KiloGram) ?? 0, 1)} Kg", Style = new() },
                new LiveReportItem { Label = "Pending Mass", Value = line.CurrentProductionOrder.ProductionSKURun?.RemainingMass.ToString() ?? "None", Style = new() }
            });

                var wipNames = string.Join(", ", line.CurrentProductionOrder.WIPs.Select(w => w.Name));
                items.Add(new LiveReportItem { Label = "Receiving from: ", Value = wipNames, Style = new() });
            }

            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column4_Lines,
                Priority = ReportPriorityInColumn.Low,
                Items = items
            };
        }

        private static ReportStyle GetLineStateStyle(ProcessLine line)
        {
            return line.OutletState switch
            {
                IStarvedLine => new() { Color = "Red", FontEmphasis = "Bold" },
                IProducerAUState => new() { Color = "Orange", FontEmphasis = "Bold" },
                IProducerState => new() { Color = "Green" },
                _ => new() { Color = "Gray" }
            };
        }

        // =============== REPORTE POR DEFECTO ===============
        private static EquipmentReportMetadata CreateDefaultReport(IEquipment equipment)
        {
            return new EquipmentReportMetadata
            {
                Column = ReportColumn.Column1_OperatorsAndRawMaterialTanks,
                Priority = ReportPriorityInColumn.Low,
                Items = new List<LiveReportItem>
            {
                new() { Label = "Type", Value = equipment.GetType().Name, Style = new() { Color = "Gray" } },
                new() { Label = "Name", Value = equipment.Name, Style = new() }
            }
            };
        }
    }

    // Clase auxiliar para la UI
    public class LiveReportableWrapper
    {
        public Guid Id { get; }
        public ReportColumn Column { get; }
        public ReportPriorityInColumn Priority { get; }
        public List<LiveReportItem> Items { get; set; }
        public ILiveReportable Equipment { get; } // ← Nueva propiedad
        public string EquipmentName => Equipment is IEquipment eq ? eq.Name : "Unknown";

        public LiveReportableWrapper(ILiveReportable equipment, EquipmentReportMetadata metadata)
        {
            Id = equipment.Id;
            Column = metadata.Column;
            Priority = metadata.Priority;
            Items = metadata.Items;
            Equipment = equipment; // ← Almacenar referencia
        }
    }
}
