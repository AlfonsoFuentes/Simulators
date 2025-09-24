using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines
{
    public class ProcessLine : Equipment, ILiveReportable
    {

        public List<ProcessMixer> PreferredManufacturer { get; set; } = new();

        public Amount CurrentSKUPlannedMass => CurrentSKU?.TotalPlannedMass ?? new(0, MassUnits.KiloGram);
        public Amount NextSKUPlannedMass => NextSKU?.TotalPlannedMass ?? new(0, MassUnits.KiloGram);
        public Amount PlannedMass => IsNextMaterialSame ? CurrentSKUPlannedMass + NextSKUPlannedMass : CurrentSKUPlannedMass;
        bool IsNextMaterialSame => CurrentSKU == null || NextSKU == null ? false : CurrentSKU?.Material.Id == NextSKU?.Material.Id;

        public Amount TimeToChangeFormat => CurrentSKU == null ? new Amount(0, TimeUnits.Second) : CurrentSKU.TimeToChangeFormat;


        private ProductionSKURun? ProductionSKURun { get; set; } = null!;
        ProcessSKUByLine? CurrentSKU { get; set; }
        ProcessSKUByLine? NextSKU { get; set; }


        private List<ProcessPump>? _inletPumps;
        private List<ProcessWipTankForLine>? _wipTanks;

        public List<ProcessPump> InletPumps => _inletPumps ??= InletEquipments.OfType<ProcessPump>().ToList();
        public List<ProcessWipTankForLine> WIPTanksAttached => _wipTanks ??= InletPumps.SelectMany(x => x.InletWipTanks).ToList();
        //Aqui hay que poner el miezclador de corrientes
        Queue<ProcessSKUByLine> QueueSKUs { get; set; } = new();
        public ShiftType ShiftType { get; set; } = ShiftType.Shift_1_2_3;
        public CurrentShift ActualShift { get; set; }
        public override void ValidateOutletInitialState(DateTime currentdate)
        {

            ActualShift = GetCurrentShift(currentdate);
            OutletState = new LineStateInitialState(this);
            SetPumpsFlowToZeroAtInit();




        }

        public CurrentShift GetCurrentShift(DateTime date)
        {
            return date.Hour switch
            {
                >= 6 and < 14 => CurrentShift.Shift_1,   // 6am - 2pm
                >= 14 and < 22 => CurrentShift.Shift_2,  // 2pm - 10pm
                _ => CurrentShift.Shift_3                // 10pm - 6am
            };
        }
        public override void BeforeRun(DateTime currentdate)
        {
            ActualShift = GetCurrentShift(currentdate);


        }
        public override void AfterRun(DateTime currentdate)
        {
            if (OutletState is IProducerState)
            {
                SetPumpsFlowToProduce();
            }
            else
            {
                SetPumpsFlowToZero();
            }
        }
        public void SetPumpsFlowToZeroAtInit()
        {
            InletPumps.ForEach(x => x.ActualFlow = ZeroFlow);
        }



        public TimeSpan? GetTimeToNextScheduledShift(DateTime currentdate)
        {
            var shiftStartHours = new Dictionary<CurrentShift, int>
    {
        { CurrentShift.Shift_1, 6 },   // 6 AM
        { CurrentShift.Shift_2, 14 },  // 2 PM
        { CurrentShift.Shift_3, 22 }   // 10 PM
    };

            var now = currentdate;
            var today = now.Date;

            // Recopilar todos los posibles inicios de turno (hoy y mañana) que estén programados
            var upcomingStartTimes = new List<DateTime>();

            foreach (var shift in Enum.GetValues<CurrentShift>())
            {
                if (IsScheduledForShift(shift)) // ← Ya lo tienes implementado
                {
                    var hour = shiftStartHours[shift];
                    var todayStart = today.AddHours(hour);
                    if (todayStart > now)
                        upcomingStartTimes.Add(todayStart);

                    var tomorrowStart = today.AddDays(1).AddHours(hour);
                    upcomingStartTimes.Add(tomorrowStart);
                }
            }

            // Ordenar y tomar el más cercano
            var nextStart = upcomingStartTimes
                .OrderBy(x => x)
                .FirstOrDefault(x => x > now);

            if (nextStart == default) return null;

            return nextStart - now;
        }

        public void PrepareNextSKU()
        {
            // Si la cola está vacía, limpiamos referencias
            if (QueueSKUs.Count == 0)
            {
                CurrentSKU = null;
                NextSKU = null;
                return;
            }

            // Extraer el primer SKU (Dequeue)
            CurrentSKU = QueueSKUs.Dequeue();
            ProductionSKURun = new ProductionSKURun(CurrentSKU);
            NextSKU = QueueSKUs.Count > 0 ? QueueSKUs.Peek() : null;
        }
        public void QueueSKU(ProcessSKUByLine sku)
        {
            QueueSKUs.Enqueue(sku);
            FromLineToWipProductionOrder orderforwips = new(this, sku.Material, sku.TotalPlannedMass);

            ProductionOrders.Enqueue(orderforwips);
        }
        public bool IsScheduledForShift(CurrentShift currentShift) => currentShift switch
        {

            CurrentShift.Shift_1 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_1_2 => true,
                ShiftType.Shift_1_3 => true,
                ShiftType.Shift_1 => true,
                _ => false,
            },
            CurrentShift.Shift_2 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_1_2 => true,
                ShiftType.Shift_2_3 => true,
                ShiftType.Shift_2 => true,
                _ => false,
            },
            CurrentShift.Shift_3 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_2_3 => true,
                ShiftType.Shift_1_3 => true,
                ShiftType.Shift_3 => true,
                _ => false
            },

            _ => false
        };


        public ReportColumn ReportColumn => ReportColumn.Column4_Lines;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.Low;

        public List<LiveReportItem> GetLiveReportItems()
        {
            var items = new List<LiveReportItem>
        {
            new LiveReportItem
            {
                Label = "Line",
                Value = Name,
                Style = new ReportStyle()
            },
            new LiveReportItem
            {
                Label = "State",
                Value = OutletState?.StateLabel ?? "Unknown",
                Style = GetStateStyle()
            },
            new LiveReportItem
            {
                Label = "SKU",
                Value = CurrentSKU?.SkuName ?? "None",
                Style = new ReportStyle()
            }
            

        };
            if (ProductionOrder != null && ProductionSKURun != null)
            {
                items.Add(new LiveReportItem
                {
                    Label = "BackBone",
                    Value = CurrentSKU?.Material.CommonName ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Planned Cases",
                    Value = ProductionSKURun?.TotalCases.ToString() ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Produced Cases",
                    Value = ProductionSKURun?.ProducedCases.ToString() ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Pending Cases",
                    Value = ProductionSKURun?.RemainingCases.ToString() ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Current Flow",
                    Value = ProductionSKURun?.CurrentFlow.ToString() ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Average Flow",
                    Value = $"{Math.Round(ProductionSKURun?.AverageMassFlow.GetValue(MassFlowUnits.Kg_min) ?? 0, 2)}, Kg/min" ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Planned mass",
                    Value = ProductionSKURun?.TotalPlannedMass.ToString() ?? "None",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Mass packed",
                    Value = $"{Math.Round(ProductionSKURun?.ProducedMass.GetValue(MassUnits.KiloGram) ?? 0, 1)}, Kg" ?? "None",
                    Style = new ReportStyle()
                });
              
                items.Add(new LiveReportItem
                {
                    Label = "Pending Mass",
                    Value = ProductionSKURun?.RemainingMass.ToString() ?? "None",
                    Style = new ReportStyle()
                });
                var wipNames = string.Join(", ", ProductionOrder.WIPs.Select(w => w.Name));
                items.Add(new LiveReportItem
                {
                    Label = "Receiving from: ",
                    Value = wipNames,
                    Style = new ReportStyle()
                });

            }
            // ✅ Indicar qué tanques WIP alimentan esta línea


            return items;
        }

        private ReportStyle GetStateStyle()
        {
            return OutletState switch
            {
                IStarvedLine => new ReportStyle { Color = "Red", FontEmphasis = "Bold" },
                IProducerAUState => new ReportStyle { Color = "Orange", FontEmphasis = "Bold" },
                IProducerState => new ReportStyle { Color = "Green" },
                _ => new ReportStyle { Color = "Gray" }
            };
        }


        public bool IsLineScheduled => QueueSKUs.Any();

        Queue<FromLineToWipProductionOrder> ProductionOrders { get; set; } = new Queue<FromLineToWipProductionOrder>();
        FromLineToWipProductionOrder ProductionOrder { get; set; } = null!;

        public bool SelectProductionRun()
        {
            if (QueueSKUs.Any())
            {
                CurrentSKU = QueueSKUs.Dequeue();
                ProductionOrder = ProductionOrders.Dequeue();
                ProductionSKURun = new ProductionSKURun(CurrentSKU);
                NextSKU = QueueSKUs.Count > 0 ? QueueSKUs.Peek() : null;

                SendToWipProductionOrder(ProductionOrder);

                return true;
            }

            return false;

        }
        void SendToWipProductionOrder(FromLineToWipProductionOrder order)
        {

            foreach (var wip in WIPTanksAttached)
            {
                wip.ReceiveFromLineProductionOrder(order);
            }
        }
        bool RealesedOrderFromWIP = false;
        public void ReceivedWIPCurrentOrderRealsed()
        {
            RealesedOrderFromWIP = true;
        }
        public bool IsOrderFromWIPRealsed()
        {
            if (RealesedOrderFromWIP)
            {
                RealesedOrderFromWIP = false;
                return true;
            }
            return false;
        }
        public void ReceiveWipCanHandleMaterial(ProcessWipTankForLine wip)
        {

            if (ProductionOrder != null)
            {
                ProductionOrder.WIPs.Add(wip);

            }
        }

        public bool IsLineStarvedByLowLevelWips()
        {
            if (ProductionOrder != null)
            {
                if (!ProductionOrder.WIPs.Any())
                {
                    StartCriticalReport(this, $"No Manufaturer found for {ProductionOrder.MaterialName}", $"Line {Name} stopped due to low level in WIP tank(s).");
                    return true;
                }
                var wipstarved = ProductionOrder.WIPs.FirstOrDefault(x => x.OutletState is ITankOuletStarved);
                if (wipstarved != null)
                {
                    StartCriticalReport(wipstarved, "Starved by Low Level WIP", $"Line {Name} stopped due to low level in WIP tank(s).");

                    // ✅ Iniciar reporte crítico y obtener su Id


                    return true;
                }

            }


            return false;
        }
        public bool IsLineStarvedByLowLevelWipsWhenEmptyTankToChangeMaterial()
        {
            if (ProductionOrder != null)
            {
                var wipstarved = ProductionOrder.WIPs.All(x => x.OutletState is ITankOuletStarved);
                if (wipstarved)
                {
                    ProductionOrder.WIPs.ForEach(x => x.CurrentLevel = ZeroMass);

                    return true;
                }

            }


            return false;
        }

        public bool MustRunByAu()
        {
            if (ProductionSKURun == null) return true;
            if (ProductionSKURun.IsRunningAU) return false;
            if (ProductionSKURun!.Pending_Time_Producing <= ZeroTime)
            {
                return true;
            }
            return false;

        }
        public bool IsPlannedDowntime()
        {
            return CheckStatusForPlannedDowntime();
        }
        public bool IsPlannedDowntimeAchieved()
        {
            return CheckStatusForPlannedDowntime();
        }
        public bool MustRunProducing()
        {
            if (ProductionSKURun == null) return true;
            if (!ProductionSKURun.IsRunningAU) return false;
            if (ProductionSKURun!.Pending_Time_StarvedByAU <= ZeroTime)
            {
                return true;
            }
            return false;

        }
        public bool IsLineAvailableAfterStarved()
        {
            if (ProductionOrder != null)
            {
                if (ProductionOrder.WIPs.All(x => x.OutletState is not ITankOuletStarved))
                {
                    EndCriticalReport();
                    return true;
                }


            }


            return false;
        }
        public void RunByAu()
        {
            ProductionSKURun?.ProcessDuringAU();

        }
        public void RunByProducing()
        {
            ProductionSKURun?.Produce();

        }

        List<ProcessPump> CurrentPumps => ProductionOrder == null ? new List<ProcessPump>() : ProductionOrder.WIPs.SelectMany(x => x.OutletPumps).ToList();
        public void SetPumpsFlowToZero()
        {
            if (ProductionSKURun != null)
                CurrentPumps.ForEach(x => x.ActualFlow = ZeroFlow);
        }
        public void SetPumpsFlowToProduce()
        {
            if (ProductionSKURun != null)
                CurrentPumps.ForEach(x => x.ActualFlow = ProductionSKURun.MaxMassFlow);
        }
        public bool IsCurrentProductionFinished()
        {
            if (ProductionSKURun?.IsCompleted ?? false)
            {
                return true;
            }
            return false;
        }
        public bool IfCanStopLineCompletely()
        {
            if (NextSKU == null)
            {
                return true;
            }
            return false;
        }
        public bool MustEmptyWipTanks()
        {
            if (ProductionOrder != null)
            {
                if (NextSKU == null)
                {
                    return true;
                }
                if (NextSKU.Material.Id != CurrentSKU!.Material.Id)
                {

                    return true;
                }


            }
            return false;
        }
        public bool MustChangeFormat()
        {
            if (NextSKU == null) return false;
            if (ProductionOrder != null)
            {
                if (CurrentSKU?.Size != NextSKU.Size)
                {
                    return true;
                }
            }
            return false;

        }
        public bool ReviewIfWipTanksIsLoLevel()
        {
            if (ProductionOrder != null)
            {
                return ProductionOrder.WIPs.All(x => x.CurrentLevel <= x.LoLevel);
            }
            return false;
        }
        public void CalculateAU()
        {
            ProductionSKURun?.CalculateTimeStarvedAU();

        }
    }
}


