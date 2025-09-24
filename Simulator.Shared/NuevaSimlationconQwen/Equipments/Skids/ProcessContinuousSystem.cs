using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids
{
    public class ProcessContinuousSystem : ManufaturingEquipment, ILiveReportable
    {


        public Amount ActualFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public Amount Capacity { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);

        public List<IManufactureFeeder> InletFeeder => InletEquipments.OfType<IManufactureFeeder>().ToList();


        public List<ProcessWipTankForLine> WIPForProducts => OutletEquipments.OfType<ProcessWipTankForLine>().ToList();
        public List<ProcessRecipedRawMaterialTank> WIPForRawMaterialProducts => OutletEquipments.OfType<ProcessRecipedRawMaterialTank>().ToList();
        public ReportColumn ReportColumn => ReportColumn.Column2_SkidsAndMixers;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.High;
        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            OutletState = new SKIDOutletWaitingStartCommandState(this);

        }
        public override void ValidateInletInitialState(DateTime currentdate)
        {
            InletState = new SKIDInletStateWaitingStartCommandState(this);
        }
        public List<LiveReportItem> GetLiveReportItems()
        {
            return new List<LiveReportItem>
        {
            new LiveReportItem
            {
                Label = "SKID",
                Value = Name,
                Style = new ReportStyle()
            },
            new LiveReportItem
            {
                Label = "Outlet State",
                Value = OutletState?.StateLabel ?? "Unknown",
                Style = GetStateStyle()
            },
            new LiveReportItem
            {
                Label = "Inlet State",
                Value = InletState?.StateLabel ?? "Unknown",
                Style = GetStateStyle()
            },
            new LiveReportItem
            {
                Label = "Flow",
                Value = ActualFlow.ToString() ?? "Unknown",
                Style = GetStateStyle()
            }
        };
        }

        private ReportStyle GetStateStyle() => new ReportStyle();

        bool InletStartCommandReceived = false;
        bool OutletStartCommandReceived = false;
        bool InletStopCommandReceived = false;
        bool OutletStopCommandReceived = false;
        public void Produce()
        {
            InletStartCommandReceived = true;
            OutletStartCommandReceived = true;
            InletStopCommandReceived = false;
            OutletStopCommandReceived = false;
        }
        public void Stop()
        {
            ActualFlow = ZeroFlow;
            InletStartCommandReceived = false;
            OutletStartCommandReceived = false;
            InletStopCommandReceived = true;
            OutletStopCommandReceived = true;
        }
        public bool IsOutletStartCommandReceived()
        {
            if (OutletStartCommandReceived)
            {
                OutletStartCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInletStartCommandReceived()
        {
            if (InletStartCommandReceived)
            {
                InletStartCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsOutletStopCommandReceived()
        {
            if (OutletStopCommandReceived)
            {
                OutletStopCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInletStopCommandReceived()
        {
            if (InletStopCommandReceived)
            {
                InletStopCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsSkidStarved()
        {
            if (InletState is ISKIDStarvedInletState)
            {
                ActualFlow = ZeroFlow;
                return true;
            }
            return false;
        }
        public bool IsSkidStarvedReleased()
        {
            if (InletState is not ISKIDStarvedInletState)
            {

                return true;
            }
            return false;
        }
        public void SendProductToWIPS()
        {
            if (InletState is not ISKIDStarvedInletState)
            {
                WIPForProducts.ForEach(x => x.ReceiveProductFromSKID(Capacity));
            }
        }
        public bool IsRawMaterialFeedersStarved()
        {
            var feeders = InletFeeder.Where(x => 
            x.OutletState is IFeederStarved
            ).ToList();
            if (feeders.Any())
            {
                var feeder = feeders.First();
                StartCriticalReport(feeder, "No available", feeder.OutletState?.StateLabel ?? "UnKnown");
                return true;
            }


            return false;
        }

        public bool IsRawMaterialFeedersReleaseStarved()
        {
            var feedersAvailable = InletFeeder.All(x =>
            x.OutletState is not IFeederStarved );
            if (feedersAvailable)
            {
                EndCriticalReport();
                return true;
            }


            return false;
        }


        public bool IsRawMaterialFeederReleased()
        {
            if (ProcessFeederManager.ReleaseEquipment(this))
            {
                return true;
            }

            //Aqui es para liberar las bombas luego de usarlas
            return false;
        }

        public bool IsFeederCatched()
        {
            if (CurrentOrder == null || CurrentOrder.Material == null) return false;
            if (CurrentOrder.Material is IRecipedMaterial material)
            {
                if (material?.RecipeSteps == null) return false;

                ActualFlow = Capacity;

                foreach (var step in material.RecipeSteps)
                {
                    var feeder = ProcessFeederManager.TryAssignFeeder(step.RawMaterialId, this);

                    if (feeder != null)
                    {

                        feeder.ActualFlow = Capacity * step.Percentage / 100;
                    }
                    else
                    {


                        ProcessFeederManager.ReleaseEquipment(this);
                        return false;
                    }
                }
                return true;
            }
            // Validación defensiva


            return false;


        }
        bool CommandFromQueueThatFeederIsRealesed = false;
        public bool IsFeederReleasedFromQueue()
        {
            if (CommandFromQueueThatFeederIsRealesed)
            {
                CommandFromQueueThatFeederIsRealesed = false;
                return true;
            }
            return false;
        }
        public override void GetReleaseFromManager(IManufactureFeeder feeder)
        {
            CommandFromQueueThatFeederIsRealesed = true;
        }
        public void ReceiveTotalStop()
        {
            TotalStopReceived = true;
        }
        bool TotalStopReceived = false;
        public bool IsSkidTotalStopReceived()
        {
            if (TotalStopReceived)
            {
                TotalStopReceived = false;
                return true;
            }
            return false;
        }

    }


}
