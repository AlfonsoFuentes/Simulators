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


        public List<ProcessWipTankForLine> WIPTanksAttached => _wipTanks ??= InletPumps.SelectMany(x => x.InletWipTanks).ToList();
        public List<ProcessMixer> PreferredManufacturer { get; set; } = new();

        private List<ProcessPump>? _inletPumps;
        private List<ProcessWipTankForLine>? _wipTanks;

        public List<ProcessPump> InletPumps => _inletPumps ??= InletEquipments.OfType<ProcessPump>().ToList();

        //Aqui hay que poner el miezclador de corrientes

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

        public bool IsLineScheduled => ProductionOrders.Any();
        public void QueueSKU(ProcessSKUByLine sku)
        {

            FromLineToWipProductionOrder orderforwips = new(this, sku);

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


        Queue<FromLineToWipProductionOrder> ProductionOrders { get; set; } = new Queue<FromLineToWipProductionOrder>();
        public FromLineToWipProductionOrder InformNextProductionOrder => ProductionOrders.Any() ? ProductionOrders.Peek() : null!;
        public FromLineToWipProductionOrder CurrentProductionOrder { get; set; } = null!;
        public FromLineToWipProductionOrder NextProductionOrder { get; set; } = null!;
        public bool InitSelectProductionRun()
        {
            if (ProductionOrders.Any())
            {

                CurrentProductionOrder = ProductionOrders.Dequeue();

                SendToWipProductionOrder(CurrentProductionOrder);

                return true;
            }

            return false;

        }
        public bool SelectNextProductionOrder()
        {
            if (NextProductionOrder != null)
            {

                CurrentProductionOrder = NextProductionOrder;

                NextProductionOrder = null!;
                return true;
            }

            return false;

        }

        void SendToWipProductionOrder(FromLineToWipProductionOrder order)
        {

            foreach (var wip in WIPTanksAttached)
            {
                wip.ReceiveInitFromLineProductionOrder(order);
            }
        }

        public void ReceivedWIPCurrentOrderRealesed(FromLineToWipProductionOrder productionorder)
        {
            if (ProductionOrders.Any())
            {

                NextProductionOrder = ProductionOrders.Dequeue();

                SendToWipProductionOrder(NextProductionOrder);
            }

        }



        public bool IsLineStarvedByLowLevelWips()
        {
            if (CurrentProductionOrder != null)
            {
                if (!CurrentProductionOrder.WIPs.Any())
                {
                    StartCriticalReport(this, $"No Manufaturer found for {CurrentProductionOrder.MaterialName}", $"Line {Name} stopped due to low level in WIP tank(s).");
                    return true;
                }
                var wipstarved = CurrentProductionOrder.WIPs.FirstOrDefault(x => x.OutletState is ITankOuletStarved);
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
            if (CurrentProductionOrder != null)
            {
                var wipstarved = CurrentProductionOrder.WIPs.All(x => x.OutletState is ITankOuletStarved);
                if (wipstarved)
                {
                    CurrentProductionOrder.WIPs.ForEach(x => x.CurrentLevel = ZeroMass);

                    return true;
                }

            }


            return false;
        }

        public bool MustRunByAu()
        {
            if (CurrentProductionOrder.ProductionSKURun == null) return true;
            if (CurrentProductionOrder.ProductionSKURun.IsRunningAU) return false;
            if (CurrentProductionOrder.ProductionSKURun!.Pending_Time_Producing <= ZeroTime)
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
            if (CurrentProductionOrder.ProductionSKURun == null) return true;
            if (!CurrentProductionOrder.ProductionSKURun.IsRunningAU) return false;
            if (CurrentProductionOrder.ProductionSKURun!.Pending_Time_StarvedByAU <= ZeroTime)
            {
                return true;
            }
            return false;

        }
        public bool IsLineAvailableAfterStarved()
        {
            if (CurrentProductionOrder != null)
            {
                if (CurrentProductionOrder.WIPs.All(x => x.OutletState is not ITankOuletStarved))
                {
                    EndCriticalReport();
                    return true;
                }


            }


            return false;
        }
        public void RunByAu()
        {
            CurrentProductionOrder.ProductionSKURun?.ProcessDuringAU();

        }
        public void RunByProducing()
        {
            CurrentProductionOrder.ProductionSKURun?.Produce();

        }

        List<ProcessPump> CurrentPumps => CurrentProductionOrder == null ? new List<ProcessPump>() : CurrentProductionOrder.WIPs.SelectMany(x => x.OutletPumps).ToList();
        public void SetPumpsFlowToZero()
        {
            if (CurrentProductionOrder.ProductionSKURun != null)
                CurrentPumps.ForEach(x => x.ActualFlow = ZeroFlow);
        }
        public void SetPumpsFlowToProduce()
        {
            if (CurrentProductionOrder.ProductionSKURun != null)
                CurrentPumps.ForEach(x => x.ActualFlow = CurrentProductionOrder.ProductionSKURun.MaxMassFlow);
        }
        public bool IsCurrentProductionFinished()
        {
            if (CurrentProductionOrder.ProductionSKURun?.IsCompleted ?? false)
            {
                return true;
            }
            return false;
        }
        public bool IfCanStopLineCompletely()
        {
            if (NextProductionOrder == null && !ProductionOrders.Any())
            {
                CurrentProductionOrder = null!;
                return true;
            }
            return false;
        }
        public bool MustEmptyWipTanks()
        {
            if (CurrentProductionOrder != null)
            {
                if (NextProductionOrder == null)
                {
                    return true;
                }
                if (CurrentProductionOrder.Material.Id != NextProductionOrder!.Material.Id)
                {
                    if (CurrentProductionOrder.WIPs.Any(x => x.CurrentLevel.Value > 0))
                        return true;
                }


            }
            return false;
        }
        public bool MustChangeFormat()
        {
            FromLineToWipProductionOrder _nextproductionorder = 
                NextProductionOrder != null? NextProductionOrder: ProductionOrders.Any() ? ProductionOrders.Peek() : null!;
            if (_nextproductionorder == null)
            {
                return false;
            }
            if (CurrentProductionOrder != null)
            {
                if (CurrentProductionOrder.Size != _nextproductionorder.Size)
                {
                    return true;
                }
            }
            return false;

        }
        public bool ReviewIfWipTanksIsLoLevel()
        {
            if (CurrentProductionOrder != null)
            {
                return CurrentProductionOrder.WIPs.All(x => x.CurrentLevel <= x.LoLevel);
            }
            return false;
        }
        public void CalculateAU()
        {
            CurrentProductionOrder.ProductionSKURun?.CalculateTimeStarvedAU();

        }
    }
}


